using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tạo chữ UI sắc nét: pre-render font đúng cỡ hiển thị, không dùng Outline/Shadow (hay làm mờ).
/// </summary>
public static class CrispUiText
{
    static Font cachedFont;
    static readonly HashSet<int> WarmedSizes = new HashSet<int>();

    public static readonly Color White = Color.white;
    public static readonly Color Gold = new Color(1f, 0.9f, 0.4f, 1f);
    public static readonly Color Cyan = new Color(0.65f, 0.98f, 1f, 1f);

    public const string VietnameseChars =
        "0123456789 ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz" +
        "ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂưăạảấầẩẫậắằẳẵặẹẻẽềềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵýỷỹ•/:";

    public static Font GetFont()
    {
        if (cachedFont != null) return cachedFont;
        try
        {
            cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[CrispUiText] Không load được font: " + e.Message);
        }
        return cachedFont;
    }

    public static void WarmAtlas(params int[] sizes)
    {
        var font = GetFont();
        if (font == null || sizes == null || sizes.Length == 0) return;

        try
        {
            foreach (var size in sizes)
            {
                if (!WarmedSizes.Add(size)) continue;
                font.RequestCharactersInTexture(VietnameseChars, size, FontStyle.Bold);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[CrispUiText] WarmAtlas: " + e.Message);
        }
    }

    public static void WarmMenuAtlas() =>
        WarmAtlas(52, 44, 40, 34);

    public static void WarmGameplayAtlas() =>
        WarmAtlas(40, 36, 34, 30);

    public static void ApplyReadableDefaults(Text t)
    {
        if (t == null) return;
        t.supportRichText = false;
        t.resizeTextForBestFit = false;
        t.alignByGeometry = false;
        t.raycastTarget = false;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        var c = t.color;
        t.color = new Color(c.r, c.g, c.b, 1f);
    }

    public static Text Create(Transform parent, string name, string text, int fontSize,
        TextAnchor align, Vector2 anchor, Vector2 sizeDelta, Color color,
        FontStyle style = FontStyle.Bold)
    {
        if (GetFont() == null) return null;
        WarmAtlas(fontSize);

        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.sizeDelta = sizeDelta;
        rt.anchoredPosition = Vector2.zero;

        var t = go.GetComponent<Text>();
        var font = GetFont();
        if (font == null)
        {
            Debug.LogError("[CrispUiText] Font null — không tạo được Text.");
            Object.Destroy(go);
            return null;
        }
        t.font = font;
        t.fontSize = fontSize;
        t.fontStyle = style;
        t.alignment = align;
        t.color = color;
        t.text = text;
        ApplyReadableDefaults(t);
        return t;
    }

    public static void ConfigureWorldLabel(Text t, int fontSize, Color color, string text)
    {
        if (t == null) return;
        t.font = GetFont();
        t.fontSize = fontSize;
        t.fontStyle = FontStyle.Bold;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = color;
        t.text = text;
        ApplyReadableDefaults(t);
        WarmAtlas(fontSize);
    }
}
