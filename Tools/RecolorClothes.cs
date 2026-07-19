using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

class RecolorClothes
{
    enum Mode { Soldier, BaBa }

    static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: RecolorClothes <soldier|baba> <imagePath>");
            return 1;
        }

        Mode mode = args[0].Equals("baba", StringComparison.OrdinalIgnoreCase) ? Mode.BaBa : Mode.Soldier;
        string path = args[1];

        int changed;
        string tmp = path + ".recolored.jpg";
        byte[] fileBytes = File.ReadAllBytes(path);
        using (var ms = new MemoryStream(fileBytes))
        using (var src = new Bitmap(ms))
        using (var img = src.Clone(new Rectangle(0, 0, src.Width, src.Height), PixelFormat.Format24bppRgb))
        {
            changed = Process(img, mode);
            var encoder = GetJpegEncoder();
            var ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(Encoder.Quality, 95L);
            if (File.Exists(tmp)) File.Delete(tmp);
            img.Save(tmp, encoder, ep);
        }

        // Ghi de an toan: thu xoa/ghi lai, neu bi khoa thi giu file .recolored.jpg
        try
        {
            if (File.Exists(path)) File.Delete(path);
            File.Move(tmp, path);
            Console.WriteLine(mode + " pixels recolored: " + changed + " -> " + path);
        }
        catch (IOException)
        {
            Console.WriteLine(mode + " pixels recolored: " + changed + " (locked, saved as " + tmp + ")");
        }
        return 0;
    }

    static ImageCodecInfo GetJpegEncoder()
    {
        foreach (var c in ImageCodecInfo.GetImageEncoders())
            if (c.FormatID == ImageFormat.Jpeg.Guid) return c;
        throw new InvalidOperationException("JPEG encoder missing");
    }

    static int Process(Bitmap img, Mode mode)
    {
        var rect = new Rectangle(0, 0, img.Width, img.Height);
        var data = img.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
        int stride = data.Stride;
        byte[] buf = new byte[Math.Abs(stride) * img.Height];
        Marshal.Copy(data.Scan0, buf, 0, buf.Length);
        int changed = 0;

        for (int y = 0; y < img.Height; y++)
        {
            int row = y * stride;
            for (int x = 0; x < img.Width; x++)
            {
                int i = row + x * 3;
                byte b = buf[i], g = buf[i + 1], r = buf[i + 2];
                float h, s, v;
                RgbToHsv(r, g, b, out h, out s, out v);

                bool write = false;
                if (mode == Mode.Soldier)
                    write = TrySoldier(h, s, v, ref r, ref g, ref b);
                else
                    write = TryBaBa(h, s, v, ref r, ref g, ref b);

                if (write)
                {
                    buf[i] = b;
                    buf[i + 1] = g;
                    buf[i + 2] = r;
                    changed++;
                }
            }
        }

        Marshal.Copy(buf, 0, data.Scan0, buf.Length);
        img.UnlockBits(data);
        return changed;
    }

    static bool TrySoldier(float h, float s, float v, ref byte r, ref byte g, ref byte b)
    {
        if (IsSkin(h, s, v)) return false;

        bool isOlive =
            ((h >= 35f && h <= 120f) || (s < 0.22f && v > 0.18f && v < 0.75f && g >= r && g + 8 >= b))
            && v > 0.16f && v < 0.82f;
        bool isDarkGear = v < 0.18f || (s < 0.08f && v < 0.35f);
        if (!isOlive || isDarkGear) return false;

        // Xanh quan doi / ao linh cu Ho: olive drab (khong xanh la cay)
        float newH = 72f;
        float newS = Clamp(s * 0.9f + 0.22f, 0.28f, 0.48f);
        float newV = Clamp(v * 0.88f + 0.02f, 0.20f, 0.58f);
        HsvToRgb(newH, newS, newV, out r, out g, out b);
        return true;
    }

    static bool TryBaBa(float h, float s, float v, ref byte r, ref byte g, ref byte b)
    {
        // Chi giu da sang (mat/tay); vai nau vua cung chuyen thanh ao ba ba
        if (IsBrightSkin(h, s, v, r, g, b)) return false;

        bool warmFabric = h >= 5f && h <= 65f && s >= 0.04f && v > 0.05f && v < 0.82f;
        bool neutralDark = s < 0.32f && v > 0.04f && v < 0.72f;
        bool midBrownShirt = r < 170 && g < 140 && b < 120 && v < 0.70f && v > 0.12f && r >= g && g >= b - 10;
        if (!(warmFabric || neutralDark || midBrownShirt)) return false;

        // Ao ba ba den / canh gian toi: vai mat
        float luma = 0.299f * (r / 255f) + 0.587f * (g / 255f) + 0.114f * (b / 255f);
        float newH = 215f;
        float newS = 0.07f;
        float newV = Clamp(luma * 0.48f + 0.03f, 0.05f, 0.28f);
        HsvToRgb(newH, newS, newV, out r, out g, out b);
        return true;
    }

    static bool IsSkin(float h, float s, float v)
    {
        return h >= 0f && h <= 45f && s >= 0.12f && s <= 0.65f && v >= 0.28f && v <= 0.95f;
    }

    static bool IsBrightSkin(float h, float s, float v, byte r, byte g, byte b)
    {
        // Da that su thuong sang hon vai ao nau
        return h >= 0f && h <= 42f
            && s >= 0.12f && s <= 0.60f
            && v >= 0.42f
            && r >= 125
            && (r - b) >= 40
            && r > g;
    }

    static float Clamp(float x, float a, float b)
    {
        return x < a ? a : (x > b ? b : x);
    }

    static void RgbToHsv(byte R, byte G, byte B, out float h, out float s, out float v)
    {
        float r = R / 255f, g = G / 255f, b = B / 255f;
        float max = Math.Max(r, Math.Max(g, b));
        float min = Math.Min(r, Math.Min(g, b));
        v = max;
        float d = max - min;
        s = max <= 0f ? 0f : d / max;
        h = 0f;
        if (d > 1e-5f)
        {
            if (max == r) h = 60f * (((g - b) / d) % 6f);
            else if (max == g) h = 60f * (((b - r) / d) + 2f);
            else h = 60f * (((r - g) / d) + 4f);
            if (h < 0f) h += 360f;
        }
    }

    static void HsvToRgb(float h, float s, float v, out byte R, out byte G, out byte B)
    {
        float c = v * s;
        float x = c * (1f - Math.Abs((h / 60f) % 2f - 1f));
        float m = v - c;
        float rp = 0, gp = 0, bp = 0;
        if (h < 60f) { rp = c; gp = x; }
        else if (h < 120f) { rp = x; gp = c; }
        else if (h < 180f) { gp = c; bp = x; }
        else if (h < 240f) { gp = x; bp = c; }
        else if (h < 300f) { rp = x; bp = c; }
        else { rp = c; bp = x; }
        R = (byte)Math.Round(Clamp((rp + m) * 255f, 0, 255));
        G = (byte)Math.Round(Clamp((gp + m) * 255f, 0, 255));
        B = (byte)Math.Round(Clamp((bp + m) * 255f, 0, 255));
    }
}
