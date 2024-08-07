const std = @import("std");
const fs = std.fs;
const sdl2 = @cImport(
    @cInclude("SDL2/SDL.h")
);
const stb_image = @cImport(
    @cInclude("stb_image.h")
);
const stb_image_write = @cImport(
    @cInclude("stb_image_write.h")
);
const stb_truetype = @cImport(
    @cInclude("stb_truetype.h")
);
const qoi = @cImport(
    @cInclude("qoi.h")
);

const RiateuFont = [*c]stb_truetype.stbtt_fontinfo;

export fn Riateu_LoadImage(data: [*c]const u8, length: c_int, width: [*c]c_int, height: [*c]c_int, len: [*c]c_int) [*c] u8 {
    var pixels: [*c] u8 = undefined;
    var result: [*c] u8 = undefined;
    if (valid_qoi_image(data, length)) {
        const option_result = load_qoi_image(data, length, width, height);
        if (option_result) |opt| {
            result = opt;
        } else {
            sdl2.SDL_LogError(sdl2.SDL_LOG_CATEGORY_ERROR, "Unable to load QOI image", .{});
            return null;
        }
    } else {
        var w: c_int = undefined;
        var h: c_int = undefined;
        var channels: c_int = undefined;
        result = stb_image.stbi_load_from_memory(data, length, &w, &h, &channels, 4);

        width.* = w;
        height.* = h;
    }

    pixels = result;

    len.* = width.* * height.* * 4;
    var i: u32 = 0;
    while (i < len.*) {
        if (pixels[3] == 0) {
            pixels[0] = 0;
            pixels[1] = 1;
            pixels[2] = 2;
        }
        pixels += 4;
        i += 4;
    }

    return result;
}

export fn Riateu_FreeImage(data: [*c] u8) void {
    stb_image.stbi_image_free(data);
}

export fn Riateu_WritePNG(filename: [*c]const u8, data: [*c]const u8, width: c_int, height: c_int) c_int {
    return stb_image_write.stbi_write_png(filename, width, height, 4, data, width * 4);
}

export fn Riateu_WriteQOI(filename: [*c]const u8, data: [*c]const u8, width: c_int, height: c_int) c_int {
    const allocator = std.heap.c_allocator;

    var desc = allocator.create(qoi.qoi_desc) catch {
        return 1;
    };
    defer allocator.destroy(desc);
    desc.width = @as(c_uint, @intCast(width));
    desc.height = @as(c_uint, @intCast(height));
    desc.channels = 4;
    desc.colorspace = 1;

    var length: c_int = 0;
    const result = qoi.qoi_encode(data, desc, &length);
    defer std.c.free(result);
    if (result) |res| {
        const dataRes: []const u8 = @as([*]u8, @ptrCast(res))[0..@as(usize, @intCast(length))];
        const fname: []const u8 = std.mem.span(filename);
        var file = fs.cwd().createFile(fname, .{}) catch {
            return 1;
        };
        defer file.close();
        file.writeAll(dataRes) catch {
            return 1;
        };

        return 0;
    }
    return 1;
}

export fn Riateu_FontInit(data: [*c]const u8) RiateuFont {
    if (stb_truetype.stbtt_GetNumberOfFonts(data) <= 0) 
    {
        sdl2.SDL_LogError(sdl2.SDL_LOG_CATEGORY_ERROR, "Number of fonts were below 0.", .{});
        return null;
    }

    const info = std.c.malloc(@sizeOf(stb_truetype.stbtt_fontinfo));

    if (info) |inf| {
        const info_strict = @as([*]stb_truetype.stbtt_fontinfo, @alignCast(@ptrCast(inf)));
        if (stb_truetype.stbtt_InitFont(info_strict, data, 0) == 0) {
            sdl2.SDL_LogError(sdl2.SDL_LOG_CATEGORY_ERROR, "Unable to create font.", .{});
            std.c.free(info);
            return null;
        }

        return info_strict;
    }

    return null;
}

export fn Riateu_FontGetCharacter(
    font: RiateuFont, glyph: c_int, scale: f32,
    width: [*c]c_int, height: [*c]c_int, advance: [*c]f32,
    offsetX: [*c]f32, offsetY: [*c]f32, visible: [*c]c_int
) void {
    var adv: c_int = 0;
    var x0: c_int = 0;
    var y0: c_int = 0;
    var x1: c_int = 0;
    var y1: c_int = 0;
    var offX: c_int = 0;

    stb_truetype.stbtt_GetGlyphHMetrics(font, glyph, &adv, &offX);
    stb_truetype.stbtt_GetGlyphBitmapBox(font, glyph, scale, scale, &x0, &y0, &x1, &y1);

    width.* = (x1 - x0);
    height.* = (y1 - y0);
    advance.* = @as(f32, @floatFromInt(adv)) * scale;
    offsetX.* = @as(f32, @floatFromInt(offX)) * scale;
    offsetY.* = @floatFromInt(y0);
    if (width.* > 0 and height.* > 0 and stb_truetype.stbtt_IsGlyphEmpty(font, glyph) == 0) {
        visible.* = 1;
    } else {
        visible.* = 0;
    }
}

export fn Riateu_FontGetPixels(font: RiateuFont, dest: [*c]u8, glyph: c_int, width: c_int, height: c_int, scale: f32) void {
    stb_truetype.stbtt_MakeGlyphBitmap(font, dest, width, height, width, scale, scale, glyph);

    const len: c_int = width * height;
    var idx: usize = @as(usize, @intCast(len - 1)) * 4;
    var point: usize = @intCast(len - 1);
    while (point >= 0) 
    {
        dest[idx] = dest[point];
        dest[idx + 1] = dest[point];
        dest[idx + 2] = dest[point];
        dest[idx + 3] = dest[point];

        if (point > 0) 
        {
            point -= 1;
            idx -= 4;
            continue;
        }
        break;
    }
}

export fn Riateu_FontGetMetrics(font: RiateuFont, ascent: [*c]c_int, descent: [*c]c_int, line_gap: [*c]c_int) void {
    stb_truetype.stbtt_GetFontVMetrics(font, ascent, descent, line_gap);
}

export fn Riateu_FontFindGlyphIndex(font: RiateuFont, codepoint: c_int) c_int {
    return stb_truetype.stbtt_FindGlyphIndex(font, codepoint);
}

export fn Riateu_FontGetPixelScale(font: RiateuFont, scale: f32) f32 {
    return stb_truetype.stbtt_ScaleForMappingEmToPixels(font, scale);
}

export fn Riateu_FontGetKerning(font: RiateuFont, glyph1: c_int, glyph2: c_int, scale: f32) f32 {
    const kerning = stb_truetype.stbtt_GetGlyphKernAdvance(font, glyph1, glyph2);
    return @as(f32, @floatFromInt(kerning)) * scale;
}

export fn Riateu_FontFree(font: RiateuFont) void {
    std.c.free(font);
}

const QOI_HEADER_SIZE = 14;
const QOI_MAGIC: comptime_int = 'q' << 24 | '0' << 16 | 'i' << 8 | 'f';

inline fn read_qoi_magic(data: [*c]const u8) u32 {
    comptime var idx = 0;
    const q: u32 = data[idx];
    idx += 1;
    const o: u32 = data[idx];
    idx += 1;
    const i: u32 = data[idx];
    idx += 1;
    const f: u32 = data[idx];

    return q << 24 | o << 16 | i << 8 | f;
}

inline fn valid_qoi_image(data: [*c]const u8, length: c_int) bool {
    if (length < QOI_HEADER_SIZE) 
    {
        return false;
    }
    const magic = read_qoi_magic(data);
    if (magic == QOI_MAGIC) 
    {
        return true;
    }
    return false;
}

fn load_qoi_image(data: [*c]const u8, length: c_int, width: [*c]c_int, height: [*c]c_int) ?[*c] u8 {
    const desc: [*c]qoi.qoi_desc = null;
    const result = qoi.qoi_decode(data, length, desc, 4);

    if (result) |res| {
        width.* = @as(c_int, @intCast(desc.*.width));
        height.* = @as(c_int, @intCast(desc.*.height));
        return @as([*c]u8, @ptrCast(res));
    } else {
        width.* = 0;
        height.* = 0;
        return null;
    }
}

test {
    var arena = std.heap.ArenaAllocator.init(std.heap.c_allocator);
    defer arena.deinit();

    var allocator = arena.allocator();
    var file = try fs.cwd().openFile("PressStart2P-Regular.ttf", .{});
    const size = try file.getEndPos();
    const buffer = try allocator.alloc(u8, size);
    defer allocator.free(buffer);

    _ = try file.readAll(buffer);
    const font = Riateu_FontInit(buffer.ptr);
    defer Riateu_FontFree(font);

    const glyph = Riateu_FontFindGlyphIndex(font, 40);
    const scale = Riateu_FontGetPixelScale(font, 32);
    var width: c_int = 0;
    var height: c_int = 0;
    var advance: f32 = 0;
    var offsetX: f32 = 0;
    var offsetY: f32 = 0;
    var visible: bool = false;
    Riateu_FontGetCharacter(font, glyph, scale, &width, &height, &advance, &offsetX, &offsetY, &visible);

    const bytes = try allocator.alloc(u8, @intCast(width * height));
    defer allocator.free(bytes);

    Riateu_FontGetPixels(font, bytes.ptr, glyph, width, height, scale);
}