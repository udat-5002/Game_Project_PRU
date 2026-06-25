using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Biển chào tại spawn — chỉ hiện khu/chương đang chơi, không vẽ mảng màu trên đất.
/// </summary>
public class ForestZoneGuide : MonoBehaviour
{
    public int activeChapter = 1;

    void Start()
    {
        var zone = ForestZoneLayout.GetZone(activeChapter);
        var spawn = GetChapterSpawn(activeChapter);
        var pos = GroundSnap.Snap(spawn, 0.05f);

        BuildWelcomeSign(pos + Vector3.forward * 4f, zone);
    }

    static Vector3 GetChapterSpawn(int chapter) => chapter switch
    {
        1 => ForestZoneLayout.Ch1Spawn,
        2 => ForestZoneLayout.Ch2Spawn,
        3 => ForestZoneLayout.Ch3Spawn,
        _ => ForestZoneLayout.Ch1Spawn
    };

    void BuildWelcomeSign(Vector3 pos, ForestZoneLayout.Zone zone)
    {
        var root = new GameObject("WelcomeSign");
        root.transform.SetParent(transform);
        root.transform.position = pos;

        var pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.transform.SetParent(root.transform);
        pole.transform.localPosition = new Vector3(0f, 2f, 0f);
        pole.transform.localScale = new Vector3(0.4f, 2f, 0.4f);
        Object.Destroy(pole.GetComponent<Collider>());
        ApplyColor(pole.GetComponent<Renderer>(), zone.signColor);

        CreateWorldLabel(root.transform, new Vector3(0f, 4.2f, 0f),
            zone.shortName + "\n" + zone.fullName + "\n\nTheo mũi tên trên HUD",
            48, Color.white);
    }

    void CreateWorldLabel(Transform parent, Vector3 localPos, string text, int fontSize, Color color)
    {
        var canvasGo = new GameObject("Label");
        canvasGo.transform.SetParent(parent);
        canvasGo.transform.localPosition = localPos;

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasGo.AddComponent<FaceCamera>();
        var rt = canvasGo.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(620, 200);
        rt.localScale = Vector3.one * 0.018f;

        var bgGo = new GameObject("Bg", typeof(RectTransform), typeof(Image));
        bgGo.transform.SetParent(canvasGo.transform, false);
        var bgRt = bgGo.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;
        bgGo.GetComponent<Image>().color = new Color(0.02f, 0.02f, 0.03f, 0.98f);

        var labelGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
        labelGo.transform.SetParent(canvasGo.transform, false);
        var labelRt = labelGo.GetComponent<RectTransform>();
        labelRt.anchorMin = Vector2.zero;
        labelRt.anchorMax = Vector2.one;
        labelRt.offsetMin = new Vector2(12, 12);
        labelRt.offsetMax = new Vector2(-12, -12);

        var t = labelGo.GetComponent<Text>();
        CrispUiText.ConfigureWorldLabel(t, fontSize, color, text);
    }

    static void ApplyColor(Renderer renderer, Color color)
    {
        if (renderer == null) return;
        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var mat = new Material(shader);
        mat.SetColor("_BaseColor", color);
        mat.color = color;
        renderer.material = mat;
    }
}
