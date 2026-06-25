# 📖 NGƯỜI ĐƯA THƯ - GAME PROJECT INDEX

## 📌 TÓM TẮT PROJECT

**Game Title:** Người Đưa Thư (The Letter Carrier)  
**Genre:** Story-driven Adventure / Stealth Action  
**Platform:** PC (Windows/Mac/Linux)  
**Engine:** Unity 2022 LTS+  
**Development Time:** 15-20 weeks  
**Team Size:** 3-5 people  

---

## 🎯 CORE CONCEPT

Một trò chơi lấy bối cảnh chiến tranh Việt Nam, nơi người chơi vào vai Nam - một thanh niên 19 tuổi nhận nhiệm vụ vận chuyển thư giữa các làng. Thay vì tham gia chiến đấu trực tiếp, Nam chọn con đường bảo tồn hy vọng của hàng ngàn con người.

**Theme:** "Chiến tranh chia cắt con người, nhưng hy vọng luôn tìm được đường để đến nơi cần đến."

---

## 📚 DOCUMENTATION FILES

### 1. **NGUOI_DUA_THU_GDD.md** - Game Design Document
**Nội Dung:**
- Tổng quan game (concept, theme, target audience)
- Nhân vật chính: Nam, Mẹ, Anh Trai, các NPC quan trọng
- 3 chương chi tiết: bối cảnh, cốt truyện, nhiệm vụ
- Môi trường & địa điểm (làng, rừng, chiến trường)
- Level progression & emotional arc
- Visual style & art direction

**Khi Nào Dùng:**
- Khi cần hiểu toàn bộ vision của game
- Tham khảo để tạo cutscene, dialogue
- Xác định art direction, music style

---

### 2. **GAMEPLAY_MECHANICS.md** - Technical Mechanics Document
**Nội Dung:**
- Hệ thống di chuyển (walk, run, sprint, jump, stamina)
- Hệ thống stealth (detection, guards, alarm)
- Hệ thống quest (types, structure, journal)
- Nhân vật stats & progression
- Dialogue system architecture
- Save/Load implementation

**Khi Nào Dùng:**
- Khi coding các system chính
- Reference cho gameplay balance
- Understanding detection radius, stealth mechanics

---

### 3. **UNITY_SCRIPT_TEMPLATES.cs** - C# Code Templates
**Nội Dung:**
- 8 main C# classes:
  1. PlayerController - Di chuyển & input
  2. QuestManager - Quản lý nhiệm vụ
  3. DialogueSystem - Hệ thống hội thoại
  4. NPCController - NPC behavior
  5. MailBag - Hệ thống thư/item
  6. GameManager - Game flow
  7. GuardAI - Stealth guards (Chapter 2)
  8. SaveSystem - Lưu/tải game

**Khi Nào Dùng:**
- Copypaste vào Unity project
- Thêm vào Assets/Scripts/ folders
- Modify code theo project requirements

---

### 4. **UNITY_QUICK_START.md** - Setup & Implementation Guide
**Nội Dung:**
- Project requirements & packages
- Folder structure setup
- Step-by-step implementation (Phase 1-7)
- 29 detailed steps over 15 weeks
- Testing checklist
- Build & deployment instructions

**Khi Nào Dùng:**
- Bắt đầu Unity project
- Team orientation & schedules
- Troubleshooting common issues

---

## 🗺️ PROJECT ROADMAP

### PHASE 1: PROJECT INIT (Week 1)
- [ ] Create Unity project
- [ ] Import Input System
- [ ] Setup folder structure
- [ ] Create base Player prefab
- [ ] Create MainMenu scene
- [ ] Setup Game Manager

### PHASE 2: CORE SYSTEMS (Week 2-3)
- [ ] Player controller with stamina
- [ ] Camera system with Cinemachine
- [ ] Quest manager
- [ ] Save/Load system

### PHASE 3: DIALOGUE & NPCs (Week 4)
- [ ] Dialogue UI & system
- [ ] NPC prefab
- [ ] Letter/mail system
- [ ] Basic interaction

### PHASE 4: CHAPTER 1 (Week 5-8)
- [ ] Build village environment
- [ ] Create 4 main quests
- [ ] Implement terrain navigation puzzles
- [ ] Audio & music
- [ ] Chapter 1 cutscenes

### PHASE 5: CHAPTER 2 (Week 9-11)
- [ ] Guard AI & detection
- [ ] Stealth mechanics
- [ ] Forest environment
- [ ] Weather system
- [ ] Emotional scenes

### PHASE 6: CHAPTER 3 (Week 12-13)
- [ ] Bunker exploration
- [ ] Letter finding mechanic
- [ ] Journal system
- [ ] Final cutscenes

### PHASE 7: POLISH (Week 14-15)
- [ ] UI & UX refinement
- [ ] Performance optimization
- [ ] Full playthrough testing
- [ ] Bug fixes
- [ ] Build & deploy

---

## 🎮 GAMEPLAY SUMMARY

### CHAPTER 1: Con Đường Hy Vọng (15-20 min)
- **Mechanics:** Exploration, navigation, platforming
- **Enemies:** None
- **Tone:** Hopeful, introductory
- **Tasks:** Learn movement, deliver first letters

### CHAPTER 2: Bóng Tối Chiến Tranh (20-25 min)
- **Mechanics:** Stealth, evasion, survival
- **Enemies:** Guard patrols
- **Tone:** Tense, emotional
- **Boss:** Rain storm environmental challenge

### CHAPTER 3: Lá Thư Cuối Cùng (25-30 min)
- **Mechanics:** Exploration, investigation, emotional resolution
- **Enemies:** None (post-war)
- **Tone:** Bittersweet, hopeful ending
- **Tasks:** Find brother's letter, deliver final letters

---

## 👥 CHARACTER QUICK REFERENCE

| Name | Age | Role | Status |
|------|-----|------|--------|
| **Nam** | 19 | Protagonist | Alive |
| **Mẹ Nam** | 50 | Support NPC | Alive |
| **Trần Minh** | 24 | Anh Trai | Deceased |
| **Ông Hùng** | 50 | Quest Giver | Alive |
| **Bà Lan** | 50 | NPC Recipient | Alive |
| **Lính Trẻ** | 21 | Story Catalyst | Deceased (Ch.2) |
| **Anh Sơn** | 35 | Early Quest | Alive |
| **Bà Liên** | 65 | NPC Vendor | Alive |

---

## 🔑 KEY FEATURES

✅ **Emotional Story-Driven Narrative**
- 3 interconnected chapters
- Character development through choices
- Vietnamese historical context

✅ **Unique Gameplay Loop**
- Deliver letters (core mechanic)
- Explore diverse environments
- Stealth-focused Chapter 2
- Exploration-focused Chapter 3

✅ **Dynamic Systems**
- Quest system with branching objectives
- Character stat progression
- Dialogue choices with consequences
- Weather & time system

✅ **Atmospheric Experience**
- Vietnamese culture & language
- Period-accurate 1960s-70s setting
- Emotional soundtrack
- Voice acting

---

## 💾 SAVE SYSTEM

```json
{
  "chapter": 1,
  "playerPosition": {x, y, z},
  "questsCompleted": ["q_deliver_letters_ch1"],
  "characterStats": {
    "determination": 50,
    "compassion": 50,
    "courage": 40,
    "wisdom": 30
  },
  "inventory": ["letter_1", "letter_2"],
  "gameTime": "14:30"
}
```

Multiple save slots support.

---

## 🎵 AUDIO SPECIFICATION

### Music
- Chapter 1 Theme: Soft piano + đàn tranh (traditional)
- Chapter 2 Theme: Tense strings, minimal notes
- Chapter 3 Theme: Ambient, reflective
- Menu Theme: Hopeful melody

### Sound Effects
- Footsteps (grass, concrete, dirt, water)
- Ambient (birds, wind, distant gunfire)
- UI sounds (click, complete, alert)
- Weather (rain, thunder, lightning)
- Guard alerts & combat

### Voice Acting
- Main characters voiced in Vietnamese
- Dialogue & monologues
- Character personalities through voice

---

## 🎨 VISUAL STYLE

**Color Palettes:**
- **Chapter 1:** Warm oranges & browns (hope)
- **Chapter 2:** Cool blues & grays (fear)
- **Chapter 3:** Desaturated + green (recovery)

**Technical:**
- Camera: Third-person, cinematic angles
- Lighting: Dynamic shadows, volumetric fog
- Post-Processing: Film grain (period feel)
- Art Style: Realistic 3D with painterly effects

---

## 🛠️ TECHNICAL SPECS

- **Engine:** Unity 2022 LTS
- **Scripting:** C# 9.0+
- **Target Resolution:** 1920x1080 @ 60 FPS
- **Platforms:** Windows, macOS, Linux
- **Minimum RAM:** 2GB
- **Storage:** ~5GB SSD
- **Input:** Keyboard only (design choice - letter carrying, not combat)

---

## 📋 IMPLEMENTATION CHECKLIST

### PRE-DEVELOPMENT
- [ ] Read all documentation files
- [ ] Gather/create 3D models
- [ ] Record voice acting
- [ ] Compose music tracks
- [ ] Create art assets

### DEVELOPMENT (Per Phase)
- [ ] Implement core systems
- [ ] Create environments
- [ ] Populate NPCs & quests
- [ ] Record cutscenes
- [ ] Integrate audio
- [ ] Build & test

### QA TESTING
- [ ] Functionality tests (all mechanics working)
- [ ] Compatibility tests (different machines)
- [ ] Balance tests (difficulty tuning)
- [ ] Performance tests (FPS consistency)
- [ ] Localization tests (Vietnamese text)

### PRE-LAUNCH
- [ ] Final polish pass
- [ ] Performance optimization
- [ ] Bug fixes
- [ ] Documentation review
- [ ] Build for distribution

---

## 🎯 SUCCESS CRITERIA

Game is considered complete when:

1. ✅ All 3 chapters playable without crashes
2. ✅ Story coherent & emotionally resonant
3. ✅ All quests completable
4. ✅ All dialogue displays correctly
5. ✅ Stealth mechanics functional
6. ✅ Consistent 60 FPS performance
7. ✅ No major bugs
8. ✅ Vietnamese language properly displayed
9. ✅ Audio/music fully integrated
10. ✅ Playtime: 60-75 minutes (estimated)

---

## 🚀 QUICK START STEPS

1. **Read NGUOI_DUA_THU_GDD.md** - Understand the full game vision
2. **Read GAMEPLAY_MECHANICS.md** - Learn technical details
3. **Read UNITY_QUICK_START.md** - Follow setup guide
4. **Copy UNITY_SCRIPT_TEMPLATES.cs** into your project
5. **Follow Phase 1-7 Implementation schedule**
6. **Test & iterate based on documentation**

---

## 📞 CONTACT & NOTES

**Project Status:** Design Phase Complete  
**Last Updated:** June 2026  
**Version:** 1.0

This documentation provides a complete roadmap for bringing "Người Đưa Thư" to life on Unity. 

**Note:** All estimated timeframes are subject to team size, experience level, and available resources. Adjust phases as needed for your situation.

---

## 📁 FILE ORGANIZATION

```
Assets/
├── NGUOI_DUA_THU_GDD.md ← START HERE (vision & story)
├── GAMEPLAY_MECHANICS.md ← Read for systems understanding
├── UNITY_QUICK_START.md ← Implementation guide
├── UNITY_SCRIPT_TEMPLATES.cs ← Copy to Scripts/
└── README.md (this file) ← Overview
```

---

## 🎓 LEARNING PATHS

### For Game Designers
1. Read GDD thoroughly
2. Map out all NPCs & dialogue
3. Plan quest progression
4. Design level layouts

### For Programmers
1. Review GAMEPLAY_MECHANICS.md
2. Study UNITY_SCRIPT_TEMPLATES.cs
3. Follow UNITY_QUICK_START.md phases
4. Implement systems sequentially

### For Artists
1. Reference art direction in GDD
2. Exploit environment descriptions
3. Create models for all locations
4. Design UI layout

### For Sound Designers
1. Note music descriptions in GDD
2. List SFX requirements
3. Coordinate with voice actors
4. Create audio implementation plan

---

**Good luck with development! 🎮✨**

By completing this project, you'll create an emotionally impactful game about humanity, hope, and the true cost of war - told through the eyes of a letter carrier.

*"Chiến tranh chia cắt con người, nhưng hy vọng luôn tìm được đường để đến nơi cần đến."*
