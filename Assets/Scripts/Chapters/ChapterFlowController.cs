using System.Collections;
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

        SetupChapterIntro();

        switch (chapterIndex)
        {
            case 1: SetupChapter1(); break;
            case 2: SetupChapter2(); break;
            case 3: SetupChapter3(); break;
        }

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
                break;
            case 3:
                WeatherController.Create(WeatherController.WeatherPreset.Battlefield);
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

    void SetupChapterIntro()
    {
        string title = chapterIndex switch
        {
            1 => "Chương 1: Con Đường Hy Vọng",
            2 => "Chương 2: Bóng Tối Chiến Tranh",
            3 => "Chương 3: Lá Thư Cuối Cùng",
            _ => ""
        };

        GameUI.Instance?.SetChapterLabel(title);

        var zone = ForestZoneLayout.GetZone(chapterIndex);
        GameUI.Instance?.SetZoneLabel(zone.shortName);

        if (chapterIndex == 1)
        {
            DialogueManager.Instance?.ShowDialogue("",
                "Chương 1 — Rìa làng.\n\n" +
                "• Nhiệm vụ dàn khắp map — đi theo mũi tên HUD\n" +
                "• 5 mục tiêu từ tây nam → đông bắc\n" +
                "• Tránh vòng đỏ lính tuần tra, dùng bụi xanh nếu cần",
                Chapter1Voice.IntroHud);
        }
        else if (chapterIndex == 2)
        {
            DialogueManager.Instance?.ShowDialogue("",
                "Chương 2 — Rừng sâu, đêm tối.\n\n" +
                "• Tiếp tục khám phá map theo mũi tên HUD\n" +
                "• Lẻn qua lính tuần tra — núp bụi xanh\n" +
                "• Tìm chỗ trú mưa trước khi giao thư",
                Chapter2Voice.IntroHud);
        }
        else if (chapterIndex == 3)
        {
            DialogueManager.Instance?.ShowDialogue("",
                "Chương 3 — Vùng chiến sự.\n\n" +
                "• Manh mối rải khắp map — đi theo mũi tên HUD\n" +
                "• Tìm 3 manh mối → đọc thư anh trai → vượt pháo kích\n" +
                "• Giao lá thư cuối ở phía đông bắc map",
                Chapter3Voice.IntroHud);
        }
    }

    void SetupChapter1()
    {
        var qm = QuestManager.Instance;
        qm.OnAllQuestsCompleted -= OnChapter1Complete;
        qm.SetupQuests(
            ("pickup_mail", "Lấy túi thư tại Trạm Liên Lạc"),
            ("ask_elder", "Hỏi cụ già đường vào làng"),
            ("cross_obstacle", "Vượt qua khu gỗ đổ chặn đường"),
            ("sneak_patrol", "Lẻn qua lính tuần tra (tránh vòng đỏ)"),
            ("deliver_mail", "Giao thư cho Bà Lan")
        );
        qm.OnAllQuestsCompleted += OnChapter1Complete;

        CreateMailStation();
        CreateElderNpc();
        CreateObstacleSection();
        CreateStealthPath(1, ForestZoneLayout.Ch1StealthEnd, "sneak_patrol",
            ForestZoneLayout.Ch1HideSpot, ForestZoneLayout.Ch1Obstacle);
        CreateDeliveryNpc("Bà Lan", deliveryNpcPosition);
    }

    void CreateElderNpc()
    {
        var go = CreateNpcMarker("Cụ già", elderNpcPosition, new Color(0.55f, 0.45f, 0.3f), NpcVisualFactory.NpcRole.Civilian, "NhanVat/BaLao/BaLao", false);
        var model = go.transform.Find("NpcModel");
        if (model != null)
        {
            model.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            NpcVisualFactory.AutoAdjustGroundPlacement(model);
        }
        RegisterWaypoint("ask_elder", go.transform.position);
        var elder = go.AddComponent<ElderGuideInteractable>();
        elder.promptText = "Nhấn E - Hỏi đường";
    }

    void CreateObstacleSection()
    {
        var go = CreateMarker("Khu gỗ đổ", obstacleZonePosition, new Color(0.25f, 0.65f, 0.3f), new Vector3(2f, 2f, 2f));
        CreateFallenLogProp(go.transform);
        RegisterWaypoint("cross_obstacle", go.transform.position);
        var obstacle = go.AddComponent<ObstacleCrossInteractable>();
        obstacle.promptText = "Nhấn E - Vượt qua";
    }

    void SetupChapter2()
    {
        var qm = QuestManager.Instance;
        qm.OnAllQuestsCompleted -= OnChapter2Complete;
        qm.SetupQuests(
            ("receive_letter", "Nhận thư từ người lính trẻ"),
            ("stealth_cross", "Lẻn qua khu tuần tra"),
            ("survive_rain", "Trú mưa dưới gốc cây"),
            ("deliver_mother", "Giao thư cho mẹ anh lính")
        );
        qm.OnAllQuestsCompleted += OnChapter2Complete;

        CreateSoldierNpc();
        CreateStealthPath(2, ForestZoneLayout.Ch2StealthEnd, "stealth_cross",
            ForestZoneLayout.Ch2HideSpot, ForestZoneLayout.Ch2Soldier);
        CreateRainEvent();
        CreateMotherNpc();
    }

    void SetupChapter3()
    {
        var qm = QuestManager.Instance;
        qm.OnAllQuestsCompleted -= OnChapter3Complete;
        qm.SetupQuests(
            ("find_clues", "Tìm manh mối túi thư (0/3)"),
            ("read_brother_letter", "Đọc thư anh trai"),
            ("cross_danger", "Vượt vùng pháo kích"),
            ("final_delivery", "Giao thư cho các gia đình")
        );
        qm.OnAllQuestsCompleted += OnChapter3Complete;

        CreateCluePoints();
        SpawnPatrols(3, "find_clues", ForestZoneLayout.Ch3Spawn);
        CreateHideSpot(ForestZoneLayout.Ch3DangerReset, "find_clues");
        CreateDangerArea();
        SpawnPatrols(3, "cross_danger", ForestZoneLayout.Ch3DangerReset);
        CreateHideSpot(ForestZoneLayout.Ch3HideSpot, "cross_danger");
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
    }

    void CreateSoldierNpc()
    {
        var go = CreateNpcMarker("Người lính trẻ", soldierNpcPosition, Color.blue, NpcVisualFactory.NpcRole.Soldier);
        RegisterWaypoint("receive_letter", go.transform.position);
        go.AddComponent<SoldierLetterInteractable>();
    }

    void CreateStealthPath(int chapter, Vector3 endPos, string questId, Vector3 hidePos, Vector3 resetPos)
    {
        var end = GroundSnap.Snap(endPos);
        CreateZone($"StealthEnd_{questId}", end, new Vector3(5f, 3f, 5f), questId, Color.clear);
        RegisterWaypoint(questId, end);

        SpawnPatrols(chapter, questId, resetPos);
        CreateHideSpot(hidePos, questId);
    }

    void SpawnPatrols(int chapter, string questId, Vector3 resetPos)
    {
        int index = 0;
        foreach (var setup in ChapterDifficulty.GetPatrols(chapter))
        {
            if (setup.activeQuestId != questId) continue;

            var patrol = new GameObject($"PatrolEnemy_{questId}_{index++}");
            patrol.transform.SetParent(chapterRoot.transform);
            patrol.transform.position = GroundSnap.SnapCharacter(setup.pointA);

            var enemy = patrol.AddComponent<StealthEnemy>();
            enemy.pointA = GroundSnap.SnapCharacter(setup.pointA);
            enemy.pointB = GroundSnap.SnapCharacter(setup.pointB);

            Vector3 chapterStart = chapter switch
            {
                1 => ForestZoneLayout.Ch1Start,
                2 => ForestZoneLayout.Ch2Start,
                3 => ForestZoneLayout.Ch3Start,
                _ => resetPos
            };
            enemy.resetPosition = GroundSnap.SnapCharacter(chapterStart);
            enemy.moveSpeed = setup.moveSpeed;
            enemy.detectRadius = setup.detectRadius;
            enemy.detectSeconds = setup.detectSeconds;
            enemy.mustHideToPass = setup.mustHideToPass;
            enemy.activeQuestId = setup.activeQuestId;

            var modelRoot = NpcVisualFactory.Attach(patrol.transform, NpcVisualFactory.NpcRole.Enemy, "NhanVat/LowPolySoldiers_demo/models/Soldier_demo");
            if (modelRoot != null)
            {
                modelRoot.localScale = Vector3.one;
                NpcVisualFactory.AutoAdjustGroundPlacement(modelRoot);
                PatrolVisibility.Apply(patrol.transform);
                var patrolAnim = patrol.AddComponent<NpcPatrolAnimator>();
                patrolAnim.modelRoot = modelRoot;
            }
            else
            {
                CreateObjectiveLabel(patrol.transform, "Lính tuần tra");
                PatrolVisibility.Apply(patrol.transform);
            }
        }
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

    void CreateDangerArea()
    {
        var center = GroundSnap.Snap(ForestZoneLayout.Ch3DangerZone);
        var go = CreateInvisibleTrigger("ArtilleryZone", center, new Vector3(18f, 4f, 18f));
        var danger = go.AddComponent<DangerZone>();
        danger.resetPoint = GroundSnap.Snap(ForestZoneLayout.Ch3DangerReset);
        danger.exposureLimit = ChapterDifficulty.DangerExposureLimit(3);
        danger.activeQuestId = "cross_danger";

        var barrage = go.AddComponent<ArtilleryBarrage>();
        barrage.activeQuestId = "cross_danger";
        barrage.zoneRadius = 8.5f;

        var safeEnd = GroundSnap.Snap(ForestZoneLayout.Ch3DangerExit);
        CreateZone("DangerCrossEnd", safeEnd, new Vector3(5f, 3f, 5f), "cross_danger", Color.clear);
        RegisterWaypoint("cross_danger", safeEnd);
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

    void CreateRainEvent()
    {
        CreateZone("RainShelter", rainShelterPosition, new Vector3(5, 3, 5), "survive_rain", new Color(0.4f, 0.3f, 0.2f));
        RegisterWaypoint("survive_rain", rainShelterPosition);
        var rain = chapterRoot.AddComponent<RainEvent>();
        rain.triggerDuringStepId = "stealth_cross";
    }

    void CreateMotherNpc()
    {
        var go = CreateNpcMarker("Mẹ anh lính", motherNpcPosition, new Color(1f, 0.7f, 0.8f), NpcVisualFactory.NpcRole.Civilian, "NhanVat/MeAnhLinh/MeAnhLinh", false);
        var model = go.transform.Find("NpcModel");
        if (model != null)
        {
            model.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            NpcVisualFactory.AutoAdjustGroundPlacement(model);
        }
        RegisterWaypoint("deliver_mother", go.transform.position);
        go.AddComponent<MotherDeliveryInteractable>();
    }

    int cluesFound;

    void CreateCluePoints()
    {
        cluesFound = 0;
        RegisterWaypoint("read_brother_letter", clue3Position);
        CreateHouseClue("Ngôi nhà bỏ hoang",
            "Căn nhà hoang vắng cạnh chiến trường cũ. Có dấu vết ai đó từng ghé qua...",
            Chapter3Voice.ClueHouse);
        CreateClue(clue2Position, "Hầm trú ẩn", "Một túi vải rách cũ kỹ.", Chapter3Voice.ClueBunker);
        CreateClue(clue3Position, "Đồn lính đổ nát", "Túi thư cũ dưới đống gạch...", Chapter3Voice.ClueFort);
    }

    void CreateHouseClue(string title, string hint, string voiceKey)
    {
        clue1Position = ForestZoneLayout.ResolveHouseCluePosition();
        RegisterWaypoint("find_clues", clue1Position);

        var house = GameObject.Find("house");
        if (house == null)
        {
            CreateClue(clue1Position, title, hint, voiceKey);
            return;
        }

        var trigger = new GameObject("Clue_NgoiNha");
        trigger.transform.SetParent(chapterRoot.transform);
        trigger.transform.position = house.transform.position + Vector3.up * 2f;

        var box = trigger.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(12f, 6f, 12f);

        AttachClueInteractable(trigger, title, hint, voiceKey, hideAfterCollect: true);
        BoostClueVisibility(trigger.transform);
    }

    void CreateClue(Vector3 pos, string title, string hint, string voiceKey)
    {
        var go = CreateMarker(title, pos, new Color(1f, 0.84f, 0.18f), new Vector3(2.6f, 2.8f, 2.6f));
        BoostClueVisibility(go.transform);
        AttachClueInteractable(go, title, hint, voiceKey, hideAfterCollect: true);
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

    void AttachClueInteractable(GameObject go, string title, string hint, string voiceKey, bool hideAfterCollect)
    {
        var clue = go.AddComponent<ClueInteractable>();
        clue.clueTitle = title;
        clue.clueHint = hint;
        clue.voiceKey = voiceKey;
        clue.promptText = "Nhấn E - Khám phá manh mối";
        clue.hideAfterCollect = hideAfterCollect;
        clue.onClueFound = () =>
        {
            cluesFound++;
            GameUI.Instance?.ShowNotification($"Manh mối {cluesFound}/3");

            if (cluesFound >= 3 && QuestManager.Instance.IsStepActive("find_clues"))
            {
                QuestManager.Instance.CompleteStep("find_clues");
                ShowBrotherLetter();
            }
            else if (QuestManager.Instance.IsStepActive("find_clues"))
            {
                var step = QuestManager.Instance.CurrentStep;
                if (step != null)
                    step.description = $"Tìm manh mối túi thư ({cluesFound}/3)";
                GameUI.Instance?.RefreshQuestUI();
                if (cluesFound == 1) RegisterWaypoint("find_clues", clue2Position);
                else if (cluesFound == 2) RegisterWaypoint("find_clues", clue3Position);
            }
        };
    }

    void ShowBrotherLetter()
    {
        if (!QuestManager.Instance.IsStepActive("read_brother_letter")) return;

        GameUI.Instance?.ShowLetter("Thư của anh trai Nam",
            "Nếu em nhận được lá thư này, có lẽ anh đã không thể trở về.\n\n" +
            "Hãy thay anh chăm sóc mẹ.\nVà hãy sống tiếp thật tốt.",
            Chapter3Voice.BrotherLetter,
            () => QuestManager.Instance.CompleteStep("read_brother_letter"));
    }

    void CreateFinalDeliveryNpc()
    {
        var go = CreateNpcMarker("Trạm thư cuối", finalDeliveryPosition, new Color(0.9f, 0.75f, 0.2f), NpcVisualFactory.NpcRole.Civilian);
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

    GameObject CreateNpcMarker(string name, Vector3 pos, Color accent, NpcVisualFactory.NpcRole role, string overrideModelPath = null, bool showFootRing = true)
    {
        var go = new GameObject(name);
        go.transform.SetParent(chapterRoot.transform);
        go.transform.position = GroundSnap.SnapCharacter(pos);

        var col = go.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 1.2f;
        col.center = new Vector3(0f, 0f, 0f);

        NpcVisualFactory.Attach(go.transform, role, overrideModelPath);
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

    void CreateFallenLogProp(Transform parent)
    {
        var logColor = new Color(0.42f, 0.26f, 0.12f);
        CreateLog(parent, new Vector3(-1.2f, 0.35f, 0f), new Vector3(3.2f, 0.45f, 0.55f), Quaternion.Euler(0f, 15f, 88f), logColor);
        CreateLog(parent, new Vector3(0.8f, 0.28f, 0.6f), new Vector3(2.8f, 0.4f, 0.5f), Quaternion.Euler(5f, -25f, 92f), logColor);
        CreateLog(parent, new Vector3(0.2f, 0.55f, -0.5f), new Vector3(2.4f, 0.38f, 0.48f), Quaternion.Euler(-8f, 40f, 85f), logColor * 0.9f);
    }

    void CreateLog(Transform parent, Vector3 localPos, Vector3 scale, Quaternion rotation, Color color)
    {
        var log = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        log.name = "FallenLog";
        log.transform.SetParent(parent);
        log.transform.localPosition = localPos;
        log.transform.localScale = scale;
        log.transform.localRotation = rotation;
        if (log.GetComponent<Collider>() != null) Destroy(log.GetComponent<Collider>());
        var renderer = log.GetComponent<Renderer>();
        if (renderer != null) renderer.material = CreateURPMaterial(color, 1f);
    }

    void CreateObjectiveLabel(Transform parent, string label)
    {
        var canvasGo = new GameObject("ObjectiveLabel");
        canvasGo.transform.SetParent(parent);
        canvasGo.transform.localPosition = new Vector3(0f, 2.8f, 0f);

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasGo.AddComponent<FaceCamera>();
        var rt = canvasGo.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(260, 44);
        rt.localScale = Vector3.one * 0.011f;

        var bgGo = new GameObject("Bg", typeof(RectTransform), typeof(Image));
        bgGo.transform.SetParent(canvasGo.transform, false);
        var bgRt = bgGo.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;
        bgGo.GetComponent<Image>().color = new Color(0.02f, 0.02f, 0.03f, 0.98f);

        var textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
        textGo.transform.SetParent(canvasGo.transform, false);
        var textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = new Vector2(5, 4);
        textRt.offsetMax = new Vector2(-5, -4);

        var t = textGo.GetComponent<Text>();
        CrispUiText.ConfigureWorldLabel(t, 36, Color.white, label);
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
        DialogueManager.Instance?.ShowDialogue("Nam", "Mỗi lá thư đều mang theo hy vọng...", Chapter1Voice.EndNam, () =>
        {
            DialogueManager.Instance?.ShowDialogue("", "Nam được tuyển vào đội vận chuyển thư.", Chapter1Voice.EndNarrator, () =>
                GameManager.Instance?.CompleteChapter(1));
        });
    }

    void OnChapter2Complete()
    {
        DialogueManager.Instance?.ShowDialogue("Bà Lan", "Con ta... con ta không trở về nữa...", Chapter2Voice.EndBaLan, () =>
        {
            DialogueManager.Instance?.ShowDialogue("Nam", "Chiến tranh thật tàn khốc.", Chapter2Voice.EndNam, () =>
                GameManager.Instance?.CompleteChapter(2));
        });
    }

    void OnChapter3Complete()
    {
        GameManager.Instance?.CompleteChapter(3);
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
