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
        SetupForestZones();

        if (FindFirstObjectByType<QuestNavigator>() == null)
            chapterRoot.AddComponent<QuestNavigator>();

        AddChapterPressureTimer();

        SetupChapterIntro();

        switch (chapterIndex)
        {
            case 1: SetupChapter1(); break;
            case 2: SetupChapter2(); break;
            case 3: SetupChapter3(); break;
        }

        SetupWeather();
        GenerateEnvironment();
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
                clue1Position = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch3Clue1);
                clue2Position = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch3Clue2);
                clue3Position = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch3Clue3);
                finalDeliveryPosition = ForestZoneLayout.SnapPoint(ForestZoneLayout.Ch3FinalDelivery);
                break;
        }
    }

    void SetupForestZones()
    {
        var guideGo = new GameObject("ForestZoneGuide");
        guideGo.transform.SetParent(chapterRoot.transform);
        var guide = guideGo.AddComponent<ForestZoneGuide>();
        guide.activeChapter = chapterIndex;
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
        GameUI.Instance?.SetZoneLabel($"📍 {zone.shortName}  •  {zone.fullName}");

        if (chapterIndex == 1)
        {
            DialogueManager.Instance?.ShowDialogue("",
                "Chương 1 — Rìa làng.\n" +
                "5 nhiệm vụ — cuối cùng phải lẻn qua lính tuần tra.\n" +
                "Tránh vòng đỏ, dùng bụi xanh nếu cần. Nhấn E tại cột vàng.");
        }
        else if (chapterIndex == 2)
        {
            DialogueManager.Instance?.ShowDialogue("",
                "Chương 2 — Rừng sâu, khó hơn.\n" +
                "Hai lính tuần tra + mưa bão. Bắt buộc núp bụi xanh mới qua an toàn.\n" +
                "Tìm chỗ trú mưa trước khi giao thư.");
        }
        else if (chapterIndex == 3)
        {
            DialogueManager.Instance?.ShowDialogue("",
                "Chương 3 — Vùng chiến sự.\n" +
                "Tìm 3 manh mối dưới hỏa lực, đọc thư anh trai,\n" +
                "vượt vùng pháo kích rồi giao thư cuối. Không đứng lâu vùng đỏ!");
        }
    }

    void SetupChapter1()
    {
        var qm = QuestManager.Instance;
        qm.OnAllQuestsCompleted -= OnChapter1Complete;
        qm.SetupQuests(
            ("pickup_mail", "Lấy túi thư tại Trạm Liên Lạc"),
            ("ask_elder", "Hỏi cụ già đường vào làng"),
            ("cross_obstacle", "Vượt qua khu gỗ mục chặn đường"),
            ("sneak_patrol", "Lẻn qua lính tuần tra (tránh vòng đỏ)"),
            ("deliver_mail", "Giao thư đầu tiên cho Anh Sơn"),
            ("return_to_base", "Trở về Trạm Liên Lạc")
        );
        qm.OnAllQuestsCompleted += OnChapter1Complete;

        CreateMailStation();
        CreateElderNpc();
        CreateObstacleSection();
        CreateStealthPath(1, ForestZoneLayout.Ch1StealthEnd, "sneak_patrol",
            ForestZoneLayout.Ch1HideSpot, ForestZoneLayout.Ch1Obstacle);
        CreateDeliveryNpc("Anh Sơn", deliveryNpcPosition);
        CreateReturnToBaseTrigger();
    }

    void CreateReturnToBaseTrigger()
    {
        CreateZone("ReturnToBaseZone", mailStationPosition, new Vector3(6f, 3f, 6f), "return_to_base", Color.clear);
        RegisterWaypoint("return_to_base", mailStationPosition);
    }

    void CreateElderNpc()
    {
        var go = CreateMarker("Cụ già", elderNpcPosition, new Color(0.55f, 0.45f, 0.3f), new Vector3(1.5f, 2f, 1.5f));
        RegisterWaypoint("ask_elder", go.transform.position);
        var elder = go.AddComponent<ElderGuideInteractable>();
        elder.promptText = "Nhấn E - Hỏi đường";
        // Put a conical hat on the Elder
        SpawnHatVisual(go.transform, new Color(0.85f, 0.78f, 0.65f), isSoldier: false);
    }

    void CreateObstacleSection()
    {
        var pos = GroundSnap.Snap(obstacleZonePosition);
        var go = CreateMarker("Khu gỗ mục", pos, new Color(0.25f, 0.65f, 0.3f), new Vector3(2f, 2f, 2f));
        RegisterWaypoint("cross_obstacle", go.transform.position);
        var obstacle = go.AddComponent<ObstacleCrossInteractable>();
        obstacle.promptText = "Nhấn E - Vượt qua";

        // Spawn actual physical logs to jump over
        SpawnFallenLog(pos + new Vector3(-2f, 0.2f, 0f), 45f, 5f);
        SpawnFallenLog(pos + new Vector3(0f, 0.3f, 1f), -30f, 6f);
        SpawnFallenLog(pos + new Vector3(2f, 0.1f, -1f), 15f, 4f);
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
        CreateHideSpot(new Vector3(27f, 0f, -8f), "cross_danger");
        CreateFinalRecipients();
    }

    void CreateMailStation()
    {
        var go = CreateMarker("Trạm Liên Lạc", mailStationPosition, new Color(1f, 0.82f, 0.15f), new Vector3(3f, 3f, 3f));
        CreateMailBagProp(go.transform);
        RegisterWaypoint("pickup_mail", go.transform.position);
        var pickup = go.AddComponent<MailPickupInteractable>();
        pickup.promptText = "Nhấn E - Lấy túi thư";
        pickup.recipientName = "Anh Sơn";
        pickup.mailSummary = "Thư từ tiền tuyến gửi cho Anh Sơn...";
        pickup.pickupDialogue = "Nam ơi, hãy mang túi thư này đến làng Bình An và giao bức thư đầu tiên cho Anh Sơn giúp tôi nhé.";
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
        var go = CreateMarker(name, pos, Color.yellow, new Vector3(1.5f, 2f, 1.5f));
        RegisterWaypoint("deliver_mail", go.transform.position);
        var delivery = go.AddComponent<MailDeliveryInteractable>();
        delivery.recipientName = name;
        delivery.promptText = "Nhấn E - Giao thư";
        delivery.deliveryDialogue = "Cảm ơn cháu! Vợ tôi đã sinh con rồi, tôi có con trai rồi! Bức thư này thực sự mang lại hy vọng lớn lao cho tôi.";
        SpawnHatVisual(go.transform, new Color(0.85f, 0.78f, 0.65f), isSoldier: false);
    }

    void CreateSoldierNpc()
    {
        var go = CreateMarker("Người lính trẻ", soldierNpcPosition, Color.blue, new Vector3(1.5f, 2f, 1.5f));
        RegisterWaypoint("receive_letter", go.transform.position);
        go.AddComponent<SoldierLetterInteractable>();
        SpawnHatVisual(go.transform, new Color(0.2f, 0.35f, 0.2f), isSoldier: true);
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
            patrol.transform.position = GroundSnap.Snap(setup.pointA);

            var enemy = patrol.AddComponent<StealthEnemy>();
            enemy.pointA = GroundSnap.Snap(setup.pointA);
            enemy.pointB = GroundSnap.Snap(setup.pointB);
            enemy.resetPosition = GroundSnap.Snap(resetPos);
            enemy.moveSpeed = setup.moveSpeed;
            enemy.detectRadius = setup.detectRadius;
            enemy.detectSeconds = setup.detectSeconds;
            enemy.mustHideToPass = setup.mustHideToPass;
            enemy.activeQuestId = setup.activeQuestId;

            CreateSmallIndicator(patrol.transform, Color.red);
            SpawnHatVisual(patrol.transform, new Color(0.25f, 0.3f, 0.25f), isSoldier: true);
        }
    }

    void CreateHideSpot(Vector3 hidePos, string questId)
    {
        var hideGo = CreateInvisibleTrigger($"HideSpot_{questId}", GroundSnap.Snap(hidePos), new Vector3(3.5f, 2.5f, 3.5f));
        hideGo.AddComponent<HideSpot>();
        SpawnBushModel(hideGo.transform, Vector3.zero, 3.2f);
        CreateSmallIndicator(hideGo.transform, new Color(0.15f, 0.75f, 0.35f));
    }

    void CreateDangerArea()
    {
        var center = GroundSnap.Snap(ForestZoneLayout.Ch3DangerZone);
        var go = CreateInvisibleTrigger("ArtilleryZone", center, new Vector3(14f, 4f, 14f));
        var danger = go.AddComponent<DangerZone>();
        danger.resetPoint = GroundSnap.Snap(ForestZoneLayout.Ch3DangerReset);
        danger.exposureLimit = ChapterDifficulty.DangerExposureLimit(3);
        danger.activeQuestId = "cross_danger";

        CreateSmallIndicator(go.transform, new Color(0.85f, 0.2f, 0.1f));

        var safeEnd = GroundSnap.Snap(new Vector3(30f, 0f, -6f));
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
        var go = CreateMarker("Mẹ anh lính", motherNpcPosition, new Color(1f, 0.7f, 0.8f), new Vector3(1.5f, 2f, 1.5f));
        RegisterWaypoint("deliver_mother", go.transform.position);
        go.AddComponent<MotherDeliveryInteractable>();
        SpawnHatVisual(go.transform, new Color(0.85f, 0.78f, 0.65f), isSoldier: false);
    }

    int cluesFound;

    void CreateCluePoints()
    {
        cluesFound = 0;
        RegisterWaypoint("find_clues", clue1Position);
        RegisterWaypoint("read_brother_letter", clue3Position);
        CreateClue(clue1Position, "Chiến hào bỏ hoang", "Dấu chân gần đống đổ nát...");
        CreateClue(clue2Position, "Hầm trú ẩn", "Một túi vải rách half-buried...");
        CreateClue(clue3Position, "Đồn lính đổ nát", "Túi thư cũ dưới đống gạch...");
    }

    void CreateClue(Vector3 pos, string title, string hint)
    {
        var go = CreateMarker(title, pos, new Color(0.8f, 0.6f, 0.2f), new Vector3(1.5f, 2f, 1.5f));
        var clue = go.AddComponent<ClueInteractable>();
        clue.clueTitle = title;
        clue.clueHint = hint;
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
            "Hãy thay anh chăm sóc mẹ.\nVà hãy sống tiếp thật tốt.\n\nAnh yêu hai mẹ con nhiều.",
            () => QuestManager.Instance.CompleteStep("read_brother_letter"));
    }

    void CreateFinalRecipients()
    {
        FamilyRecipient.ResetCounter();

        // 1. Chị Liên
        var pos1 = GroundSnap.Snap(finalDeliveryPosition);
        var go1 = CreateMarker("Chị Liên (Vợ lính 1)", pos1, new Color(0.9f, 0.5f, 0.7f), new Vector3(1.5f, 2f, 1.5f));
        var recipient1 = go1.AddComponent<FamilyRecipient>();
        recipient1.recipientId = "lien";
        recipient1.recipientName = "Chị Liên";
        recipient1.initialDialogue = "Anh ấy hứa sẽ trở về khi mùa lúa chín... Bức thư này là tất cả những gì anh ấy để lại sao?";
        recipient1.flashbackDialogue = "Hồi tưởng: Tiếng cười ấm áp của người lính trẻ chia tay vợ bên gốc đa làng trước khi lên đường ra trận.";
        RegisterWaypoint("final_delivery", pos1);
        SpawnHatVisual(go1.transform, new Color(0.85f, 0.78f, 0.65f), isSoldier: false);

        // 2. Ông Hùng Cũ
        var pos2 = GroundSnap.Snap(finalDeliveryPosition + new Vector3(6f, 0f, -4f));
        var go2 = CreateMarker("Ông Hùng Cũ (Cha lính 2)", pos2, new Color(0.5f, 0.6f, 0.7f), new Vector3(1.5f, 2f, 1.5f));
        var recipient2 = go2.AddComponent<FamilyRecipient>();
        recipient2.recipientId = "hung_old";
        recipient2.recipientName = "Ông Hùng Cũ";
        recipient2.initialDialogue = "Con trai tôi... Nó đã hy sinh để cứu đồng đội của nó. Thằng bé chưa bao giờ làm tôi thất vọng.";
        recipient2.flashbackDialogue = "Hồi tưởng: Cha và con trai cùng sửa sang lại mái nhà lá bị dột trước khi người con lên đường nhập ngũ.";
        SpawnHatVisual(go2.transform, new Color(0.85f, 0.78f, 0.65f), isSoldier: false);

        // 3. Mẹ Nam
        var pos3 = GroundSnap.Snap(playerSpawnPosition + new Vector3(-4f, 0f, 4f));
        var go3 = CreateMarker("Mẹ Nam", pos3, new Color(1f, 0.8f, 0.8f), new Vector3(1.5f, 2f, 1.5f));
        var recipient3 = go3.AddComponent<FamilyRecipient>();
        recipient3.recipientId = "me_nam";
        recipient3.recipientName = "Mẹ Nam";
        recipient3.initialDialogue = "Anh trai con... Trần Minh... đã hy sinh rồi sao? Đứa con tội nghiệp của mẹ... Nhưng con đã đưa thư của anh về, mẹ tự hào về con.";
        recipient3.flashbackDialogue = "Hồi tưởng: Cảnh ba mẹ con bên bữa cơm chiều đạm bạc nhưng tràn đầy tiếng cười ngày xưa.";
        SpawnHatVisual(go3.transform, new Color(0.85f, 0.78f, 0.65f), isSoldier: false);
    }

    // --- Procedural Environment Generator ---
    void GenerateEnvironment()
    {
        var zone = ForestZoneLayout.GetZone(chapterIndex);
        var center = zone.center;
        var size = zone.groundSize;

        var ground = GameObject.Find("Ground") ?? GameObject.Find("Terrain") ?? GameObject.Find("Plane");
        if (ground != null)
        {
            var r = ground.GetComponent<Renderer>();
            if (r != null) r.material = CreateURPMaterial(zone.groundColor, 1f);
        }

        switch (chapterIndex)
        {
            case 1:
                GenerateChapter1Environment(center, size);
                break;
            case 2:
                GenerateChapter2Environment(center, size);
                break;
            case 3:
                GenerateChapter3Environment(center, size);
                break;
        }
    }

    void GenerateChapter1Environment(Vector3 center, Vector3 size)
    {
        SpawnHouse(mailStationPosition + new Vector3(-6f, 0f, 4f), "House1");
        SpawnHouse(mailStationPosition + new Vector3(8f, 0f, -3f), "House2");
        SpawnHouse(elderNpcPosition + new Vector3(-6f, 0f, -4f), "House3");
        SpawnHouse(deliveryNpcPosition + new Vector3(7f, 0f, 5f), "House4");

        Color leafColor = new Color(0.18f, 0.65f, 0.25f);
        for (float x = center.x - size.x * 0.45f; x <= center.x + size.x * 0.45f; x += 6f)
        {
            SpawnTree(new Vector3(x, 0f, center.z - size.z * 0.48f + Random.Range(-1f, 1f)), leafColor, Random.Range(0.85f, 1.2f));
            SpawnTree(new Vector3(x, 0f, center.z + size.z * 0.48f + Random.Range(-1f, 1f)), leafColor, Random.Range(0.85f, 1.2f));
        }
        for (float z = center.z - size.z * 0.45f; z <= center.z + size.z * 0.45f; z += 6f)
        {
            SpawnTree(new Vector3(center.x - size.x * 0.48f + Random.Range(-1f, 1f), 0f, z), leafColor, Random.Range(0.85f, 1.2f));
            SpawnTree(new Vector3(center.x + size.x * 0.48f + Random.Range(-1f, 1f), 0f, z), leafColor, Random.Range(0.85f, 1.2f));
        }

        for (int i = 0; i < 8; i++)
        {
            Vector3 rockPos = center + new Vector3(Random.Range(-size.x * 0.35f, size.x * 0.35f), 0f, Random.Range(-size.z * 0.35f, size.z * 0.35f));
            if (Vector3.Distance(rockPos, playerSpawnPosition) > 5f && Vector3.Distance(rockPos, mailStationPosition) > 5f && Vector3.Distance(rockPos, obstacleZonePosition) > 5f)
            {
                var rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rock.name = "Rock";
                rock.transform.SetParent(chapterRoot.transform);
                rock.transform.position = GroundSnap.Snap(rockPos) + Vector3.up * 0.4f;
                rock.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                rock.transform.localScale = new Vector3(Random.Range(1f, 2.5f), Random.Range(0.8f, 2f), Random.Range(1f, 2.5f));
                var rockR = rock.GetComponent<Renderer>();
                if (rockR != null) rockR.material = CreateURPMaterial(new Color(0.48f, 0.48f, 0.5f), 1f);
            }
        }
    }

    void GenerateChapter2Environment(Vector3 center, Vector3 size)
    {
        SpawnRainShelter(rainShelterPosition);
        SpawnWatchtower(new Vector3(25f, 0f, -28f));
        SpawnWatchtower(new Vector3(33f, 0f, -18f));

        Color forestLeafColor = new Color(0.08f, 0.35f, 0.15f);
        for (int i = 0; i < 45; i++)
        {
            Vector3 treePos = center + new Vector3(Random.Range(-size.x * 0.48f, size.x * 0.48f), 0f, Random.Range(-size.z * 0.48f, size.z * 0.48f));
            bool nearSpecialPoint = false;
            Vector3[] paths = { playerSpawnPosition, soldierNpcPosition, patrolStart, patrolEnd, rainShelterPosition, motherNpcPosition };
            foreach (var p in paths)
            {
                if (Vector3.Distance(treePos, p) < 4.8f)
                {
                    nearSpecialPoint = true;
                    break;
                }
            }

            if (!nearSpecialPoint)
            {
                SpawnTree(treePos, forestLeafColor, Random.Range(1.1f, 1.6f));
            }
        }
    }

    void GenerateChapter3Environment(Vector3 center, Vector3 size)
    {
        SpawnRustedTank(new Vector3(20f, 0f, -12f));

        for (int i = 0; i < 22; i++)
        {
            Vector3 treePos = center + new Vector3(Random.Range(-size.x * 0.48f, size.x * 0.48f), 0f, Random.Range(-size.z * 0.48f, size.z * 0.48f));
            bool nearSpecial = false;
            Vector3[] specials = { playerSpawnPosition, clue1Position, clue2Position, clue3Position, finalDeliveryPosition };
            foreach (var s in specials)
            {
                if (Vector3.Distance(treePos, s) < 4f)
                {
                    nearSpecial = true;
                    break;
                }
            }

            if (!nearSpecial)
            {
                treePos = GroundSnap.Snap(treePos);
                var tree = new GameObject("BurnedTree");
                tree.transform.SetParent(chapterRoot.transform);
                tree.transform.position = treePos;

                var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                trunk.transform.SetParent(tree.transform, false);
                trunk.transform.localPosition = new Vector3(0f, 2f, 0f);
                trunk.transform.localScale = new Vector3(0.35f, 2f, 0.35f);
                var trunkR = trunk.GetComponent<Renderer>();
                if (trunkR != null) trunkR.material = CreateURPMaterial(new Color(0.12f, 0.12f, 0.12f), 1f);
            }
        }

        for (int i = 0; i < 12; i++)
        {
            Vector3 blockPos = center + new Vector3(Random.Range(-size.x * 0.45f, size.x * 0.45f), 0f, Random.Range(-size.z * 0.45f, size.z * 0.45f));
            if (Vector3.Distance(blockPos, playerSpawnPosition) > 4f && Vector3.Distance(blockPos, finalDeliveryPosition) > 4f)
            {
                var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                block.name = "Rubble";
                block.transform.SetParent(chapterRoot.transform);
                block.transform.position = GroundSnap.Snap(blockPos) + new Vector3(0f, Random.Range(0.2f, 0.6f), 0f);
                block.transform.rotation = Quaternion.Euler(Random.Range(-15, 15), Random.Range(0, 360), Random.Range(-15, 15));
                block.transform.localScale = new Vector3(Random.Range(1.5f, 3.5f), Random.Range(0.5f, 1.5f), Random.Range(1f, 2.5f));
                var blockR = block.GetComponent<Renderer>();
                if (blockR != null) blockR.material = CreateURPMaterial(new Color(0.35f, 0.35f, 0.36f), 1f);
            }
        }

        StartCoroutine(ShellingRoutine());
    }

    void SpawnTree(Vector3 pos, Color leafColor, float scaleMultiplier = 1f)
    {
        pos = GroundSnap.Snap(pos);
        var tree = new GameObject("Tree");
        tree.transform.SetParent(chapterRoot.transform);
        tree.transform.position = pos;

        var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Trunk";
        trunk.transform.SetParent(tree.transform, false);
        trunk.transform.localPosition = new Vector3(0f, 1.5f * scaleMultiplier, 0f);
        trunk.transform.localScale = new Vector3(0.35f * scaleMultiplier, 1.5f * scaleMultiplier, 0.35f * scaleMultiplier);
        var trunkR = trunk.GetComponent<Renderer>();
        if (trunkR != null) trunkR.material = CreateURPMaterial(new Color(0.35f, 0.22f, 0.12f), 1f);

        var leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaves.name = "Leaves";
        leaves.transform.SetParent(tree.transform, false);
        leaves.transform.localPosition = new Vector3(0f, 3.2f * scaleMultiplier, 0f);
        leaves.transform.localScale = new Vector3(2.5f * scaleMultiplier, 2.2f * scaleMultiplier, 2.5f * scaleMultiplier);
        var leavesR = leaves.GetComponent<Renderer>();
        if (leavesR != null) leavesR.material = CreateURPMaterial(leafColor, 1f);
    }

    void SpawnBushModel(Transform parent, Vector3 localPos, float scale = 1f)
    {
        var bush = new GameObject("BushVisual");
        bush.transform.SetParent(parent, false);
        bush.transform.localPosition = localPos;

        for (int i = 0; i < 3; i++)
        {
            var leaf = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leaf.transform.SetParent(bush.transform, false);
            leaf.transform.localPosition = new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(0.2f, 0.5f), Random.Range(-0.4f, 0.4f));
            leaf.transform.localScale = Vector3.one * Random.Range(0.9f, 1.4f) * scale;
            if (leaf.GetComponent<Collider>() != null) Destroy(leaf.GetComponent<Collider>());
            var leafR = leaf.GetComponent<Renderer>();
            if (leafR != null) leafR.material = CreateURPMaterial(new Color(0.12f, 0.5f, 0.18f), 1f);
        }
    }

    void SpawnFallenLog(Vector3 pos, float rotationY, float length)
    {
        pos = GroundSnap.Snap(pos);
        var log = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        log.name = "FallenLog";
        log.transform.SetParent(chapterRoot.transform);
        log.transform.position = pos + Vector3.up * 0.25f;
        log.transform.rotation = Quaternion.Euler(0f, rotationY, 90f);
        log.transform.localScale = new Vector3(0.5f, length * 0.5f, 0.5f);
        var logR = log.GetComponent<Renderer>();
        if (logR != null) logR.material = CreateURPMaterial(new Color(0.28f, 0.18f, 0.1f), 1f);
    }

    void SpawnHouse(Vector3 pos, string name)
    {
        pos = GroundSnap.Snap(pos);
        var house = new GameObject(name);
        house.transform.SetParent(chapterRoot.transform);
        house.transform.position = pos;

        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Walls";
        wall.transform.SetParent(house.transform, false);
        wall.transform.localPosition = new Vector3(0f, 1.2f, 0f);
        wall.transform.localScale = new Vector3(3.5f, 2.4f, 3f);
        var wallR = wall.GetComponent<Renderer>();
        if (wallR != null) wallR.material = CreateURPMaterial(new Color(0.9f, 0.86f, 0.76f), 1f);

        var roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roof.name = "Roof";
        roof.transform.SetParent(house.transform, false);
        roof.transform.localPosition = new Vector3(0f, 2.7f, 0f);
        roof.transform.localRotation = Quaternion.Euler(0f, 0f, 25f);
        roof.transform.localScale = new Vector3(2.6f, 2.6f, 3.4f);
        var roofR = roof.GetComponent<Renderer>();
        if (roofR != null) roofR.material = CreateURPMaterial(new Color(0.65f, 0.35f, 0.18f), 1f);
    }

    void SpawnRainShelter(Vector3 pos)
    {
        pos = GroundSnap.Snap(pos);
        var shelter = new GameObject("RainShelterVisual");
        shelter.transform.SetParent(chapterRoot.transform);
        shelter.transform.position = pos;

        Vector3[] offsets = { new Vector3(-2f, 0f, -2f), new Vector3(2f, 0f, -2f), new Vector3(-2f, 0f, 2f), new Vector3(2f, 0f, 2f) };
        foreach (var offset in offsets)
        {
            var pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.transform.SetParent(shelter.transform, false);
            pillar.transform.localPosition = offset + Vector3.up * 1.5f;
            pillar.transform.localScale = new Vector3(0.18f, 1.5f, 0.18f);
            var pillarR = pillar.GetComponent<Renderer>();
            if (pillarR != null) pillarR.material = CreateURPMaterial(new Color(0.35f, 0.22f, 0.12f), 1f);
        }

        var roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roof.name = "Roof";
        roof.transform.SetParent(shelter.transform, false);
        roof.transform.localPosition = new Vector3(0f, 3f, 0f);
        roof.transform.localScale = new Vector3(4.8f, 0.25f, 4.8f);
        var roofR = roof.GetComponent<Renderer>();
        if (roofR != null) roofR.material = CreateURPMaterial(new Color(0.5f, 0.4f, 0.25f), 1f);
    }

    void SpawnWatchtower(Vector3 pos)
    {
        pos = GroundSnap.Snap(pos);
        var tower = new GameObject("Watchtower");
        tower.transform.SetParent(chapterRoot.transform);
        tower.transform.position = pos;

        var frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frame.name = "Frame";
        frame.transform.SetParent(tower.transform, false);
        frame.transform.localPosition = new Vector3(0f, 2.5f, 0f);
        frame.transform.localScale = new Vector3(1.2f, 5f, 1.2f);
        var frameR = frame.GetComponent<Renderer>();
        if (frameR != null) frameR.material = CreateURPMaterial(new Color(0.2f, 0.2f, 0.22f), 1f);

        var platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.name = "Platform";
        platform.transform.SetParent(tower.transform, false);
        platform.transform.localPosition = new Vector3(0f, 5f, 0f);
        platform.transform.localScale = new Vector3(2.5f, 0.3f, 2.5f);
        var platformR = platform.GetComponent<Renderer>();
        if (platformR != null) platformR.material = CreateURPMaterial(new Color(0.35f, 0.35f, 0.38f), 1f);

        var lightGo = new GameObject("WatchtowerLight");
        lightGo.transform.SetParent(tower.transform, false);
        lightGo.transform.localPosition = new Vector3(0f, 5.5f, 0f);
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Spot;
        light.color = new Color(1f, 0.95f, 0.8f);
        light.intensity = 4.5f;
        light.range = 25f;
        light.spotAngle = 35f;

        var rotVisual = lightGo.AddComponent<FlashlightVisual>();
        rotVisual.beamLength = 15f;
        rotVisual.beamWidth = 4f;
        rotVisual.sweepAngle = 35f;
        rotVisual.sweepSpeed = 0.8f;
    }

    void SpawnRustedTank(Vector3 pos)
    {
        pos = GroundSnap.Snap(pos);
        var tank = new GameObject("RustedTank");
        tank.transform.SetParent(chapterRoot.transform);
        tank.transform.position = pos;

        var chassis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        chassis.name = "Chassis";
        chassis.transform.SetParent(tank.transform, false);
        chassis.transform.localPosition = new Vector3(0f, 0.6f, 0f);
        chassis.transform.localScale = new Vector3(3f, 1f, 5f);
        var chassisR = chassis.GetComponent<Renderer>();
        if (chassisR != null) chassisR.material = CreateURPMaterial(new Color(0.42f, 0.28f, 0.2f), 1f);

        var turret = GameObject.CreatePrimitive(PrimitiveType.Cube);
        turret.name = "Turret";
        turret.transform.SetParent(tank.transform, false);
        turret.transform.localPosition = new Vector3(0f, 1.4f, -0.4f);
        turret.transform.localScale = new Vector3(1.8f, 0.8f, 2.2f);
        var turretR = turret.GetComponent<Renderer>();
        if (turretR != null) turretR.material = CreateURPMaterial(new Color(0.38f, 0.25f, 0.18f), 1f);

        var barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        barrel.name = "Barrel";
        barrel.transform.SetParent(tank.transform, false);
        barrel.transform.localPosition = new Vector3(0f, 1.4f, 1.8f);
        barrel.transform.localRotation = Quaternion.Euler(85f, 0f, 0f);
        barrel.transform.localScale = new Vector3(0.25f, 1.8f, 0.25f);
        var barrelR = barrel.GetComponent<Renderer>();
        if (barrelR != null) barrelR.material = CreateURPMaterial(new Color(0.3f, 0.2f, 0.15f), 1f);
    }

    void SpawnHatVisual(Transform npcTransform, Color color, bool isSoldier)
    {
        var hat = GameObject.CreatePrimitive(isSoldier ? PrimitiveType.Sphere : PrimitiveType.Cylinder);
        hat.name = "HatVisual";
        hat.transform.SetParent(npcTransform, false);
        
        if (isSoldier)
        {
            hat.transform.localPosition = new Vector3(0f, 2f, 0f);
            hat.transform.localScale = new Vector3(0.9f, 0.45f, 0.9f);
        }
        else // Conical hat (Nón lá) simulation
        {
            hat.transform.localPosition = new Vector3(0f, 2.05f, 0f);
            hat.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            hat.transform.localScale = new Vector3(1.1f, 0.12f, 1.1f);
            
            // Conical tip
            var tip = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tip.transform.SetParent(hat.transform, false);
            tip.transform.localPosition = new Vector3(0f, 0.8f, 0f);
            tip.transform.localScale = new Vector3(0.3f, 1f, 0.3f);
            Destroy(tip.GetComponent<Collider>());
            var tipR = tip.GetComponent<Renderer>();
            if (tipR != null) tipR.material = CreateURPMaterial(color, 1f);
        }

        Destroy(hat.GetComponent<Collider>());
        var r = hat.GetComponent<Renderer>();
        if (r != null) r.material = CreateURPMaterial(color, 1f);
    }

    IEnumerator ShellingRoutine()
    {
        while (chapterIndex == 3)
        {
            yield return new WaitForSeconds(Random.Range(4f, 7f));

            if (QuestManager.Instance == null || !QuestManager.Instance.IsStepActive("cross_danger"))
                continue;

            var player = GameManager.Instance?.player;
            if (player == null) continue;

            var dangerCenter = ForestZoneLayout.Ch3DangerZone;
            if (Vector3.Distance(player.position, dangerCenter) > 22f)
                continue;

            Vector3 targetPos = player.position + new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
            targetPos = GroundSnap.Snap(targetPos);

            var warning = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            warning.name = "ArtilleryWarning";
            warning.transform.SetParent(chapterRoot.transform);
            warning.transform.position = targetPos + new Vector3(0f, 0.05f, 0f);
            warning.transform.localScale = new Vector3(4.5f, 0.02f, 4.5f);
            Destroy(warning.GetComponent<Collider>());
            var warningR = warning.GetComponent<Renderer>();
            if (warningR != null) warningR.material = CreateURPMaterial(new Color(1f, 0f, 0f, 0.35f), 0.35f);

            GameUI.Instance?.ShowNotification("⚠ Cảnh báo pháo kích! Tránh xa vòng đỏ!", 1.5f);

            yield return new WaitForSeconds(2f);

            if (warning != null) Destroy(warning);

            var explosion = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            explosion.name = "ExplosionVisual";
            explosion.transform.SetParent(chapterRoot.transform);
            explosion.transform.position = targetPos + Vector3.up * 1.5f;
            explosion.transform.localScale = Vector3.one * 0.5f;
            Destroy(explosion.GetComponent<Collider>());
            var expR = explosion.GetComponent<Renderer>();
            if (expR != null) expR.material = CreateURPMaterial(new Color(1f, 0.45f, 0.1f, 0.95f), 0.95f);

            float elapsed = 0f;
            bool hitPlayer = false;
            while (elapsed < 0.4f)
            {
                if (explosion == null) break;
                float t = elapsed / 0.4f;
                explosion.transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one * 7f, t);
                
                if (!hitPlayer && player != null && Vector3.Distance(player.position, targetPos) <= 3.8f)
                {
                    hitPlayer = true;
                    GameUI.Instance?.ShowNotification("Bị trúng pháo kích! Rút lui về checkpoint.", 3f);
                    GameManager.Instance?.TeleportPlayer(ForestZoneLayout.Ch3DangerReset);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (explosion != null) Destroy(explosion);
        }
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

    GameObject CreateMarker(string name, Vector3 pos, Color color, Vector3 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(chapterRoot.transform);
        go.transform.position = GroundSnap.Snap(pos);

        var col = go.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = Mathf.Max(size.x, size.z) * 0.55f;
        col.center = new Vector3(0f, 1.2f, 0f);

        CreateSmallIndicator(go.transform, color);
        CreateObjectiveLabel(go.transform, name);
        CreateBeaconLight(go.transform, color);
        return go;
    }

    void CreateObjectiveLabel(Transform parent, string label)
    {
        var canvasGo = new GameObject("ObjectiveLabel");
        canvasGo.transform.SetParent(parent);
        canvasGo.transform.localPosition = new Vector3(0f, 3.2f, 0f);

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasGo.AddComponent<FaceCamera>();
        var rt = canvasGo.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(420, 72);
        rt.localScale = Vector3.one * 0.018f;

        var textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
        textGo.transform.SetParent(canvasGo.transform, false);
        var textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        var t = textGo.GetComponent<Text>();
        t.font = CrispUiText.GetFont();
        t.fontSize = 32;
        t.fontStyle = FontStyle.Bold;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.text = label;
        t.raycastTarget = false;
        CrispUiText.WarmAtlas(26);
    }

    void CreateSmallIndicator(Transform parent, Color color)
    {
        var baseRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        baseRing.name = "BaseRing";
        baseRing.transform.SetParent(parent);
        baseRing.transform.localPosition = new Vector3(0f, 0.05f, 0f);
        baseRing.transform.localScale = new Vector3(1.6f, 0.04f, 1.6f);
        if (baseRing.GetComponent<Collider>() != null) Destroy(baseRing.GetComponent<Collider>());
        var baseR = baseRing.GetComponent<Renderer>();
        if (baseR != null) baseR.material = CreateURPMaterial(color, 0.55f);

        var pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.name = "Indicator";
        pole.transform.SetParent(parent);
        pole.transform.localPosition = new Vector3(0f, 1.6f, 0f);
        pole.transform.localScale = new Vector3(0.45f, 1.6f, 0.45f);

        var col = pole.GetComponent<Collider>();
        if (col != null) Destroy(col);

        var renderer = pole.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material = CreateURPMaterial(color, 1f);

        var ring = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ring.name = "Glow";
        ring.transform.SetParent(parent);
        ring.transform.localPosition = new Vector3(0f, 3.3f, 0f);
        ring.transform.localScale = Vector3.one * 0.7f;
        if (ring.GetComponent<Collider>() != null) Destroy(ring.GetComponent<Collider>());
        var ringR = ring.GetComponent<Renderer>();
        if (ringR != null)
            ringR.material = CreateURPMaterial(color, 1f);
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
        DialogueManager.Instance?.ShowDialogue("Ông Hùng", "Cậu đã trở về! Và đã giao lá thư đầu tiên thành công chứ?", () =>
        {
            DialogueManager.Instance?.ShowDialogue("Nam", "Dạ rồi ạ. Nhìn thấy nụ cười của anh Sơn khi nhận được tin vui từ gia đình, em hiểu công việc này có ý nghĩa thế nào.", () =>
            {
                DialogueManager.Instance?.ShowDialogue("Ông Hùng", "Tốt lắm. Mỗi bức thư là một tia hy vọng. Từ hôm nay, cậu chính thức là người đưa thư của chúng tôi.", () =>
                {
                    GameManager.Instance?.CompleteChapter(1);
                });
            });
        });
    }

    void OnChapter2Complete()
    {
        DialogueManager.Instance?.ShowDialogue("Bà Lan", "Con ta... con ta không trở về nữa...", () =>
        {
            DialogueManager.Instance?.ShowDialogue("Nam", "Chiến tranh thật tàn khốc.", () =>
                GameManager.Instance?.CompleteChapter(2));
        });
    }

    void OnChapter3Complete()
    {
        DialogueManager.Instance?.ShowDialogue("Nam", "Chiến tranh đã qua, nhưng những lá thư và hy vọng này sẽ sống mãi.", () =>
        {
            GameManager.Instance?.CompleteChapter(3);
        });
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
