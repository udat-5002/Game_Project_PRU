using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterFlowController : MonoBehaviour
{
    public int chapterIndex = 1;
    public Vector3 playerSpawnPosition = new Vector3(20f, 3.09f, -43f);

    [Header("Chapter 1")]
    public Vector3 mailStationPosition = new Vector3(12f, 3f, -41f);
    public Vector3 elderNpcPosition = new Vector3(-2f, 3f, -38f);
    public Vector3 obstacleZonePosition = new Vector3(-10f, 3f, -34f);
    public Vector3 deliveryNpcPosition = new Vector3(-18f, 3f, -28f);

    [Header("Chapter 2")]
    public Vector3 soldierNpcPosition = new Vector3(20f, 3f, -35f);
    public Vector3 stealthPathEnd = new Vector3(30f, 3f, -20f);
    public Vector3 rainShelterPosition = new Vector3(35f, 3f, -15f);
    public Vector3 motherNpcPosition = new Vector3(40f, 3f, -10f);
    public Vector3 patrolStart = new Vector3(22f, 3f, -28f);
    public Vector3 patrolEnd = new Vector3(28f, 3f, -22f);

    [Header("Chapter 3")]
    public Vector3 clue1Position = new Vector3(15f, 3f, -20f);
    public Vector3 clue2Position = new Vector3(18f, 3f, -15f);
    public Vector3 clue3Position = new Vector3(22f, 3f, -10f);
    public Vector3 finalDeliveryPosition = new Vector3(25f, 3f, -5f);

    GameObject chapterRoot;
    PatrolRelocator patrolRelocator;
    readonly HashSet<string> collectedClueIds = new HashSet<string>();
    readonly HashSet<string> visitedShortcutIds = new HashSet<string>();
    static readonly string[] ShortcutOrder = { "landmark_banyan", "landmark_well" };

    public static ChapterFlowController Active { get; private set; }

    void OnEnable() => Active = this;

    void OnDisable()
    {
        if (Active == this) Active = null;
    }

    public bool HasCollectedClue(string clueId) =>
        !string.IsNullOrEmpty(clueId) && collectedClueIds.Contains(clueId);

    void Start()
    {
        EnsureGlobalSystems();
        GameManager.Instance.SetCurrentChapter(chapterIndex);
        chapterRoot = new GameObject($"Chapter{chapterIndex}_Content");
        StartCoroutine(InitChapterWhenGroundReady());
    }

    IEnumerator InitChapterWhenGroundReady()
    {
        ApplyZonePositions();

        for (int i = 0; i < 90; i++)
        {
            Physics.SyncTransforms();
            if (GroundSnap.TryGetGroundY(playerSpawnPosition, out _))
                break;
            yield return null;
        }

        QuestWaypointRegistry.Clear();
        GameSession.ResetChapterGuidance();
        GameManager.Instance.TeleportPlayer(playerSpawnPosition);
        yield return new WaitForSeconds(0.25f);
        GameManager.Instance.TeleportPlayer(playerSpawnPosition);

        MapCollisionCleanup.ForceRun();
        yield return null;
        MapCollisionCleanup.ForceRun();

        if (FindFirstObjectByType<QuestNavigator>() == null)
            chapterRoot.AddComponent<QuestNavigator>();

        AddChapterPressureTimer();

        while (SceneTransition.Instance != null && SceneTransition.Instance.IsShowingChapterTitle)
            yield return null;

        PreloadVoicesForChapter(chapterIndex);
        DialogueVoicePlayer.Instance?.ClearCache();

        SetupChapterIntro();

        switch (chapterIndex)
        {
            case 1: SetupChapter1(); break;
            case 2: SetupChapter2(); break;
            case 3: SetupChapter3(); break;
        }

        PropGroundSnap.SnapLogsInScene();
        SetupWeather();
    }

    void ApplyZonePositions()
    {
        switch (chapterIndex)
        {
            case 1:
                playerSpawnPosition = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch1Spawn);
                mailStationPosition = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch1MailStation);
                elderNpcPosition = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch1Elder);
                obstacleZonePosition = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch1Obstacle);
                deliveryNpcPosition = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch1Delivery);
                break;
            case 2:
                playerSpawnPosition = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch2Spawn);
                soldierNpcPosition = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch2Soldier);
                patrolStart = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch2PatrolStart);
                patrolEnd = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch2PatrolEnd);
                stealthPathEnd = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch2StealthEnd);
                rainShelterPosition = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch2RainShelter);
                motherNpcPosition = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch2Mother);
                break;
            case 3:
                playerSpawnPosition = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch3Spawn);
                clue1Position = ForestZoneLayout.ResolveHouseCluePosition();
                clue2Position = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch3Clue2);
                clue3Position = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch3Clue3);
                finalDeliveryPosition = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch3FinalDelivery);
                break;
        }
    }

    void SetupWeather()
    {
        switch (chapterIndex)
        {
            case 1:
                WeatherController.Create(WeatherController.WeatherPreset.Overcast);
                break;
            case 2:
                WeatherController.Create(WeatherController.WeatherPreset.DarkForest);
                WeatherController.Instance?.EnableChapterRain(instant: true);
                break;
            case 3:
                WeatherController.Create(WeatherController.WeatherPreset.Battlefield);
                WeatherController.Instance?.EnableChapterRain(instant: true);
                break;
        }
    }

    void EnsureGlobalSystems()
    {
        if (GameManager.Instance == null)
            new GameObject("GameManager").AddComponent<GameManager>();
        if (SceneTransition.Instance == null)
            new GameObject("SceneTransition").AddComponent<SceneTransition>();
        if (GameUI.Instance == null)
            new GameObject("GameUI").AddComponent<GameUI>();
        if (DialogueManager.Instance == null)
            new GameObject("DialogueManager").AddComponent<DialogueManager>();

        var player = FindFirstObjectByType<ThirdPersonController>();
        if (player != null)
        {
            CharacterMaterialFixer.ApplyTo(player.transform);
            if (player.GetComponent<MailInventory>() == null)
                player.gameObject.AddComponent<MailInventory>();
            if (player.GetComponent<PlayerInteraction>() == null)
                player.gameObject.AddComponent<PlayerInteraction>();
            if (player.GetComponent<PlayerAppearancePreserver>() == null)
                player.gameObject.AddComponent<PlayerAppearancePreserver>();
        }

        if (FindFirstObjectByType<QuestManager>() == null)
            new GameObject("QuestManager").AddComponent<QuestManager>();
    }

    void ApplyStealthToVegetation()
    {
        var allRenderers = FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        foreach (var r in allRenderers)
        {
            string name = r.gameObject.name.ToLower();
            if (name.Contains("fern") || name.Contains("agave") || name.Contains("bush"))
            {
                if (r.GetComponentInParent<HideSpot>() == null && r.gameObject.GetComponent<HideSpot>() == null)
                {
                    var col = r.gameObject.AddComponent<SphereCollider>();
                    col.isTrigger = true;
                    col.radius = 1.2f;
                    r.gameObject.AddComponent<HideSpot>();
                }
            }
        }
    }

    void SetupChapterIntro()
    {
        string title = chapterIndex switch
        {
            1 => "Chương 1: Nhận Thư, Mở Bản Đồ",
            2 => "Chương 2: Thư Người Lính",
            3 => "Chương 3: Lá Thư Cuối Cùng",
            _ => ""
        };

        GameUI.Instance?.SetChapterLabel(title);

        var zone = ForestZoneLayout.GetZone(chapterIndex);
        GameUI.Instance?.SetZoneLabel(zone.shortName);

        if (chapterIndex == 1)
        {
            ShowIntroWithVoice(Chapter1Dialogue.IntroHudBody, Chapter1Voice.IntroHud);
        }
        else if (chapterIndex == 2)
        {
            ShowIntroWithVoice(Chapter2Dialogue.IntroHudBody, Chapter2Voice.IntroHud);
        }
        else if (chapterIndex == 3)
        {
            ShowIntroWithVoice(Chapter3Dialogue.IntroHudBody, Chapter3Voice.IntroHud);
            GameUI.Instance?.ShowNotification(Chapter3Dialogue.CheckMapPrompt, 5f, CrispUiText.Gold);
        }
    }

    void ShowIntroWithVoice(string body, string voiceKey)
    {
        var dm = DialogueManager.Instance;
        if (dm == null) return;

        DialogueAudio.Load(voiceKey, refreshFromDisk: true);
        DialogueVoicePlayer.Instance?.ClearCache();

        var options = dm.DefaultOptions;
        options.voiceKey = voiceKey;
        options.typewriter = false;
        dm.ShowDialogue("", body, null, options);
    }

    void SetupChapter1()
    {
        var qm = QuestManager.Instance;
        qm.OnAllQuestsCompleted -= OnChapter1Complete;
        qm.SetupQuests(
            ("pickup_mail", "Nhận thư tại Trạm Liên Lạc"),
            ("check_map", "Mở bản đồ (Tab) xem đường tới Bà Lan"),
            ("ask_elder", "Hỏi cụ già đường vào làng"),
            ("cross_obstacle", "Vượt qua khu gỗ đổ chặn đường"),
            ("find_shortcut", "Tìm lối tắt theo lời cụ (đi đúng mốc)"),
            ("deliver_mail", "Giao thư đúng người: Bà Lan")
        );
        qm.OnAllQuestsCompleted += OnChapter1Complete;

        CreateMailStation();
        CreateElderNpc();
        CreateObstacleSection();
        CreateShortcutPath();
        CreateDeliveryNpc("Bà Lan", deliveryNpcPosition);
    }

    void CreateElderNpc()
    {
        var go = CreateNpcMarker("Cụ già", elderNpcPosition, new Color(0.55f, 0.45f, 0.3f), NpcVisualFactory.NpcRole.Civilian, "NhanVat/BaLao/BaLao", false, false);
        var model = go.transform.Find("NpcModel");
        if (model != null)
        {
            model.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            NpcVisualFactory.AutoAdjustGroundPlacement(model);
        }
        AttachNpcHeadLabel(go, "Cụ già");
        RegisterWaypoint("ask_elder", go.transform.position);
        var elder = go.AddComponent<ElderGuideInteractable>();
        elder.promptText = "Nhấn E - Hỏi đường";
    }

    void CreateObstacleSection()
    {
        var pos = obstacleZonePosition;
        Transform mapWood = null;
        if (TryFindMapWoodNear(pos, 22f, out mapWood, out var woodCenter))
            pos = woodCenter;

        var go = CreateMarker("Khu gỗ đổ", pos, new Color(0.25f, 0.65f, 0.3f), new Vector3(2f, 2f, 2f));

        var worldLabel = go.transform.Find("ObjectiveLabel");
        if (worldLabel != null)
            Destroy(worldLabel.gameObject);

        var tag = go.GetComponent<NpcHeadLabel>() ?? go.AddComponent<NpcHeadLabel>();
        tag.Configure("Khu gỗ đổ", 3.6f);

        SpawnFallenLogs(go.transform, mapWood);

        RegisterWaypoint("cross_obstacle", go.transform.position);
        var obstacle = go.AddComponent<ObstacleCrossInteractable>();
        obstacle.promptText = "Nhấn E - Vượt qua";
    }

    void SpawnFallenLogs(Transform parent, Transform mapWoodPrototype = null)
    {
        var prefab = Resources.Load<GameObject>("FallenLogs");
        if (prefab != null)
        {
            var logs = Object.Instantiate(prefab, parent);
            logs.transform.localPosition = Vector3.zero;
            PropGroundSnap.SnapLogsInScene();
            return;
        }

        if (mapWoodPrototype != null && SpawnFallenWoodPile(parent, mapWoodPrototype))
        {
            PropGroundSnap.SnapLogsInScene();
            return;
        }

        var cracked = Resources.Load<GameObject>("CrackedTree");
        if (cracked != null && SpawnCrackedTreePile(parent, cracked))
        {
            PropGroundSnap.SnapLogsInScene();
            return;
        }

        const float logScale = 12f;
        var pile = new (string resource, Vector3 offset, Vector3 euler)[]
        {
            ("WoodLogs/SM_AFS_Log14_LowEndPC", new Vector3(-2.5f, 0f, 0f), new Vector3(0f, 20f, 90f)),
            ("WoodLogs/SM_AFS_Log06_LowEndPC", new Vector3(0f, 0f, 0f), new Vector3(0f, -10f, 90f)),
            ("WoodLogs/SM_AFS_Log04_LowEndPC", new Vector3(2.5f, 0f, 0.3f), new Vector3(5f, 35f, 88f)),
        };

        foreach (var entry in pile)
        {
            var logPrefab = Resources.Load<GameObject>(entry.resource);
            if (logPrefab == null) continue;

            var log = Object.Instantiate(logPrefab, parent);
            log.name = "FallenLog_" + log.name;
            log.transform.localPosition = entry.offset;
            log.transform.localRotation = Quaternion.Euler(entry.euler);
            log.transform.localScale = Vector3.one * logScale;
        }

        PropGroundSnap.SnapLogsInScene();
    }

    static bool TryFindMapWoodNear(Vector3 point, float radius, out Transform prototype, out Vector3 center)
    {
        prototype = null;
        center = point;

        float bestScore = float.MaxValue;
        foreach (var rootName in new[] { "Map", "Environment_Decor" })
        {
            var root = GameObject.Find(rootName)?.transform;
            if (root == null) continue;

            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                if (!IsMapWoodCandidate(t)) continue;

                var renderer = t.GetComponent<Renderer>() ?? t.GetComponentInChildren<Renderer>();
                if (renderer == null) continue;

                var c = renderer.bounds.center;
                float dist = HorizontalDistance(point, c);
                if (dist > radius) continue;

                float score = dist;
                if (IsMapLogLike(renderer.bounds)) score -= 3f;
                if (t.name.IndexOf("NewTree", System.StringComparison.OrdinalIgnoreCase) >= 0) score -= 1f;

                if (score >= bestScore) continue;

                bestScore = score;
                prototype = t;
                center = new Vector3(c.x, 0f, c.z);
            }
        }

        return prototype != null;
    }

    static bool IsMapWoodCandidate(Transform t)
    {
        var name = t.name;
        if (name.IndexOf("NewTree", System.StringComparison.OrdinalIgnoreCase) >= 0) return true;
        if (name.IndexOf("Log", System.StringComparison.OrdinalIgnoreCase) >= 0) return true;
        if (name.IndexOf("leaves", System.StringComparison.OrdinalIgnoreCase) >= 0) return false;

        var renderer = t.GetComponent<Renderer>();
        return renderer != null && IsMapLogLike(renderer.bounds);
    }

    static bool IsMapLogLike(Bounds b)
    {
        var s = b.size;
        float height = s.y;
        float longAxis = Mathf.Max(s.x, s.z);
        float shortAxis = Mathf.Min(s.x, s.z);

        if (height > 2.2f || longAxis < 0.6f) return false;
        if (height > 1.1f && longAxis / Mathf.Max(shortAxis, 0.01f) < 1.6f) return false;
        return longAxis >= 0.9f && shortAxis <= 1.4f;
    }

    static bool SpawnFallenWoodPile(Transform parent, Transform prototype)
    {
        if (prototype == null) return false;

        bool isTree = prototype.name.IndexOf("NewTree", System.StringComparison.OrdinalIgnoreCase) >= 0;
        var scale = prototype.lossyScale;
        var offsets = new[]
        {
            Vector3.zero,
            new Vector3(-2.8f, 0f, 0.45f),
            new Vector3(2.6f, 0f, -0.35f),
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            var copy = Object.Instantiate(prototype.gameObject, parent);
            copy.name = $"FallenLog_{i + 1}";

            var rot = isTree
                ? Quaternion.Euler(0f, prototype.eulerAngles.y + i * 28f, 90f)
                : Quaternion.Euler(prototype.eulerAngles.x, prototype.eulerAngles.y + i * 18f, prototype.eulerAngles.z);

            copy.transform.SetPositionAndRotation(parent.position + offsets[i], rot);
            copy.transform.localScale = scale;
            copy.transform.SetParent(parent, true);
            StripColliders(copy);
        }

        return true;
    }

    static bool SpawnCrackedTreePile(Transform parent, GameObject crackedPrefab)
    {
        var offsets = new[]
        {
            Vector3.zero,
            new Vector3(-2.4f, 0f, 0.35f),
            new Vector3(2.2f, 0f, -0.25f),
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            var copy = Object.Instantiate(crackedPrefab, parent);
            copy.name = $"FallenLog_{i + 1}";
            copy.transform.localPosition = offsets[i];
            copy.transform.localRotation = Quaternion.Euler(0f, i * 35f, 90f);
            copy.transform.localScale = Vector3.one * 1.2f;
            StripColliders(copy);
        }

        return true;
    }

    static void StripColliders(GameObject go)
    {
        foreach (var col in go.GetComponentsInChildren<Collider>())
            Object.Destroy(col);
    }

    static float HorizontalDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    void SetupChapter2()
    {
        var qm = QuestManager.Instance;
        qm.OnAllQuestsCompleted -= OnChapter2Complete;
        qm.SetupQuests(
            ("receive_letter", "Nhận thư mới từ người lính trẻ"),
            ("check_map", "Mở bản đồ (Tab) xem đường tới mẹ người lính"),
            ("keep_letter_dry", "Trú mưa giữ thư khô, đi đúng tuyến"),
            ("deliver_mother", "Giao thư cho mẹ người lính")
        );
        qm.OnAllQuestsCompleted += OnChapter2Complete;

        CreateSoldierNpc();
        CreateRainShelterPath();
        CreateRainAtmosphere();
        CreateMotherNpc();
    }

    void SetupChapter3()
    {
        var qm = QuestManager.Instance;
        qm.OnAllQuestsCompleted -= OnChapter3Complete;
        qm.SetupQuests(
            ("check_map", "Mở bản đồ (Tab) ghép tuyến đường cũ"),
            ("find_clues", "Tìm manh mối theo tuyến trên bản đồ"),
            ("read_brother_letter", "Đọc thư anh trai"),
            ("final_delivery", "Đi đúng tuyến, giao nốt thư thất lạc")
        );
        qm.OnAllQuestsCompleted += OnChapter3Complete;

        CreateCluePoints();
        CreateChapter3Rain();
        CreateFinalDeliveryNpc();
    }

    void CreateMailStation()
    {
        var go = CreateMarker("Trạm Liên Lạc", mailStationPosition, new Color(1f, 0.82f, 0.15f), new Vector3(3f, 3f, 3f));
        CreateMailBagProp(go.transform);
        RegisterWaypoint("pickup_mail", go.transform.position);
        var pickup = go.AddComponent<MailPickupInteractable>();
        pickup.promptText = "Nhấn E - Lấy túi thư";
    }

    void CreateMailBagProp(Transform parent)
    {
        var prefab = Resources.Load<GameObject>("TramLienLac/TramLienLac");
        if (prefab != null)
        {
            var model = Object.Instantiate(prefab, parent);
            model.transform.localPosition = new Vector3(0f, 0.274f, 0f);
            model.transform.localScale = Vector3.one;
            return;
        }

        var bag = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bag.name = "MailBag";
        bag.transform.SetParent(parent);
        bag.transform.localPosition = new Vector3(0f, 0.55f, 0f);
        bag.transform.localScale = new Vector3(0.75f, 0.5f, 0.55f);
        if (bag.GetComponent<Collider>() != null) Destroy(bag.GetComponent<Collider>());
        var bagR = bag.GetComponent<Renderer>();
        if (bagR != null) bagR.material = CreateURPMaterial(new Color(0.55f, 0.38f, 0.12f), 1f);

        var post = GameObject.CreatePrimitive(PrimitiveType.Cube);
        post.name = "SignPost";
        post.transform.SetParent(parent);
        post.transform.localPosition = new Vector3(0.9f, 0.9f, 0f);
        post.transform.localScale = new Vector3(0.12f, 1.8f, 0.12f);
        if (post.GetComponent<Collider>() != null) Destroy(post.GetComponent<Collider>());
        var postR = post.GetComponent<Renderer>();
        if (postR != null) postR.material = CreateURPMaterial(new Color(0.35f, 0.22f, 0.1f), 1f);

        var board = GameObject.CreatePrimitive(PrimitiveType.Cube);
        board.name = "SignBoard";
        board.transform.SetParent(parent);
        board.transform.localPosition = new Vector3(0.9f, 1.65f, 0f);
        board.transform.localScale = new Vector3(0.05f, 0.55f, 0.9f);
        if (board.GetComponent<Collider>() != null) Destroy(board.GetComponent<Collider>());
        var boardR = board.GetComponent<Renderer>();
        if (boardR != null) boardR.material = CreateURPMaterial(new Color(0.15f, 0.45f, 0.2f), 1f);
    }

    void CreateDeliveryNpc(string name, Vector3 pos)
    {
        var go = CreateNpcMarker(name, pos, Color.yellow, NpcVisualFactory.NpcRole.Civilian, "NhanVat/BaLan/BaLan", false);
        var model = go.transform.Find("NpcModel");
        if (model != null)
        {
            model.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            NpcVisualFactory.AutoAdjustGroundPlacement(model);
        }
        RegisterWaypoint("deliver_mail", go.transform.position);
        var delivery = go.AddComponent<MailDeliveryInteractable>();
        delivery.recipientName = "Bà Lan - Làng Bình An";
        delivery.promptText = "Nhấn E - Giao thư";
        AttachNpcHeadLabel(go, "Bà Lan");
    }

    void CreateSoldierNpc()
    {
        var go = CreateNpcMarker("Người lính trẻ", soldierNpcPosition, Color.blue, NpcVisualFactory.NpcRole.Soldier);
        RegisterWaypoint("receive_letter", go.transform.position);
        go.AddComponent<SoldierLetterInteractable>();
    }

    void CreateShortcutPath()
    {
        visitedShortcutIds.Clear();

        CreateShortcutLandmark(
            "landmark_banyan",
            ForestZoneLayout.Ch1LandmarkBanyan,
            "Ngã ba cây đa",
            Chapter1Dialogue.LandmarkBanyan,
            new Color(0.35f, 0.75f, 0.4f));

        CreateShortcutLandmark(
            "landmark_well",
            ForestZoneLayout.Ch1LandmarkWell,
            "Giếng hoang",
            Chapter1Dialogue.LandmarkWell,
            new Color(0.4f, 0.7f, 0.85f));

        RefreshShortcutWaypoint();
    }

    void CreateShortcutLandmark(string id, Vector3 pos, string title, string confirmLine, Color accent)
    {
        var go = CreateMarker(title, pos, accent, new Vector3(2.8f, 2.8f, 2.8f));
        var worldLabel = go.transform.Find("ObjectiveLabel");
        if (worldLabel != null)
            Destroy(worldLabel.gameObject);

        if (id == "landmark_banyan")
            SpawnBanyanCrossroadsVisual(go.transform);
        else if (id == "landmark_well")
            SpawnAbandonedWellVisual(go.transform);

        float labelHeight = id == "landmark_banyan" ? 5.5f : 3.4f;
        var tag = go.GetComponent<NpcHeadLabel>() ?? go.AddComponent<NpcHeadLabel>();
        tag.Configure(title, labelHeight);

        var landmark = go.AddComponent<ShortcutLandmarkInteractable>();
        landmark.landmarkId = id;
        landmark.landmarkTitle = title;
        landmark.confirmLine = confirmLine;
        landmark.promptText = "Nhấn E - Nhìn mốc đường";
    }

    void SpawnBanyanCrossroadsVisual(Transform parent)
    {
        // Cây đứng rõ ràng — không dùng khúc gỗ nằm (tránh lẫn với khu gỗ đổ).
        var cracked = Resources.Load<GameObject>("CrackedTree");
        if (cracked != null)
        {
            var main = Object.Instantiate(cracked, parent);
            main.name = "BanyanTree_Main";
            main.transform.localPosition = Vector3.zero;
            main.transform.localRotation = Quaternion.identity;
            main.transform.localScale = Vector3.one * 3.6f;
            StripColliders(main);

            var side = Object.Instantiate(cracked, parent);
            side.name = "BanyanTree_Side";
            side.transform.localPosition = new Vector3(-2.6f, 0f, 1.8f);
            side.transform.localRotation = Quaternion.Euler(0f, 35f, 0f);
            side.transform.localScale = Vector3.one * 2.4f;
            StripColliders(side);
        }

        if (TryFindMapTreeNear(parent.position, 80f, out var treeProto))
            SpawnUprightTreeClone(parent, treeProto, new Vector3(2.8f, 0f, -1.6f), 1.1f, -40f);

        SpawnLandmarkBush(parent, new Vector3(1.2f, 0f, 1.1f), 3.4f);
        SpawnLandmarkBush(parent, new Vector3(-1.4f, 0f, -0.8f), 2.8f);
        SpawnLandmarkBush(parent, new Vector3(0.2f, 0f, -2.2f), 2.5f);
    }

    void SpawnAbandonedWellVisual(Transform parent)
    {
        // Giếng đá hoang — vòng thành + miệng giếng tối
        var rim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rim.name = "WellRim";
        rim.transform.SetParent(parent, false);
        rim.transform.localPosition = new Vector3(0f, 0.45f, 0f);
        rim.transform.localScale = new Vector3(2.2f, 0.45f, 2.2f);
        StripColliders(rim);
        var rimR = rim.GetComponent<Renderer>();
        if (rimR != null) rimR.material = CreateURPMaterial(new Color(0.42f, 0.4f, 0.36f), 1f);

        var inner = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        inner.name = "WellHole";
        inner.transform.SetParent(parent, false);
        inner.transform.localPosition = new Vector3(0f, 0.55f, 0f);
        inner.transform.localScale = new Vector3(1.45f, 0.2f, 1.45f);
        StripColliders(inner);
        var innerR = inner.GetComponent<Renderer>();
        if (innerR != null) innerR.material = CreateURPMaterial(new Color(0.08f, 0.1f, 0.12f), 1f);

        var water = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        water.name = "WellWater";
        water.transform.SetParent(parent, false);
        water.transform.localPosition = new Vector3(0f, 0.35f, 0f);
        water.transform.localScale = new Vector3(1.35f, 0.05f, 1.35f);
        StripColliders(water);
        var waterR = water.GetComponent<Renderer>();
        if (waterR != null) waterR.material = CreateURPMaterial(new Color(0.15f, 0.28f, 0.32f), 0.85f);

        // Cây / bụi xung quanh cho cảm giác bỏ hoang trong rừng
        var cracked = Resources.Load<GameObject>("CrackedTree");
        if (cracked != null)
        {
            var tree = Object.Instantiate(cracked, parent);
            tree.name = "WellSideTree";
            tree.transform.localPosition = new Vector3(2.4f, 0f, -1.2f);
            tree.transform.localRotation = Quaternion.Euler(0f, 25f, 8f);
            tree.transform.localScale = Vector3.one * 1.8f;
            StripColliders(tree);
        }

        SpawnLandmarkBush(parent, new Vector3(-1.6f, 0f, 1.3f), 2.8f);
        SpawnLandmarkBush(parent, new Vector3(1.8f, 0f, 1.5f), 2.4f);
    }

    void SpawnLandmarkBush(Transform parent, Vector3 localPos, float scale)
    {
        var bushPrefab = Resources.Load<GameObject>("WildGrass/Bush");
        if (bushPrefab == null) return;

        var bush = Object.Instantiate(bushPrefab, parent);
        bush.name = "LandmarkBush";
        bush.transform.localPosition = localPos;
        bush.transform.localScale = Vector3.one * scale;
        var bushMat = Resources.Load<Material>("WildGrass/BushMat");
        if (bushMat != null)
        {
            foreach (var r in bush.GetComponentsInChildren<Renderer>())
                r.material = bushMat;
        }
    }

    static void SpawnUprightTreeClone(Transform parent, Transform prototype, Vector3 localOffset, float scaleMul, float yaw)
    {
        if (prototype == null) return;

        var copy = Object.Instantiate(prototype.gameObject, parent);
        copy.name = "BanyanTree_Map";
        // Ép cây đứng thẳng theo trục Y thế giới — tránh dính rotation nằm của prefab map.
        copy.transform.SetPositionAndRotation(
            parent.TransformPoint(localOffset),
            Quaternion.Euler(0f, yaw, 0f));
        copy.transform.SetParent(parent, true);
        copy.transform.localScale = Vector3.one * Mathf.Max(1.2f, prototype.lossyScale.y * scaleMul);
        StripColliders(copy);
    }

    static bool TryFindMapTreeNear(Vector3 point, float radius, out Transform prototype)
    {
        prototype = null;
        float bestDist = float.MaxValue;

        foreach (var rootName in new[] { "Map", "Environment_Decor" })
        {
            var root = GameObject.Find(rootName)?.transform;
            if (root == null) continue;

            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                if (t.name.IndexOf("NewTree", System.StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                var renderer = t.GetComponent<Renderer>() ?? t.GetComponentInChildren<Renderer>();
                if (renderer == null) continue;

                // Ưu tiên cây đứng (cao), không lấy khúc gỗ nằm
                if (renderer.bounds.size.y < 2.5f) continue;

                float dist = HorizontalDistance(point, renderer.bounds.center);
                if (dist > radius || dist >= bestDist) continue;

                bestDist = dist;
                prototype = t;
            }
        }

        return prototype != null;
    }

    public bool IsNextShortcutLandmark(string landmarkId)
    {
        if (string.IsNullOrEmpty(landmarkId) || visitedShortcutIds.Contains(landmarkId))
            return false;

        foreach (var id in ShortcutOrder)
        {
            if (visitedShortcutIds.Contains(id)) continue;
            return id == landmarkId;
        }

        return false;
    }

    public bool TryVisitShortcutLandmark(string landmarkId)
    {
        if (QuestManager.Instance == null || !QuestManager.Instance.IsStepActive("find_shortcut"))
            return false;
        if (!IsNextShortcutLandmark(landmarkId))
            return false;

        visitedShortcutIds.Add(landmarkId);

        bool done = true;
        foreach (var id in ShortcutOrder)
        {
            if (!visitedShortcutIds.Contains(id))
            {
                done = false;
                break;
            }
        }

        if (done)
        {
            QuestManager.Instance.CompleteStep("find_shortcut");
            GameUI.Instance?.ShowNotification("Đã hết lối tắt — tới nhà Bà Lan giao thư.", 4f, CrispUiText.Gold);
            return true;
        }

        GameUI.Instance?.ShowNotification("Đúng mốc rồi. Đi tiếp theo lối tắt.", 3f);
        RefreshShortcutWaypoint();
        return true;
    }

    void RefreshShortcutWaypoint()
    {
        if (!visitedShortcutIds.Contains("landmark_banyan"))
            RegisterWaypoint("find_shortcut", ForestZoneLayout.Ch1LandmarkBanyan);
        else
            RegisterWaypoint("find_shortcut", ForestZoneLayout.Ch1LandmarkWell);
    }

    void CreateRainShelterPath()
    {
        CreateRainShelterPoint(ForestZoneLayout.Ch2RainShelter, "Chỗ trú mưa");

        var end = GroundSnap.Snap(ForestZoneLayout.Ch2RainPathEnd);
        CreateZone("RainPathEnd_keep_letter_dry", end, new Vector3(5f, 3f, 5f), "keep_letter_dry", Color.clear);
        RegisterWaypoint("keep_letter_dry", end);

        var tracker = chapterRoot.AddComponent<LetterDrynessTracker>();
        tracker.activeQuestId = "keep_letter_dry";
        // Đi vòng map xa — cho đủ thời gian chạy giữa các điểm
        tracker.wetSecondsToFail = 45f;
        tracker.fallbackResetPosition = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch2Soldier);
        RainShelter.LastShelterPosition = GroundSnap.SnapCharacter(ForestZoneLayout.Ch2Soldier);
    }

    void CreateRainShelterPoint(Vector3 pos, string label)
    {
        var root = new GameObject("RainHut_Shelter");
        root.transform.SetParent(chapterRoot.transform);
        root.transform.position = GroundSnap.Snap(pos);
        // Mặt trước mở hướng về phía người lính (tây) để đi vào núp.
        root.transform.rotation = Quaternion.Euler(0f, -55f, 0f);

        // Trigger chỉ trong lòng chòi — phải bước vào mới trú được.
        var triggerGo = new GameObject("ShelterInterior");
        triggerGo.transform.SetParent(root.transform, false);
        triggerGo.transform.localPosition = new Vector3(0f, 1.4f, -0.2f);
        var trigger = triggerGo.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(3.6f, 2.8f, 3.4f);

        var shelter = triggerGo.AddComponent<RainShelter>();
        shelter.enterMessage = "Đã vào chòi — đang núp mưa, thư khô.";

        var tag = root.GetComponent<NpcHeadLabel>() ?? root.AddComponent<NpcHeadLabel>();
        tag.Configure("Chòi trú mưa", 4.2f);

        SpawnEnterableRainHut(root.transform);

        var lightGo = new GameObject("ShelterLight");
        lightGo.transform.SetParent(root.transform, false);
        lightGo.transform.localPosition = new Vector3(0f, 2.6f, -0.3f);
        var beacon = lightGo.AddComponent<Light>();
        beacon.type = LightType.Point;
        beacon.color = new Color(1f, 0.82f, 0.45f);
        beacon.intensity = 5.5f;
        beacon.range = 9f;
    }

    /// <summary>
    /// Chòi gỗ mở cửa trước — có tường chắn, đi vào trong mới núp mưa.
    /// </summary>
    void SpawnEnterableRainHut(Transform parent)
    {
        var wood = new Color(0.42f, 0.28f, 0.14f);
        var darkWood = new Color(0.28f, 0.18f, 0.1f);
        var thatch = new Color(0.55f, 0.45f, 0.22f);

        // solid=true: giữ collider để đứng/núp trong chòi, không xuyên tường.
        void Part(string name, PrimitiveType type, Vector3 localPos, Vector3 localScale, Vector3 euler, Color color, bool solid)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localRotation = Quaternion.Euler(euler);
            go.transform.localScale = localScale;
            if (!solid)
                StripColliders(go);
            var r = go.GetComponent<Renderer>();
            if (r != null) r.material = CreateURPMaterial(color, 1f);
        }

        // Cột góc
        Part("PostFL", PrimitiveType.Cylinder, new Vector3(-2.3f, 1.55f, 2.1f), new Vector3(0.3f, 1.55f, 0.3f), Vector3.zero, wood, true);
        Part("PostFR", PrimitiveType.Cylinder, new Vector3(2.3f, 1.55f, 2.1f), new Vector3(0.3f, 1.55f, 0.3f), Vector3.zero, wood, true);
        Part("PostBL", PrimitiveType.Cylinder, new Vector3(-2.3f, 1.55f, -2.1f), new Vector3(0.3f, 1.55f, 0.3f), Vector3.zero, wood, true);
        Part("PostBR", PrimitiveType.Cylinder, new Vector3(2.3f, 1.55f, -2.1f), new Vector3(0.3f, 1.55f, 0.3f), Vector3.zero, wood, true);

        // Tường sau + hai bên — mặt +Z để trống làm cửa vào
        Part("WallBack", PrimitiveType.Cube, new Vector3(0f, 1.65f, -2.25f), new Vector3(5f, 3.2f, 0.18f), Vector3.zero, wood, true);
        Part("WallLeft", PrimitiveType.Cube, new Vector3(-2.45f, 1.65f, 0f), new Vector3(0.18f, 3.2f, 4.4f), Vector3.zero, wood, true);
        Part("WallRight", PrimitiveType.Cube, new Vector3(2.45f, 1.65f, 0f), new Vector3(0.18f, 3.2f, 4.4f), Vector3.zero, wood, true);

        // Mái che mưa
        Part("Roof", PrimitiveType.Cube, new Vector3(0f, 3.55f, 0f), new Vector3(5.6f, 0.18f, 5.2f), new Vector3(-12f, 0f, 0f), thatch, true);
        Part("RoofBeam", PrimitiveType.Cube, new Vector3(0f, 3.15f, 0f), new Vector3(5.2f, 0.16f, 0.22f), Vector3.zero, darkWood, false);

        // Ngưỡng cửa gợi lối vào (không chắn)
        Part("DoorSill", PrimitiveType.Cube, new Vector3(0f, 0.08f, 2.15f), new Vector3(2.4f, 0.12f, 0.35f), Vector3.zero, darkWood, false);

        SpawnLandmarkBush(parent, new Vector3(-3.4f, 0f, -2.6f), 3f);
        SpawnLandmarkBush(parent, new Vector3(3.5f, 0f, -2.4f), 2.8f);
    }

    void CreateStealthPath(int chapter, Vector3 endPos, string questId, Vector3 hidePos, Vector3 resetPos)
    {
        // Giữ API cũ — chương 1/2 không còn dùng tuần tra.
        var end = GroundSnap.Snap(endPos);
        CreateZone($"StealthEnd_{questId}", end, new Vector3(5f, 3f, 5f), questId, Color.clear);
        RegisterWaypoint(questId, end);
        CreateHideSpot(hidePos, questId);
    }

    void SpawnPatrols(int chapter, string questId, Vector3 resetPos)
    {
        if (chapter == 1 || chapter == 2 || chapter == 3)
            return;

        if (patrolRelocator == null)
            patrolRelocator = chapterRoot.GetComponent<PatrolRelocator>() ?? chapterRoot.AddComponent<PatrolRelocator>();

        int index = 0;
        foreach (var setup in ChapterDifficulty.GetPatrols(chapter, questId))
        {
            if (setup.activeQuestId != questId) continue;

            var patrol = new GameObject($"PatrolEnemy_{questId}_{index}");
            patrol.transform.SetParent(chapterRoot.transform);

            var enemy = patrol.AddComponent<StealthEnemy>();
            enemy.pointA = GroundSnap.SnapCharacter(setup.pointA);
            enemy.pointB = GroundSnap.SnapCharacter(setup.pointB);
            patrol.transform.position = enemy.pointA;

            Vector3 chapterStart = chapter switch
            {
                1 => ForestZoneLayout.Ch1Spawn,
                2 => ForestZoneLayout.Ch2Spawn,
                3 => ForestZoneLayout.Ch3Spawn,
                _ => resetPos
            };
            enemy.resetPosition = GroundSnap.SnapCharacter(chapterStart);
            enemy.moveSpeed = setup.moveSpeed;
            enemy.detectRadius = setup.detectRadius;
            enemy.detectSeconds = setup.detectSeconds;
            enemy.mustHideToPass = setup.mustHideToPass;
            enemy.activeQuestId = setup.activeQuestId;
            enemy.SetStagingIndex(index++);
            enemy.SetRoute(enemy.pointA, enemy.pointB);
            enemy.SetPatrolActive(false);

            var modelRoot = NpcVisualFactory.Attach(patrol.transform, NpcVisualFactory.NpcRole.Enemy, "NhanVat/LowPolySoldiers_demo/models/Soldier_demo");
            if (modelRoot != null)
            {
                modelRoot.localPosition = Vector3.zero;
                modelRoot.localScale = Vector3.one * 1.3f;
                PatrolVisibility.Apply(patrol.transform, enemy.detectRadius);
                var patrolAnim = patrol.AddComponent<NpcPatrolAnimator>();
                patrolAnim.modelRoot = modelRoot;
            }
            else
            {
                CreateObjectiveLabel(patrol.transform, "Lính tuần tra");
                PatrolVisibility.Apply(patrol.transform, enemy.detectRadius);
            }

            patrolRelocator.Register(enemy);
        }
    }

    void AttachNpcHeadLabel(GameObject npc, string label)
    {
        var tag = npc.GetComponent<NpcHeadLabel>() ?? npc.AddComponent<NpcHeadLabel>();
        tag.Configure(label, 3.2f);
    }

    void AttachClueHeadLabel(GameObject go, string label)
    {
        var tag = go.GetComponent<NpcHeadLabel>() ?? go.AddComponent<NpcHeadLabel>();
        tag.Configure(label, 2.8f);
    }

    void CreateHideSpot(Vector3 hidePos, string questId)
    {
        var hideGo = CreateInvisibleTrigger($"HideSpot_{questId}", GroundSnap.Snap(hidePos), new Vector3(3.5f, 2.5f, 3.5f));
        hideGo.AddComponent<HideSpot>();

        var bushPrefab = Resources.Load<GameObject>("WildGrass/Bush");
        if (bushPrefab != null)
        {
            var bush = Object.Instantiate(bushPrefab, hideGo.transform);
            bush.transform.localPosition = new Vector3(0, -0.1f, 0);
            bush.transform.localScale = Vector3.one * 5f;

            var bushMat = Resources.Load<Material>("WildGrass/BushMat");
            if (bushMat != null)
            {
                foreach (var r in bush.GetComponentsInChildren<Renderer>())
                {
                    r.material = bushMat;
                }
            }
        }
    }

    void AddChapterPressureTimer()
    {
        var timer = chapterRoot.AddComponent<ChapterPressureTimer>();
        timer.chapter = chapterIndex;
    }

    void CreateStealthSection()
    {
        // Giữ cho tương thích — dùng CreateStealthPath
    }

    void CreateRainAtmosphere()
    {
        var rain = chapterRoot.AddComponent<RainEvent>();
        rain.triggerDuringStepId = "keep_letter_dry";
        rain.rainFromChapterStart = true;
        rain.escalateToStorm = true;
        rain.showRainDialogue = false;
        rain.stormNotification = "Mưa lớn! Tìm chòi trú mưa, giữ thư khô.";
        rain.stormNotificationDuration = 4f;
        rain.rainDialogue = "";
    }

    void CreateChapter3Rain()
    {
        var rain = chapterRoot.AddComponent<RainEvent>();
        rain.triggerDuringStepId = "read_brother_letter";
        rain.rainFromChapterStart = true;
        rain.escalateToStorm = true;
        rain.showRainDialogue = false;
        rain.stormNotification = "Mưa nặng hạt trên chiến trường...";
        rain.stormNotificationDuration = 3f;
    }

    void CreateMotherNpc()
    {
        var go = CreateNpcMarker("Mẹ anh lính", motherNpcPosition, new Color(1f, 0.7f, 0.8f), NpcVisualFactory.NpcRole.Civilian, "NhanVat/MeAnhLinh/MeAnhLinh", false, false);
        var model = go.transform.Find("NpcModel");
        if (model != null)
        {
            model.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            NpcVisualFactory.AutoAdjustGroundPlacement(model);
        }
        AttachNpcHeadLabel(go, "Mẹ người lính");
        RegisterWaypoint("deliver_mother", go.transform.position);
        go.AddComponent<MotherDeliveryInteractable>();
    }

    void PreloadVoicesForChapter(int chapter)
    {
        switch (chapter)
        {
            case 1: PreloadChapter1Voices(); break;
            case 2: PreloadChapter2Voices(); break;
            case 3: PreloadChapter3Voices(); break;
        }
    }

    void PreloadChapter2Voices()
    {
        DialogueAudio.Preload(
            refreshFromDisk: true,
            Chapter2Voice.Transition,
            Chapter2Voice.IntroHud,
            Chapter2Voice.Soldier,
            Chapter2Voice.SoldierNam,
            Chapter2Voice.StealthNam,
            Chapter2Voice.MotherAsk,
            Chapter2Voice.MotherNam,
            Chapter2Voice.MotherThanks,
            Chapter2Voice.EndNam);
    }

    void PreloadChapter1Voices()
    {
        DialogueAudio.Preload(
            refreshFromDisk: true,
            Chapter1Voice.Transition,
            Chapter1Voice.IntroHud,
            Chapter1Voice.PickupStation,
            Chapter1Voice.PickupNam,
            Chapter1Voice.Elder,
            Chapter1Voice.ElderNam,
            Chapter1Voice.CrossObstacle,
            Chapter1Voice.DeliverBaLan1,
            Chapter1Voice.DeliverNam,
            Chapter1Voice.DeliverBaLan2,
            Chapter1Voice.EndNam);
    }

    void PreloadChapter3Voices()
    {
        DialogueAudio.Preload(
            refreshFromDisk: true,
            Chapter3Voice.Transition,
            Chapter3Voice.IntroHud,
            Chapter3Voice.ClueHouse,
            Chapter3Voice.ClueBunker,
            Chapter3Voice.ClueFort,
            Chapter3Voice.CluesComplete,
            Chapter3Voice.BrotherFound,
            Chapter3Voice.BrotherLetter,
            Chapter3Voice.BrotherNam,
            Chapter3Voice.DeliverNam,
            Chapter3Voice.DeliverRecipient,
            Chapter3Voice.EndNam,
            Chapter3Voice.Ending);
    }

    void CreateCluePoints()
    {
        collectedClueIds.Clear();
        RegisterWaypoint("read_brother_letter", clue3Position);
        CreateHouseClue("Ngôi nhà bỏ hoang",
            Chapter3Dialogue.ClueHouse,
            Chapter3Voice.ClueHouse);

        CreateClue("bunker", clue2Position, "Hầm trú ẩn",
            Chapter3Dialogue.ClueBunker, Chapter3Voice.ClueBunker);

        CreateFortClue("Đồn lính đổ nát", Chapter3Dialogue.ClueFort, Chapter3Voice.ClueFort);
        RegisterWaypoint("find_clues", clue1Position);
        RefreshClueWaypoint();
    }

    void CreateHouseClue(string title, string hint, string voiceKey)
    {
        var house = GameObject.Find("house");
        if (house == null)
        {
            clue1Position = ForestZoneLayout.ResolveHouseCluePosition();
            CreateClue("house", clue1Position, title, hint, voiceKey);
            return;
        }

        foreach (var mf in house.GetComponentsInChildren<MeshFilter>())
        {
            if (mf.GetComponent<Collider>() == null)
                mf.gameObject.AddComponent<MeshCollider>();
        }

        clue1Position = house.transform.position + Vector3.up * 1f;

        var trigger = new GameObject("Clue_NgoiNha");
        trigger.transform.SetParent(chapterRoot.transform);
        trigger.transform.position = clue1Position;

        var box = trigger.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(4f, 3f, 4f);

        AttachClueInteractable(trigger, "house", title, hint, voiceKey, hideAfterCollect: true);
        BoostClueVisibility(trigger.transform);
        AttachClueHeadLabel(trigger, title);
    }

    void CreateFortClue(string title, string hint, string voiceKey)
    {
        var go = new GameObject("Clue_Fort");
        go.transform.SetParent(chapterRoot.transform);
        go.transform.position = GroundSnap.Snap(clue3Position);

        var wallPrefab = Resources.Load<GameObject>("RuinedWall/RuinedWall");
        if (wallPrefab != null)
        {
            var wall = Object.Instantiate(wallPrefab, go.transform);
            wall.transform.localPosition = Vector3.zero;
            wall.transform.localRotation = Quaternion.Euler(0f, 135f, 0f);
            wall.transform.localScale = Vector3.one * 3f;
            wall.name = "RuinedWall";
        }

        var bagPrefab = Resources.Load<GameObject>("OldMailBag/OldMailBag");
        if (bagPrefab != null)
        {
            var bag = Object.Instantiate(bagPrefab, go.transform);
            bag.transform.localPosition = new Vector3(0.4f, 0.15f, 0.6f);
            bag.transform.localScale = Vector3.one * 1.5f;
            bag.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            bag.name = "OldMailBag";
        }

        var box = go.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.center = new Vector3(0f, 0.9f, 0f);
        box.size = new Vector3(5.5f, 2.8f, 5.5f);

        BoostClueVisibility(go.transform);
        AttachClueInteractable(go, "fort", title, hint, voiceKey, hideAfterCollect: false);
        AttachClueHeadLabel(go, title);

        foreach (var col in go.GetComponentsInChildren<Collider>())
        {
            if (col != box)
                Destroy(col);
        }
    }

    void CreateClue(string clueId, Vector3 pos, string title, string hint, string voiceKey, string modelPrefab = null)
    {
        var go = CreateMarker(title, pos, new Color(1f, 0.84f, 0.18f), new Vector3(2.6f, 2.8f, 2.6f));

        var worldLabel = go.transform.Find("ObjectiveLabel");
        if (worldLabel != null)
            Destroy(worldLabel.gameObject);
        AttachClueHeadLabel(go, title);
        
        if (!string.IsNullOrEmpty(modelPrefab))
        {
            var prefab = Resources.Load<GameObject>(modelPrefab);
            if (prefab != null)
            {
                var visual = Object.Instantiate(prefab, go.transform);
                if (modelPrefab.Contains("OldMailBag"))
                    visual.transform.localPosition = new Vector3(0f, 0.45f, 0f);
                else
                    visual.transform.localPosition = Vector3.zero;
                visual.transform.localScale = Vector3.one * 1.5f;
                visual.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            }
        }

        BoostClueVisibility(go.transform);
        AttachClueInteractable(go, clueId, title, hint, voiceKey, hideAfterCollect: true);
    }

    void BoostClueVisibility(Transform root)
    {
        var beacon = root.Find("BeaconLight")?.GetComponent<Light>();
        if (beacon == null)
        {
            var lightGo = new GameObject("BeaconLight");
            lightGo.transform.SetParent(root, false);
            lightGo.transform.localPosition = new Vector3(0f, 3.5f, 0f);
            beacon = lightGo.AddComponent<Light>();
            beacon.type = LightType.Point;
            beacon.color = new Color(1f, 0.84f, 0.18f);
        }

        beacon.intensity = 5f;
        beacon.range = 20f;

        var col = root.GetComponent<SphereCollider>();
        if (col != null)
            col.radius = 2.2f;
    }

    void AttachClueInteractable(GameObject go, string clueId, string title, string hint, string voiceKey, bool hideAfterCollect)
    {
        var clue = go.AddComponent<ClueInteractable>();
        clue.clueId = clueId;
        clue.clueTitle = title;
        clue.clueHint = hint;
        clue.dialogueSpeaker = "Nam";
        clue.voiceKey = voiceKey;
        clue.promptText = "Nhấn E - Khám phá manh mối";
        clue.hideAfterCollect = hideAfterCollect;
    }

    public bool TryRegisterClue(string clueId)
    {
        if (QuestManager.Instance == null || !QuestManager.Instance.IsStepActive("find_clues"))
            return false;
        if (!collectedClueIds.Add(clueId))
            return false;

        bool hasAll =
            HasCollectedClue("house") &&
            HasCollectedClue("bunker") &&
            HasCollectedClue("fort");

        if (hasAll)
        {
            QuestManager.Instance.CompleteStep("find_clues");
            var dm = DialogueManager.Instance;
            if (dm != null)
            {
                dm.ShowDialogue("Nam", Chapter3Dialogue.CluesComplete, Chapter3Voice.CluesComplete, () =>
                {
                    dm.ShowDialogue("Nam", Chapter3Dialogue.BrotherFound, Chapter3Voice.BrotherFound, () => ShowBrotherLetter());
                });
            }
            else
                ShowBrotherLetter();
            return true;
        }

        GameUI.Instance?.ShowNotification("Đã tìm thấy manh mối");
        RefreshClueWaypoint();
        return true;
    }

    void RefreshClueWaypoint()
    {
        if (QuestManager.Instance == null) return;
        if (!QuestManager.Instance.IsStepActive("find_clues") &&
            !QuestManager.Instance.IsStepActive("check_map"))
            return;

        if (!HasCollectedClue("house"))
            RegisterWaypoint("find_clues", clue1Position);
        else if (!HasCollectedClue("bunker"))
            RegisterWaypoint("find_clues", clue2Position);
        else
            RegisterWaypoint("find_clues", clue3Position);
    }

    /// <summary>Gọi sau khi mở bản đồ xong bước check_map Chương 3.</summary>
    public void RefreshClueWaypointsAfterMapUnlock() => RefreshClueWaypoint();


    void ShowBrotherLetter()
    {
        if (!QuestManager.Instance.IsStepActive("read_brother_letter")) return;

        GameUI.Instance?.ShowLetter("Thư của anh trai Nam",
            Chapter3Dialogue.BrotherLetterBody,
            Chapter3Voice.BrotherLetter,
            () =>
            {
                DialogueManager.Instance?.ShowDialogue("Nam", Chapter3Dialogue.BrotherNam, Chapter3Voice.BrotherNam, () =>
                    QuestManager.Instance.CompleteStep("read_brother_letter"));
            });
    }

    void CreateFinalDeliveryNpc()
    {
        var go = CreateNpcMarker("Trạm thư cuối", finalDeliveryPosition, new Color(0.9f, 0.75f, 0.2f),
            NpcVisualFactory.NpcRole.Civilian, null, true, false);
        AttachNpcHeadLabel(go, "Trạm thư cuối");
        RegisterWaypoint("final_delivery", go.transform.position);
        go.AddComponent<FinalDeliveryInteractable>();
    }

    static void RegisterWaypoint(string questStepId, Vector3 pos) =>
        QuestWaypointRegistry.Register(questStepId, pos);

    GameObject CreateZone(string name, Vector3 pos, Vector3 size, string questId, Color color)
    {
        var go = CreateInvisibleTrigger(name, pos, size);
        if (!string.IsNullOrEmpty(questId))
        {
            var zone = go.AddComponent<QuestZone>();
            zone.questStepId = questId;
        }
        return go;
    }

    GameObject CreateInvisibleTrigger(string name, Vector3 pos, Vector3 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(chapterRoot.transform);
        go.transform.position = pos;
        var box = go.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = size;
        return go;
    }

    GameObject CreateNpcMarker(string name, Vector3 pos, Color accent, NpcVisualFactory.NpcRole role, string overrideModelPath = null, bool showFootRing = true, bool showDefaultLabel = true)
    {
        var go = new GameObject(name);
        go.transform.SetParent(chapterRoot.transform);
        go.transform.position = GroundSnap.SnapCharacter(pos);

        var col = go.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 1.2f;
        col.center = new Vector3(0f, 0f, 0f);

        NpcVisualFactory.Attach(go.transform, role, overrideModelPath);
        if (showDefaultLabel)
            CreateObjectiveLabel(go.transform, name);
        if (showFootRing) CreateNpcFootRing(go.transform, accent);
        return go;
    }

    void CreateNpcFootRing(Transform parent, Color color)
    {
        var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "FootRing";
        ring.transform.SetParent(parent);
        ring.transform.localPosition = new Vector3(0f, -GroundSnap.CharacterFootToPivot + 0.05f, 0f);
        ring.transform.localScale = new Vector3(1.4f, 0.03f, 1.4f);
        if (ring.GetComponent<Collider>() != null) Destroy(ring.GetComponent<Collider>());
        var renderer = ring.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material = CreateURPMaterial(color, 0.45f);
    }

    GameObject CreateMarker(string name, Vector3 pos, Color color, Vector3 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(chapterRoot.transform);
        go.transform.position = GroundSnap.Snap(pos);

        var col = go.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = Mathf.Max(size.x, size.z) * 0.55f;
        col.center = new Vector3(0f, 1.2f, 0f);

        CreateObjectiveLabel(go.transform, name);
        CreateBeaconLight(go.transform, color);
        return go;
    }

    void CreateObjectiveLabel(Transform parent, string label, string questStepId = null, float height = 2.8f)
    {
        var canvasGo = new GameObject("ObjectiveLabel");
        canvasGo.transform.SetParent(parent);
        canvasGo.transform.localPosition = new Vector3(0f, height, 0f);

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        if (Camera.main != null)
            canvas.worldCamera = Camera.main;
        canvasGo.AddComponent<FaceCamera>();
        var rt = canvasGo.GetComponent<RectTransform>();
        rt.sizeDelta = CrispUiText.NameTagSize;
        rt.localScale = Vector3.one * 0.011f;

        var bgGo = new GameObject("Bg", typeof(RectTransform), typeof(Image));
        bgGo.transform.SetParent(canvasGo.transform, false);
        var bgRt = bgGo.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;
        bgGo.GetComponent<Image>().color = CrispUiText.NameTagBg;

        var textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
        textGo.transform.SetParent(canvasGo.transform, false);
        var textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = CrispUiText.NameTagTextInsetMin;
        textRt.offsetMax = CrispUiText.NameTagTextInsetMax;

        CrispUiText.ConfigureNameTagText(textGo.GetComponent<Text>(), label);

        if (!string.IsNullOrEmpty(questStepId))
        {
            var stepLabel = canvasGo.AddComponent<QuestStepLabel>();
            stepLabel.questStepId = questStepId;
        }
    }

    void CreateBeaconLight(Transform parent, Color color)
    {
        var lightGo = new GameObject("BeaconLight");
        lightGo.transform.SetParent(parent);
        lightGo.transform.localPosition = new Vector3(0f, 3.5f, 0f);
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = 2.5f;
        light.range = 12f;
    }

    Material CreateURPMaterial(Color color, float alpha)
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var mat = new Material(shader);
        var c = new Color(color.r, color.g, color.b, alpha);
        mat.SetColor("_BaseColor", c);
        mat.color = c;
        return mat;
    }

    void OnChapter1Complete()
    {
        DialogueManager.Instance?.ShowDialogue("Nam", Chapter1Dialogue.EndNam, Chapter1Voice.EndNam, () =>
            GameManager.Instance?.CompleteChapter(1));
    }

    void OnChapter2Complete()
    {
        DialogueManager.Instance?.ShowDialogue("Nam", Chapter2Dialogue.EndNam, Chapter2Voice.EndNam, () =>
            GameManager.Instance?.CompleteChapter(2));
    }

    void OnChapter3Complete()
    {
        DialogueManager.Instance?.ShowDialogue("Nam", Chapter3Dialogue.EndNam, Chapter3Voice.EndNam, () =>
            GameManager.Instance?.CompleteChapter(3));
    }

    void OnDestroy()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnAllQuestsCompleted -= OnChapter1Complete;
            QuestManager.Instance.OnAllQuestsCompleted -= OnChapter2Complete;
            QuestManager.Instance.OnAllQuestsCompleted -= OnChapter3Complete;
        }
    }
}
