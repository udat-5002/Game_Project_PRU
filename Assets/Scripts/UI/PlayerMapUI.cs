using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Bản đồ hành trình — nhấn Tab để mở, xem điểm đến và tuyến đường.
/// </summary>
public class PlayerMapUI : MonoBehaviour
{
    public static PlayerMapUI Instance { get; private set; }

    public bool IsOpen { get; private set; }

    struct MapMarker
    {
        public string id;
        public string label;
        public Vector3 worldPos;
        public bool isDelivery;
        public bool isShelterHint;
    }

    Canvas canvas;
    GameObject root;
    RectTransform mapArea;
    Text titleText;
    Text objectiveText;
    Text hintText;
    readonly List<GameObject> dynamicMarkers = new List<GameObject>();
    readonly List<MapMarker> route = new List<MapMarker>();

    Image playerDot;
    RectTransform playerDotRt;

    static readonly Color PanelBg = new Color(0.02f, 0.02f, 0.03f, 0.94f);
    static readonly Color MapBg = new Color(0.12f, 0.16f, 0.12f, 1f);
    static readonly Color RouteLine = new Color(0.85f, 0.75f, 0.35f, 0.75f);
    static readonly Color MarkerIdle = new Color(0.75f, 0.78f, 0.7f, 1f);
    static readonly Color MarkerActive = new Color(1f, 0.82f, 0.2f, 1f);
    static readonly Color MarkerDelivery = new Color(0.35f, 0.85f, 0.45f, 1f);
    static readonly Color MarkerShelter = new Color(0.45f, 0.75f, 1f, 0.9f);
    static readonly Color PlayerColor = new Color(0.35f, 0.7f, 1f, 1f);

    Vector2 mapMinXZ;
    Vector2 mapMaxXZ;
    bool lockedInputByMap;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        Build();
        SetOpen(false);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
        if (lockedInputByMap && GameManager.Instance != null)
            GameManager.Instance.LockInput(false);
    }

    void Update()
    {
        if (!CanUseMap())
        {
            if (IsOpen)
                SetOpen(false);
            return;
        }

        if (GameInput.MapPressedThisFrame)
        {
            if (IsOpen)
                SetOpen(false);
            else if (!IsBlocked())
                SetOpen(true);
            return;
        }

        if (IsOpen && GameInput.EscapePressedThisFrame)
        {
            SetOpen(false);
            return;
        }

        if (IsOpen)
            RefreshPlayerDot();
    }

    bool CanUseMap()
    {
        string scene = SceneManager.GetActiveScene().name;
        if (scene != GameManager.SceneChapter1 &&
            scene != GameManager.SceneChapter2 &&
            scene != GameManager.SceneChapter3)
            return false;

        if (GameManager.Instance == null) return false;
        int chapter = GameManager.Instance.CurrentChapter;
        return chapter >= 1 && chapter <= 3;
    }

    bool IsBlocked()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsShowing)
            return true;
        if (GameManager.Instance != null && GameManager.Instance.InputLocked && !lockedInputByMap)
            return true;
        return false;
    }

    public void SetOpen(bool open)
    {
        IsOpen = open;
        if (root != null)
            root.SetActive(open);

        if (open)
        {
            RebuildRoute();
            RefreshMarkers();
            RefreshPlayerDot();
            TryCompleteMandatoryMapCheck();
            if (GameManager.Instance != null && !GameManager.Instance.InputLocked)
            {
                GameManager.Instance.LockInput(true);
                lockedInputByMap = true;
            }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            if (lockedInputByMap && GameManager.Instance != null)
            {
                GameManager.Instance.LockInput(false);
                lockedInputByMap = false;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void TryCompleteMandatoryMapCheck()
    {
        if (QuestManager.Instance == null) return;
        if (!QuestManager.Instance.IsStepActive("check_map")) return;

        QuestManager.Instance.CompleteStep("check_map");
        GameSession.UnlockMapRoute();

        int chapter = GameManager.Instance != null ? GameManager.Instance.CurrentChapter : 1;
        string doneMsg = chapter switch
        {
            2 => Chapter2Dialogue.CheckMapDone,
            3 => Chapter3Dialogue.CheckMapDone,
            _ => Chapter1Dialogue.CheckMapDone
        };
        GameUI.Instance?.ShowNotification(doneMsg, 4f, CrispUiText.Gold);

        // Ch3: bảo đảm waypoint manh mối sẵn sàng sau khi mở khóa
        if (chapter == 3 && ChapterFlowController.Active != null)
            ChapterFlowController.Active.RefreshClueWaypointsAfterMapUnlock();

        RebuildRoute();
        RefreshMarkers();
    }

    void Build()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 1f;
            gameObject.AddComponent<GraphicRaycaster>();
        }

        root = CreatePanel("MapRoot", PanelBg, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        titleText = CreateText(root.transform, "Title", 56, new Vector2(0f, 440f), new Vector2(0.5f, 0.5f),
            new Vector2(1100, 70), CrispUiText.Gold, FontStyle.Bold);
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.text = "BẢN ĐỒ HÀNH TRÌNH";
        CrispUiText.WarmAtlas(56);

        objectiveText = CreateText(root.transform, "Objective", 38, new Vector2(0f, 375f), new Vector2(0.5f, 0.5f),
            new Vector2(1200, 56), Color.white, FontStyle.Bold);
        objectiveText.alignment = TextAnchor.MiddleCenter;
        CrispUiText.WarmAtlas(38);

        var mapPanel = CreatePanel("MapArea", MapBg,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0f, -20f), new Vector2(980f, 620f));
        mapPanel.transform.SetParent(root.transform, false);
        var mapRt = mapPanel.GetComponent<RectTransform>();
        mapRt.anchorMin = new Vector2(0.5f, 0.5f);
        mapRt.anchorMax = new Vector2(0.5f, 0.5f);
        mapRt.pivot = new Vector2(0.5f, 0.5f);
        mapRt.anchoredPosition = new Vector2(0f, -10f);
        mapRt.sizeDelta = new Vector2(980f, 620f);
        mapArea = mapRt;

        // Border
        var border = new GameObject("Border", typeof(RectTransform), typeof(Image));
        border.transform.SetParent(mapArea, false);
        var borderRt = border.GetComponent<RectTransform>();
        borderRt.anchorMin = Vector2.zero;
        borderRt.anchorMax = Vector2.one;
        borderRt.offsetMin = new Vector2(-4f, -4f);
        borderRt.offsetMax = new Vector2(4f, 4f);
        border.GetComponent<Image>().color = new Color(0.7f, 0.6f, 0.3f, 0.55f);
        border.transform.SetAsFirstSibling();

        playerDot = CreateDot(mapArea, "Player", PlayerColor, 22f);
        playerDotRt = playerDot.rectTransform;

        hintText = CreateText(root.transform, "Hint", 32, new Vector2(0f, -430f), new Vector2(0.5f, 0.5f),
            new Vector2(1200, 44), Color.white, FontStyle.Bold);
        hintText.alignment = TextAnchor.MiddleCenter;
        hintText.text = "Tab đóng  •  Vàng = mục tiêu hiện tại  •  Xanh = điểm giao thư";

        var legend = CreateText(root.transform, "Legend", 30, new Vector2(0f, -380f), new Vector2(0.5f, 0.5f),
            new Vector2(1200, 40), new Color(0.95f, 0.95f, 0.9f), FontStyle.Bold);
        legend.alignment = TextAnchor.MiddleCenter;
        legend.text = "Mở bản đồ để đối chiếu đường đi và điểm giao thư trước khi lên đường.";
    }

    void RebuildRoute()
    {
        route.Clear();
        int chapter = GameManager.Instance != null ? GameManager.Instance.CurrentChapter : 1;

        if (hintText != null)
        {
            hintText.text = chapter == 2
                ? "Tab đóng  •  Vàng = mục tiêu  •  Xanh = giao thư  •  Xanh dương = chỗ trú mưa"
                : "Tab đóng  •  Vàng = mục tiêu hiện tại  •  Xanh = điểm giao thư";
        }

        switch (chapter)
        {
            case 1:
                titleText.text = "BẢN ĐỒ — Chương 1: Nhận Thư, Mở Bản Đồ";
                Add("pickup_mail", "Trạm Liên Lạc", ForestZoneLayout.Ch1MailStation);
                Add("ask_elder", "Cụ già / hỏi đường", ForestZoneLayout.Ch1Elder);
                Add("cross_obstacle", "Khu gỗ đổ", ForestZoneLayout.Ch1Obstacle);
                Add("landmark_banyan", "Ngã ba cây đa", ForestZoneLayout.Ch1LandmarkBanyan);
                Add("landmark_well", "Giếng hoang", ForestZoneLayout.Ch1LandmarkWell);
                Add("deliver_mail", "Giao thư: Bà Lan", ForestZoneLayout.Ch1Delivery, delivery: true);
                SetBoundsFrom(
                    ForestZoneLayout.Ch1Spawn,
                    ForestZoneLayout.Ch1Elder,
                    ForestZoneLayout.Ch1Obstacle,
                    ForestZoneLayout.Ch1LandmarkBanyan,
                    ForestZoneLayout.Ch1LandmarkWell,
                    ForestZoneLayout.Ch1Delivery);
                break;
            case 2:
                titleText.text = "BẢN ĐỒ — Chương 2: Thư Người Lính";
                Add("receive_letter", "Người lính trẻ", ForestZoneLayout.Ch2Soldier);
                Add("shelter_rain", "Chòi trú mưa", ForestZoneLayout.Ch2RainShelter, shelter: true);
                Add("keep_letter_dry", "Hết đoạn mưa", ForestZoneLayout.Ch2RainPathEnd);
                Add("deliver_mother", "Giao thư: Mẹ người lính", ForestZoneLayout.Ch2Mother, delivery: true);
                SetBoundsFrom(
                    ForestZoneLayout.Ch2Spawn,
                    ForestZoneLayout.Ch2Soldier,
                    ForestZoneLayout.Ch2RainShelter,
                    ForestZoneLayout.Ch2RainPathEnd,
                    ForestZoneLayout.Ch2Mother);
                break;
            default:
                titleText.text = "BẢN ĐỒ — Chương 3: Lá Thư Cuối Cùng";
                Add("house", "Nhà bỏ hoang", ForestZoneLayout.ResolveHouseCluePosition());
                Add("bunker", "Hầm trú ẩn", ForestZoneLayout.Ch3Clue2);
                Add("fort", "Đồn lính đổ nát", ForestZoneLayout.Ch3Clue3);
                Add("final_delivery", "Giao thư cuối", ForestZoneLayout.Ch3FinalDelivery, delivery: true);
                SetBoundsFrom(ForestZoneLayout.Ch3Spawn, ForestZoneLayout.Ch3FinalDelivery, ForestZoneLayout.Ch3Clue1);
                break;
        }

        // Prefer live registered waypoints when available
        for (int i = 0; i < route.Count; i++)
        {
            var m = route[i];
            if (QuestWaypointRegistry.TryGet(m.id, out var live))
            {
                m.worldPos = live;
                route[i] = m;
            }
        }

        string objective = "Chưa có nhiệm vụ";
        if (QuestManager.Instance?.CurrentStep != null)
            objective = "Mục tiêu: " + QuestManager.Instance.CurrentStep.description;
        objectiveText.text = objective;
    }

    void Add(string id, string label, Vector3 world, bool delivery = false, bool shelter = false)
    {
        route.Add(new MapMarker
        {
            id = id,
            label = label,
            worldPos = world,
            isDelivery = delivery,
            isShelterHint = shelter
        });
    }

    void SetBoundsFrom(params Vector3[] points)
    {
        float minX = float.MaxValue, maxX = float.MinValue;
        float minZ = float.MaxValue, maxZ = float.MinValue;
        foreach (var p in points)
        {
            minX = Mathf.Min(minX, p.x);
            maxX = Mathf.Max(maxX, p.x);
            minZ = Mathf.Min(minZ, p.z);
            maxZ = Mathf.Max(maxZ, p.z);
        }

        float pad = 18f;
        mapMinXZ = new Vector2(minX - pad, minZ - pad);
        mapMaxXZ = new Vector2(maxX + pad, maxZ + pad);
    }

    void RefreshMarkers()
    {
        foreach (var go in dynamicMarkers)
        {
            if (go != null) Destroy(go);
        }
        dynamicMarkers.Clear();

        string activeId = QuestManager.Instance?.CurrentStep?.id;

        // Route lines between main objectives (bỏ chỗ trú để đường không rối)
        var pathPoints = new List<Vector2>();
        foreach (var m in route)
        {
            if (m.isShelterHint) continue;
            pathPoints.Add(WorldToMap(m.worldPos));
        }

        for (int i = 0; i < pathPoints.Count - 1; i++)
            dynamicMarkers.Add(CreateLine(mapArea, pathPoints[i], pathPoints[i + 1]));

        for (int i = 0; i < route.Count; i++)
        {
            var m = route[i];
            bool active = IsActiveMarker(m.id, activeId);
            Color color = m.isShelterHint ? MarkerShelter
                : m.isDelivery ? MarkerDelivery
                : active ? MarkerActive
                : MarkerIdle;

            float size = active || m.isDelivery ? 32f : 24f;
            var dot = CreateDot(mapArea, "Mk_" + m.id, color, size);
            var rt = dot.rectTransform;
            Vector2 mapPos = WorldToMap(m.worldPos);
            rt.anchoredPosition = mapPos;
            dynamicMarkers.Add(dot.gameObject);

            // Xen kẽ nhãn trên/dưới để tránh chồng chữ
            float yOff = (i % 2 == 0) ? 50f : -50f;
            Color labelColor = active ? MarkerActive : Color.white;
            dynamicMarkers.Add(CreateMarkerLabel(mapArea, "Lb_" + m.id, m.label, mapPos + new Vector2(0f, yOff), labelColor));
        }
    }

    GameObject CreateMarkerLabel(Transform parent, string name, string text, Vector2 anchoredPos, Color textColor)
    {
        const int fontSize = 36;
        CrispUiText.WarmAtlas(fontSize);

        var root = new GameObject(name, typeof(RectTransform));
        root.transform.SetParent(parent, false);
        var rootRt = root.GetComponent<RectTransform>();
        rootRt.anchorMin = new Vector2(0.5f, 0.5f);
        rootRt.anchorMax = new Vector2(0.5f, 0.5f);
        rootRt.pivot = new Vector2(0.5f, 0.5f);
        rootRt.anchoredPosition = anchoredPos;
        rootRt.sizeDelta = new Vector2(340f, 52f);

        var bg = new GameObject("Bg", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(root.transform, false);
        var bgRt = bg.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;
        bg.GetComponent<Image>().color = new Color(0.02f, 0.02f, 0.03f, 0.92f);

        var label = CreateText(root.transform, "Text", fontSize,
            Vector2.zero, new Vector2(0.5f, 0.5f), new Vector2(320f, 48f),
            textColor, FontStyle.Bold);
        label.alignment = TextAnchor.MiddleCenter;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Overflow;
        label.text = text;
        var lrt = label.rectTransform;
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.offsetMin = new Vector2(8f, 4f);
        lrt.offsetMax = new Vector2(-8f, -4f);

        return root;
    }

    static bool IsActiveMarker(string markerId, string activeQuestId)
    {
        if (string.IsNullOrEmpty(activeQuestId)) return false;
        if (markerId == activeQuestId) return true;

        // Bắt buộc xem bản đồ sau nhận thư — tô điểm đến & cụ già
        if (activeQuestId == "check_map" &&
            (markerId == "ask_elder" || markerId == "deliver_mail" ||
             markerId == "landmark_banyan" || markerId == "keep_letter_dry" ||
             markerId == "shelter_rain" || markerId == "deliver_mother"))
            return true;

        if (activeQuestId == "find_shortcut" &&
            (markerId == "landmark_banyan" || markerId == "landmark_well"))
            return true;

        if (activeQuestId == "keep_letter_dry" &&
            (markerId == "keep_letter_dry" || markerId == "shelter_rain"))
            return true;

        // Chapter 3 clues share find_clues step
        if (activeQuestId == "find_clues" &&
            (markerId == "house" || markerId == "bunker" || markerId == "fort"))
            return true;

        if (activeQuestId == "read_brother_letter" && markerId == "fort")
            return true;

        return false;
    }

    void RefreshPlayerDot()
    {
        if (playerDotRt == null || mapArea == null) return;

        var player = GameManager.Instance != null ? GameManager.Instance.player : null;
        if (player == null)
        {
            var tpc = FindFirstObjectByType<ThirdPersonController>();
            if (tpc != null) player = tpc.transform;
        }

        if (player == null)
        {
            playerDot.gameObject.SetActive(false);
            return;
        }

        playerDot.gameObject.SetActive(true);
        playerDotRt.anchoredPosition = WorldToMap(player.position);
        playerDotRt.SetAsLastSibling();
    }

    Vector2 WorldToMap(Vector3 world)
    {
        float w = mapArea.sizeDelta.x;
        float h = mapArea.sizeDelta.y;
        float nx = Mathf.InverseLerp(mapMinXZ.x, mapMaxXZ.x, world.x);
        float nz = Mathf.InverseLerp(mapMinXZ.y, mapMaxXZ.y, world.z);
        // Centered in map area
        return new Vector2((nx - 0.5f) * w * 0.88f, (nz - 0.5f) * h * 0.88f);
    }

    GameObject CreateLine(RectTransform parent, Vector2 a, Vector2 b)
    {
        var go = new GameObject("RouteLine", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        var mid = (a + b) * 0.5f;
        var dir = b - a;
        float len = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(Mathf.Max(len, 4f), 4f);
        rt.anchoredPosition = mid;
        rt.localRotation = Quaternion.Euler(0f, 0f, angle);
        go.GetComponent<Image>().color = RouteLine;
        go.transform.SetAsFirstSibling();
        return go;
    }

    Image CreateDot(Transform parent, string name, Color color, float size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(size, size);
        var img = go.GetComponent<Image>();
        img.color = color;
        return img;
    }

    Text CreateText(Transform parent, string name, int size, Vector2 anchoredPos, Vector2 pivot,
        Vector2 sizeDelta, Color color, FontStyle style)
    {
        CrispUiText.WarmAtlas(size);
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var t = go.GetComponent<Text>();
        t.font = CrispUiText.GetFont();
        t.fontSize = size;
        t.fontStyle = style;
        t.color = new Color(color.r, color.g, color.b, 1f);
        t.text = "";
        CrispUiText.ApplyReadableDefaults(t);
        var rt = t.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
        return t;
    }

    GameObject CreatePanel(string name, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 sizeDelta)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(transform, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        if (anchorMin == anchorMax)
        {
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = offsetMin;
            rt.sizeDelta = sizeDelta;
        }
        else
        {
            rt.offsetMin = offsetMin;
            rt.offsetMax = sizeDelta;
        }
        go.GetComponent<Image>().color = color;
        return go;
    }
}
