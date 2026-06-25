# 🛠️ QUICK START GUIDE - UNITY SETUP

## 📋 PRE-SETUP REQUIREMENTS

### Hardware Minimum
- Unity 2022 LTS hoặc cao hơn
- C# khả năng cơ bản
- Visual Studio hoặc Rider
- 5GB disk space

### Unity Packages Cần Cài Đặt
```
Window → TextMeshPro → Import TMP Essential Resources
Window → Package Manager:
  - Input System (NEW)
  - Timeline
  - Cinemachine 2.x
  - Animator 2.0+
  - ProBuilder (optional, for level design)
```

---

## 🎮 PROJECT STRUCTURE

### Folders to Create

Tạo các folder này trong Assets/:

```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   ├── PlayerInput.cs
│   │   └── PlayerStats.cs
│   ├── Quest/
│   │   ├── QuestManager.cs
│   │   ├── Quest.cs
│   │   └── QuestUI.cs
│   ├── Dialogue/
│   │   ├── DialogueSystem.cs
│   │   ├── DialogueManager.cs
│   │   └── DialogueData.cs
│   ├── NPC/
│   │   ├── NPCController.cs
│   │   ├── NPCData.cs
│   │   └── NPCBehavior.cs
│   ├── Items/
│   │   ├── MailBag.cs
│   │   ├── Letter.cs
│   │   └── ItemPickup.cs
│   ├── Managers/
│   │   ├── GameManager.cs
│   │   ├── SaveSystem.cs
│   │   ├── InputManager.cs
│   │   └── EventManager.cs
│   ├── Stealth/
│   │   ├── GuardAI.cs
│   │   ├── DetectionSystem.cs
│   │   └── StealthZone.cs
│   ├── UI/
│   │   ├── UIManager.cs
│   │   ├── InventoryUI.cs
│   │   ├── JournalUI.cs
│   │   └── PauseMenuUI.cs
│   ├── Audio/
│   │   ├── AudioManager.cs
│   │   └── SoundPool.cs
│   └── Utils/
│       ├── CameraBob.cs
│       ├── CinematicController.cs
│       └── WeatherSystem.cs
├── Scenes/
│   ├── MainMenu.unity
│   ├── Chapter_1.unity
│   ├── Chapter_2.unity
│   └── Chapter_3.unity
├── Prefabs/
│   ├── NPCs/
│   ├── UI/
│   ├── Particles/
│   └── Enemies/
├── Assets/
│   ├── Models/
│   ├── Animations/
│   ├── UI/
│   └── Cutscenes/
├── Audio/
│   ├── Music/
│   ├── SFX/
│   └── VO/ (voice over)
├── Materials/
│   └── (Materials and Shaders)
├── Resources/
│   ├── Dialogue/
│   ├── Quests/
│   └── Data/
├── Save/
│   └── (Save files)
└── Settings/
    └── (Project settings)
```

---

## ⚙️ UNITY SETTINGS CONFIGURATION

### Player Settings
```
File → Build Settings:
- Platform: PC, Mac & Linux Standalone
- Resolution: 1920x1080
- Aspect Ratio: 16:9
- Quality: High
- V-Sync: On
```

### Scene Setup

#### Main Menu Scene (MainMenu.unity)
```
Hierarchy:
├── Canvas (UI)
│   ├── Background (Image)
│   ├── TitleText (TextMeshPro)
│   ├── ButtonContainer
│   │   ├── NewGameButton
│   │   ├── ContinueButton
│   │   ├── SettingsButton
│   │   └── ExitButton
│   └── CreditsText
├── AudioListener
└── Background Music (AudioSource)
```

#### Game Scene Template (Chapter_X.unity)
```
Hierarchy:
├── Player
│   ├── PlayerController
│   ├── PlayerAnimator
│   ├── Camera (CinemachineFreeLook)
│   └── MailBag
├── Environment
│   ├── Terrain
│   ├── Buildings
│   ├── Props
│   └── Hazards
├── NPCs
│   ├── NPC_1 (Prefab Instance)
│   ├── NPC_2 (Prefab Instance)
│   └── ...
├── UI
│   ├── HUD Canvas
│   │   ├── MailBag Indicator
│   │   ├── TimeDisplay
│   │   └── WeatherIcon
│   └── Pause Menu Canvas
├── Managers
│   ├── GameManager
│   ├── QuestManager
│   ├── DialogueSystem
│   └── AudioManager
├── Lighting
│   ├── Directional Light (Sun)
│   └── Ambient Settings
└── Audio
    ├── BG Music (AudioSource)
    └── SFX Pool (AudioSource array)
```

---

## 📌 STEP-BY-STEP SETUP GUIDE

### PHASE 1: PROJECT INITIALIZATION (Week 1)

#### Step 1: Create Project
```
Unity Hub → New Project
- Template: 3D (Core)
- Project Name: Game_Project_PRU
- Location: Your workspace
```

#### Step 2: Import Input System
```
Window → Package Manager → Input System
Import the NEW INPUT SYSTEM
Delete the old InputManager if asked
```

#### Step 3: Create Folder Structure
```
In Assets/:
- Run create_folder.bat (or manually create folders as shown above)
```

#### Step 4: Create Player Prefab
```
1. Create empty GameObject: "Player"
2. Add Components:
   - Rigidbody (mass: 1, drag: 5, use gravity: ON)
   - Capsule Collider (height: 1.8, radius: 0.3)
   - PlayerController.cs
   - Animator (with humanoid avatar)
3. Child: CameraHolder → Main Camera
4. Drag to Prefabs/Player
```

#### Step 5: Input System Setup
```
Create InputActions Asset:
- Assets → Create → Input Actions
- Name: "InputActions"
- Add actions:
  * Move (Value: Vector2)
  * Sprint (Button: Shift)
  * Jump (Button: Space)
  * Interact (Button: E)
  * Pause (Button: Esc)
- Save and Enable the asset
```

#### Step 6: Create Main Scene
```
1. Scene → New Scene (Save as "MainMenu")
2. Create UICanvas
3. Add buttons for New Game, Load, Settings, Exit
4. Save scene
```

---

### PHASE 2: CORE SYSTEMS (Week 2-3)

#### Step 7: Implement Player Controller
```
1. Copy PlayerController.cs from templates
2. Attach to Player prefab
3. Setup:
   - Ground Layer: Create "Ground" layer
   - Assign to terrain objects
   - Test basic movement
   - Adjust speeds/acceleration
```

#### Step 8: Implement Camera
```
1. Install Cinemachine from Package Manager
2. Add CinemachineFreeLook to Player
3. Setup target: Player head position
4. Adjust follow/look speeds
5. Add CameraBob effect
```

#### Step 9: Create Game Manager
```
1. Create empty GameObject: "GameManager"
2. Attach GameManager.cs
3. Setup:
   - Set canvas targets
   - Link scene references
   - Create save system
4. Make DontDestroyOnLoad
```

#### Step 10: Implement Quest System
```
1. Attach QuestManager.cs to GameManager
2. Create quest database:
   - ScriptableObject for quests
   - Populate with Chapter 1 quests
3. Test quest updates
```

---

### PHASE 3: DIALOGUE & NPCs (Week 4)

#### Step 11: Dialogue System
```
1. Create Canvas for Dialogue UI
2. Add TextMeshPro fields:
   - Character Name
   - Dialogue Text
   - Options Panel
3. Implement DialogueSystem.cs
4. Test with sample dialogue
```

#### Step 12: NPC Setup
```
1. Create NPC Prefab:
   - 3D Model (humanoid)
   - Animator
   - Capsule Collider (trigger)
   - NPCController.cs
2. Add to scene, customize per NPC
3. Assign dialogue & quests
```

#### Step 13: Letter/Mail System
```
1. Implement MailBag.cs
2. Create Letter prefab
3. Test pickup/delivery mechanics
4. Link to quest completion
```

---

### PHASE 4: CHAPTER 1 CONTENT (Week 5-8)

#### Step 14: Build Chapter 1 Scene
```
1. Create Chapter_1.unity scene
2. Build environment:
   - Terrain (Unity terrain or modeled)
   - Houses (props/models)
   - Path to Binh An village
3. Place NPCs from prefabs
4. Setup lighting (daytime)
```

#### Step 15: Chapter 1 Quests
```
1. Create 4 main quests:
   - Get letter from Trạm Liên Lạc
   - Navigate terrain
   - Deliver to Binh An
   - Return to base
2. Link quest triggers to NPCs
3. Create rewards system
```

#### Step 16: Audio Setup
```
1. Create AudioManager.cs
2. Add background music to Chapter 1
3. Add footstep SFX
4. Create audio pool for dynamic sounds
5. Test audio transitions
```

#### Step 17: Cutscenes
```
1. Use Timeline for intro/outro
2. Create cutscene for "letter significance"
3. Record animations for key moment
```

---

### PHASE 5: CHAPTER 2 - STEALTH (Week 9-11)

#### Step 18: Guard AI
```
1. Implement GuardAI.cs
2. Create Guard prefab
3. Setup patrol routes (waypoints)
4. Test detection mechanics
```

#### Step 19: Detection System
```
1. Implement DetectionSystem.cs
2. Add vision cone visualization
3. Test player detection/evasion
4. Tune detection ranges
```

#### Step 20: Build Chapter 2 Scene
```
1. Create Chapter_2.unity
2. Build forest environment
3. Add guard patrols
4. Create hiding spots (bushes, shadows)
5. Setup weather system
```

#### Step 21: Weather System
```
1. Create WeatherSystem.cs
2. Implement rain effects:
   - Particle system for rain
   - Visual blur effect
   - Sound effects
   - Gameplay mechanic (shelter)
```

#### Step 22: Emotional Scenes
```
1. Create Cutscene for "young soldier meeting"
2. Record important dialogue
3. Implement emotional stat changes
```

---

### PHASE 6: CHAPTER 3 - EXPLORATION (Week 12-13)

#### Step 23: Build Chapter 3 Scene
```
1. Create Chapter_3.unity
2. Build ruined bunker environment
3. Add exploration areas:
   - Destroyed bunker
   - Trenches
   - Memorial site
4. Scatter documents/letters
```

#### Step 24: Letter Finding System
```
1. Create FindLetterSystem.cs
2. Implement highlight/examine mechanics
3. Create journal system for collected letters
```

#### Step 25: Final Cutscenes
```
1. Create "finding brother's letter" cutscene
2. Implement emotional peak scene
3. Create ending credits roll
```

---

### PHASE 7: POLISH & TESTING (Week 14-15)

#### Step 26: UI Polish
```
1. Create pause menu
2. Create settings menu
3. Create inventory/journal UI
4. Add HUD elements
```

#### Step 27: Performance
```
1. Profile with Profiler
2. Optimize:
   - Draw calls
   - Memory usage
   - Physics calculations
3. Target: 60 FPS consistent
```

#### Step 28: Full Playthrough
```
1. Play through all chapters
2. Test all quests
3. Test dialogue branches
4. Check audio/visual consistency
5. Document bugs
```

#### Step 29: Bug Fixes
```
1. Fix reported issues
2. Balance difficulty
3. Tune audio levels
4. Check text for Vietnamese language
```

---

## 🎨 ASSETS NEEDED

### 3D Models
- [ ] Player character (Nam)
- [ ] NPCs (Ông Hùng, Bà Lan, etc.)
- [ ] Environments (village, forest, bunker)
- [ ] Props (houses, trees, etc.)

### Animations
- [ ] Walk/Run/Sprint
- [ ] Jump/Fall
- [ ] Idle animations
- [ ] Interaction animations (pick up letter)
- [ ] Death/falling animations
- [ ] Dialogue animations

### Audio
- [ ] Background music (3 chapters)
- [ ] Footstep sounds (grass, concrete, dirt)
- [ ] Ambient sounds (birds, wind, rain)
- [ ] Voice acting (characters)
- [ ] UI sounds (click, complete)
- [ ] Weather effects (thunder, rain)

### Graphics
- [ ] UI Sprites
- [ ] Particle effects (rain, dust)
- [ ] Shader/Post-processing

---

## 🧪 TESTING CHECKLIST

### Functionality Tests
- [ ] Player can move in all directions
- [ ] Player can jump
- [ ] Camera follows player smoothly
- [ ] Stamina system works
- [ ] Quests track properly
- [ ] NPCs are interactive
- [ ] Letters can be picked up/delivered
- [ ] Dialogue displays correctly

### Chapter 1 Tests
- [ ] Terrain navigation works
- [ ] All NPCs present and functional
- [ ] Quest progression logical
- [ ] Cutscenes play correctly
- [ ] Audio synced

### Chapter 2 Tests
- [ ] Stealth mechanics work
- [ ] Guards detect player correctly
- [ ] Weather system active
- [ ] Shelter mechanic works
- [ ] Mission fails on detection

### Chapter 3 Tests
- [ ] Exploration works smoothly
- [ ] Letters can be found
- [ ] Journal updates properly
- [ ] Final cutscene plays
- [ ] Credits roll

---

## 🚀 BUILD & DEPLOYMENT

### Build Settings
```
File → Build Settings:
1. Add all Scenes to build:
   - MainMenu
   - Chapter_1
   - Chapter_2
   - Chapter_3
2. Set MainMenu as first scene
3. Configure for Windows/Mac/Linux
```

### Creating Build
```
File → Build Settings → Build
- Create Builds/ folder
- Output: Game_Project_PRU.exe (Windows)
- Test on another PC for compatibility
```

---

## 📞 TROUBLESHOOTING

### Player Not Moving
- Check Input System is imported
- Verify InputActions asset assigned
- Check Rigidbody settings (not kinematic)

### Dialogue Not Showing
- Verify Canvas has Graphics Raycaster
- Check TextMeshPro resources imported
- Confirm DialogueSystem in scene

### NPCs Not Interacting
- Check NPC has collider set to trigger
- Verify NPCController script attached
- Confirm interaction key mapped

### Performance Issues
- Check Profiler for bottlenecks
- Reduce polygon count on models
- Batch UI rendering
- Use object pooling for effects

---

## 📚 LEARNING RESOURCES

- Unity Documentation: docs.unity.com
- TextMeshPro: docs.unity.com/Packages/com.unity.textmeshpro
- Input System: docs.unity.com/Packages/com.unity.inputsystem
- Timeline: docs.unity.com/Packages/com.unity.timeline
- C# Programming: docs.microsoft.com/en-us/dotnet/csharp/

---

**Happy Developing! 🎮**

Bắt đầu từ PHASE 1 và tiến dần theo schedule.
Mỗi phase xong hãy test trước khi move to next phase.
