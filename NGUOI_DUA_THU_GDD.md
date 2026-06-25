# 👨‍💼 NGƯỜI ĐƯA THƯ - GAME DESIGN DOCUMENT

**Game Title:** Người Đưa Thư (The Letter Carrier)  
**Genre:** Story-driven Adventure / Stealth Action  
**Platform:** PC / Unity  
**Target Audience:** Ages 13+  
**Setting:** Wartime Vietnam, 1960s-1970s

---

## 📋 MỤC LỤC
1. [Tổng Quan Game](#tổng-quan)
2. [Nhân Vật Chính](#nhân-vật-chính)
3. [Nhân Vật Phụ & NPC](#nhân-vật-phụ--npc)
4. [Môi Trường & Địa Điểm](#môi-trường--địa-điểm)
5. [Cốt Truyện Chi Tiết](#cốt-truyện-chi-tiết)
6. [Gameplay & Mechanics](#gameplay--mechanics)
7. [UI/UX Design](#uiux-design)
8. [Art & Audio](#art--audio)

---

## 🎮 TỔNG QUAN

### Core Concept
Người chơi vào vai Nam, một thanh niên 19 tuổi sống giữa chiến tranh. Thay vì tham gia chiến đấu trực tiếp, Nam chọn con đường vận chuyển thư - một công việc tưởng như đơn giản nhưng lưu giữ hy vọng của hàng trăm con người.

### Theme & Message
- **Chủ đề chính:** Hy vọng giữa tuyệt vọng, tình nhân ái giữa chiến tranh
- **Thông điệp:** Chiến tranh chia cắt con người, nhưng hy vọng luôn tìm được đường
- **Tone:** Realist, emotional, bittersweet

### Target Gameplay Length
- **Chapter 1:** 15-20 phút
- **Chapter 2:** 20-25 phút  
- **Chapter 3:** 25-30 phút
- **Total:** ~60-75 phút

---

## 👥 NHÂN VẬT CHÍNH

### 🔹 NAM - Người Nhân Vật Chính

**Thông Tin Cơ Bản:**
- **Tuổi:** 19 tuổi
- **Tính Cách:** Nhút nhát ban đầu, chịu thương nhân hậu, mạnh mẽ giữa thử thách
- **Lời Thoại:** Bình thường, nhưng có sự trưởng thành dần qua các chương

**Ngoại Hình:**
- Mặc áo sơ mi cũ, quần côn đen
- Đeo túi ba lô cashed
- Có sẹo nhỏ trên trán (từ lần bị bom phá nhà)
- Biểu cảm mặt đơn giản nhưng biểu cảm sâu sắc

**Development Arc:**
- **Chương 1:** Chàng trai ngây thơ, chỉ cho là công việc kiếm tiền
- **Chương 2:** Bắt đầu nhận ra trách nhiệm, khó khăn của chiến tranh
- **Chương 3:** Trưởng thành hoàn toàn, chấp nhận mất mát nhưng vẫn giữ hy vọng

**Kỹ Năng Gameplay:**
- Di chuyển: Chạy, nhảy, leo dốc
- Stealth: Núp, lẩn trốn
- Interaction: Lấy/giao đồ vật
- Stamina-based recovery: Cần nghỉ ngơi để hồi phục

**Mục Tiêu Gợi Nhớ:**
- Tìm anh trai (tiềm ẩn)
- Hoàn thành nhiệm vụ vận chuyển thư
- Bảo vệ mẹ

---

### 🔹 MẸ NAM - Người Mẹ

**Thông Tin:**
- **Tuổi:** ~50 tuổi
- **Tình Trạng:** Gầy yếu, buồn bã nhưng kiên cường
- **Vai Trò:** NPC ở nhà, không tham gia gameplay

**Ngoại Hình:**
- Mặc áo dài truyền thống màu xám
- Tóc bạc, khuôn mặt sâu
- Thường ngồi bên cửa, chờ đợi

**Câu Thoại:**
- **Chương 1 (đầu):** 
  - "Con, cậy công việc này nhé. Chúng mẹ con cần tiền."
  - "Hãy cẩn thận trên đường."
  
- **Chương 2 (lúc Nam về):**
  - "Sao con mặt mũi buồn rầu vậy? Con có chứng kiến gì sơ sơ trên đường?"
  - "Chiến tranh này... chắc chắn sẽ còn nhiều người chết."
  
- **Chương 3 (cảnh cuối):**
  - "Anh trai con đã không thể trở về... nhưng con còn đây, đó đã đủ."

**Relationship Quest:** Các cuộc hội thoại giữa Nam và Mẹ sẽ phản ánh sự trưởng thành

---

### 🔹 ANH TRAI NAM (Trần Minh) - Nhân Vật Vắng Mặt

**Thông Tin:**
- **Tuổi:** 24 tuổi
- **Tình Trạng:** Tử sĩ tại chiến trường
- **Vai Trò:** Motivate story, không xuất hiện trực tiếp (ngoài flashback)

**Ngoại Hình:**
- Mặc quân phục lính
- Khuôn mặt giống Nam nhưng góc cạnh hơn
- Đôi mắt xanh xáo

**Nhật Ký Anh:**
- "Nếu em nhận được lá thư này, có lẽ anh đã không thể trở về."
- "Hãy thay anh chăm sóc mẹ."
- "Dù chiến tranh kết thúc thế nào, anh muốn em sống một cuộc sống bình yên."

---

## 🎭 NHÂN VẬT PHỤ & NPC

### CHƯƠNG 1

#### 🔹 TRẠM TRƯỞNG LIÊN LẠC - Ông Hùng

**Thông Tin:**
- **Tuổi:** 50 tuổi, cựu lính
- **Vai Trò:** Người giao nhiệm vụ cho Nam
- **Tính Cách:** Hiền lành nhưng có quyền lực, tàn tật

**Ngoại Hình:**
- Mặc sơ mi cũ, gùi gậy
- Tay hai gập vì bom
- Khuôn mặt cằn cỗi

**Câu Thoại:**
- **Lần đầu gặp:** "Cậu là con trai người góa phụ ở làng Mỏ Khí phải không? Tôi cần một người trẻ khỏe mạnh. Cậu có dám không?"
- **Nếu Nam từ chối:** "Không sao. Tôi sẽ tìm người khác. Nhưng cơm cơ... cũng sẽ khó tìm thêm."
- **Khi giao nhiệm vụ:** "Mỗi lá thư đều quan trọng. Nó mang theo tin tức, mang theo hy vọng."

**Rewards:**
- 50.000 đồng cho mỗi túi thư thành công
- +10 Fame
- Unlock: Trạm Liên Lạc

---

#### 🔹 NGƯỜI NHẬN THƯ ĐẦUTIÊN - Anh Sơn

**Thông Tin:**
- **Tuổi:** 35 tuổi
- **Vai Trò:** NPC nhận thư đầu tiên, giáo dục người chơi
- **Tính Cách:** Lịch sự, buồn bã

**Ngoại Hình:**
- Mặc áo thợ rách rưới
-坐 trong ngôi nhà bị bom phá nửa tường
- Mắt buồn

**Câu Thoại:**
- **Khi Nam tìm tới:** "Cậu là người mang thư? Tôi chờ đã lâu rồi."
- **Nhận thư:** "(Mở thư, đọc, nước mắt chảy) Vợ tôi... vợ tôi đã sinh con trai rồi."
- **Cảm ơn:** "Cảm ơn cậu từ đáy lòng. Lá thư này... nó là hy vọng của tôi."

**Side Quest:** Anh Sơn sẽ hỏi Nam mang thư trả lời cho vợ

---

#### 🔹 CÁC NPC KHÁC - Dân Làng Bình An

**Bà Già Quán Nước (Bà Liên):**
- **Tuổi:** 65 tuổi
- **Vai Trò:** NPC side quest, bán nước
- **Câu Thoại:** "Cậu thật dũng cảm. Nên uống chút nước luôn, con trai."

**Trẻ Em:**
- **Tuổi:** 5-10 tuổi
- **Vai Trò:** Environmental storytelling
- **Hành Động:** Chơi trong tro cốt, không sợ bom

---

### CHƯƠNG 2

#### 🔹 NGƯỜI LÍNH TRẺ - Lính Thứ Nhất

**Thông Tin:**
- **Tuổi:** 21 tuổi
- **Vai Trò:** Người nhận viên, death quest catalyst
- **Tính Cách:** Yếu đuối, sợ chết, có lương tâm

**Ngoại Hình:**
- Mặc quân phục rách rưới
- Lúc nào cũng run rẩy
- Mắt sợ hãi

**Câu Thoại (Gặp Nam lần đầu):**
> "Em là người mang thư? Tốt rồi... em có thể giúp em gửi bức thư này cho... cho mẹ em không? Nếu em không quay lại, hãy bảo mẹ em rằng... em yêu mẹ."

**Câu Thoại (Nếu bị bắt):**
> "Hãy cứu em! Em không muốn chết! Em chưa thực sự sống bao giờ!"

**Kết Cục:**
- Người lính chết trong nhiệm vụ
- Nam phải giao thư cho mẹ anh ở cuối chương

---

#### 🔹 MẸ NGƯỜI LÍNH - Bà Lan

**Thông Tin:**
- **Tuổi:** 50 tuổi
- **Vai Trò:** Emotional climax, death catalyst
- **Tính Cách:** Hy vọng đến tuyệt vọng

**Ngoại Hình:**
- Mặc áo dài cũ xám
- Tóc bạc, mặt gầy guộc
- Đôi mắt tìm kiếm

**Cảnh Này (Cuối Chương 2):**
```
Nam: "Bà Lan... em..."
Bà Lan: "Anh con có quay lại không?"
Nam: "(Không nói gì, chỉ lắc đầu)..."
Bà Lan: "(Nối lấy bức thư, nước mắt chảy dòng dòng)"
"Tôi biết rồi... tôi đã có cảm giác khi con này không quay lại."
(Bà ngồi xuống, đọc thư của con, kêu la đau đớn)
```

---

### CHƯƠNG 3

#### 🔹 CỰC TRƯỞNG TRẠM THƯ - Ông Thiệu

**Thông Tin:**
- **Tuổi:** 60 tuổi
- **Vai Trò:** Quản lý cuối cùng, phân công finale quest
- **Tính Cách:** Nghiêm túc, công bằng

**Ngoại Hình:**
- Mặc quân phục gọn gàng
- Mặt kểu, đôi mắt sắc sảo
- Tóc bạc hoàn toàn

**Dialogue (Lần đầu):**
> "Các bức thư này là của các anh hùng đã ngã xuống. Chúng ta phải đảm bảo những dòng chữ cuối cùng của họ được giao đến được."

---

#### 🔹 NHỮNG NGƯỜI YÊU (Wives/Families)

**Vợ Người Lính 1 (Chị Liên):**
- Có con nhỏ, chờ tin người chồng
- Reaction: Buồn bã khi nhận tin

**Cha Người Lính 2 (Ông Hùng Cũ):**
- Mỏi mệt, ngồi chờ
- Reaction: Từ hy vọng chuyển sang tuyệt vọng

---

## 🌍 MÔI TRƯỜNG & ĐỊA ĐIỂM

### CHƯƠNG 1: LÀNG MỎ KHÍ → LÀNG BÌNH AN

#### 📍 NHÀ NAM

**Mô Tả:**
- Ngôi nhà lợp tranh nhỏ
- Bức tường bị bom phá, nửa sập
- Bên trong: Một cô bàn ăn cũ, chiếu ngủ, bóng sáng từ cửa sổ

**Props:**
- Ảnh anh trai trên tường
- Chảo cơm cũ
- Lớp bụi dày

**Environmental Story:**
- Các dấu tích chiến tranh: lỗ bom, khiên nát
- Tuber cây lớn bị đâm xuyên
- Đất cát vàng bị ô nhiễm

**Interaction:**
- Nói chuyện với Mẹ Nam
- Tìm đồ vật (optional)

---

#### 📍 TRẠM LIÊN LẠC

**Mô Tả:**
- Căn cứ tạm thời, kiên cố hơn nhà dân
- Tường bê tông, cửa sắt
- Bên trong: Bàn, ghế cũ, tấm bản đồ trên tường

**Props:**
- Túi thư, giấy viết
- Đèn dầu, bộ sơ cấp cứu
- Bàn chế độ ám thực

**NPCs:**
- Trạm trưởng Ông Hùng
- 2-3 nhân viên khác

**Interaction:**
- Nhận nhiệm vụ
- Xem bản đồ
- Lấy túi thư

---

#### 📍 CON ĐƯỜNG MỎ KHÍ → BÌNH AN

**Mô Tả:**
- Con đường đất, rộng 2-3 meter
- Cây cối che phủ 60% con đường
- 3 main sections:
  1. **Section 1:** Làng tợi, nhà cửa đổ nát
  2. **Section 2:** Rừng lơ gò, thoa
  3. **Section 3:** Cánh đồng, nơi chiến trường cũ

**Environmental Hazards:**
- **Obstacles:** Gốc cây, đá to, hố bom
- **Weather:** Nắng quá mặt (Section 1), má che bớt (Section 2)
- **Enemies:** Không có (Chương 1 an toàn)
- **Ambiance:** Tiếng chim, gió, tiếng xa tác chiến

**Props & Stories:**
- Quan tài gỗ bỏ hoang
- Giày cũ nằm trên đường
- Ảnh gia đình rách rưới
- Xác phá hủy

**Puzzle/Challenge:**
1. **Gỗ Mục:** Cần nhảy qua đúng vị trí
2. **Đá To:** Cần leo qua hoặc vòng
3. **Hố Bom:** Cần nhảy xa

---

#### 📍 LÀNG BÌNH AN

**Mô Tả:**
- Làng ven sông nhỏ
- ~30-40 ngôi nhà, nửa sập
- Cây weeping willow bên bờ sông

**Districts:**
1. **Market Area:** Chợ cũ, quán nước
2. **Residential:** Nhà ở, nhà thờ
3. **River:** Bến nước, cây

**NPCs:**
- Anh Sơn (quest receiver)
- Bà Liên (bán nước)
- Trẻ em chơi
- Người già ngồi đợi

**Interaction:**
- Giao thư cho Anh Sơn
- Nói chuyện với các NPC
- Optional: Giúp đỡ NPC khác

---

### CHƯƠNG 2: VÀO ĐÊMRỪNG

#### 📍 CĂN CỨ TIẾN TUYẾN

**Mô Tả:**
- Hầm đất được xây cố định
- Cửa sắt, bản đồ chiến trường
- Khí thở sã, ánh đèn dầu yếu

**NPCs:**
- Lính trạm đạo
- Người lính trẻ (quest giver)
- Sĩ quan chỉ huy

**Interaction:**
- Nhận thư từ người lính trẻ
- Nghe câu chuyện (emotional sequence)

---

#### 📍 RỪNG ĐÊM - STEALTH SECTION

**Mô Tả:**
- Rừng dày đặc, hạn chế tầm nhìn
- Mưa từ từ, sau đó mưa nặng
- Lạnh giá, âm u

**Enemies & Threat:**
- **Patrol Groups:** 2-3 lính địch đi tuần
- **Searchlights:** Đèn chiếu xung quanh
- **Alarm Posts:** Chuông báo động
- **Dynamic Weather:** Gió, sấm, chớp

**Environmental Challenges:**
1. **Bụi Cây:** Núp được
2. **Nước Biển:** Chỉ nước cạn
3. **Tục Cây:** Cản đường

**Stealth Mechanics:**
- Nhân vật bị phát hiện nếu:
  - Bị đèn chiếu sáng
  - Di chuyển quá gần lính
  - Làm tiếng ồn (nước, gỗ gãy)

**Weather Event (Boss Encounter):**
- **Mưa Giông Lớn:** Điểm cao trào
- Người chơi forced vào hầm trú ẩn
- Mưa to tầm tả, sấm sét gây hại
- Cảnh quay: Mưa, sấm, đất nổi trộn

---

#### 📍 HẦM TRú ẨN

**Mô Tả:**
- Hang tự nhiên hoặc hầm đất cũ
- Tường đất, bụi bặm
- Duy nhất nơi an toàn

**Mechanics:**
- Người chơi phải chạy vào khi mưa
- Nằm trên đất chờ mưa đi
- Optional: Đối thoại với Nam về chiến tranh

---

#### 📍 BÊN SỐ - CĂN CỨ TIẾP ĐÓN

**Mô Tả:**
- Lều quân quả toàn
- Bàn bế, bật thưa
- Liên tục có tiếng pháo
- Người lính mệt mỏi

**NPCs:**
- Người nhận thư (NPC khác)
- Điều dưỡng quân đội

**Quest Resolution:**
- Giao thư cho người lính của Bà Lan
- Anh này có bị thương nhẹ

---

### CHƯƠNG 3: CHIẾN TRƯỜNG CŨ - EXPLORATION

#### 📍 HẦM QUÂN SỰ ĐỔ NƯỚC

**Mô Tả:**
- Một cấu trúc khổng lồ bê tông bị bomb
- Bên trong: Đặc xối xả, ôi thối
- Bóng tối, cần đèn

**Exploration Route:**
1. Vào từ lỗ bom phía trên
2. Hành lang tối
3. Phòng chỉ huy (bản đồ, hộp thư)
4. Phòng ngủ lính (xác, hành trang)

**Props & Story:**
- Bàn bế nát bét
- Mũ quân, vũ khí cũ
- Ảnh gia đình rách
- **VĂN KIỆN QUAN TRỌNG:** Túi thư chôn vùi

---

#### 📍 CHIẾN HÀO BỎ HOANG

**Mô Tả:**
- Hàng dài Bunker, hầm trú ẩn
- Thành đất cao, câu thép sợi
- Thảm tường tàn dư

**Puzzles:**
- Tìm đường trong hầm (maze-like)
- Tránh sập đổ
- Collect documents/letters

**NPCs:** Không, chỉ môi trường

---

#### 📍 ĐỒNG QUẬTANÃ - NỤI CUỐ

**Mô Tả:**
- Bộ phận rộng lớn
- Cây bị cháy, đất đen
- Gió gulo, u ám

**Final Destination:**
- Nơi Anh Trai Nam hy sinh
- Memorial scene
- Read the brother's letter

---

## 📖 CỐT TRUYỆN CHI TIẾT

### 🔷 CHƯƠNG 1: CON ĐƯỜNG HY VỌNG

#### ACT 1: CUỘC GỌI TÌN TƯỞNG
```
[HOME - NAM & MẸ]
MẸ: "Con, tiền cơm cơ hết rồi. Hôm nay Mẹ chỉ nấu được cháo lợn."
NAM: "Vâng mẹ. Con sẽ tìm việc thêm."
MẸ: "Hơn nữa... tin anh con không có được 6 tháng rồi."
NAM: "Anh sẽ quay lại, Mẹ ơi. Anh chắc chắn sẽ quay lại."

[TRANSITION: FADE TO BLACK, TIẾNG BOM NỔ]

[TRẠM LIÊN LẠC - GẶP ÔNG HÙNG]
ÔNG HÙNG: "Cậu là con trai người góa phụ ở làng Mỏ Khí phải không?"
NAM: "Vâng ông."
ÔNG HÙNG: "Tôi cần một người trẻ khỏe mạnh. Cậu có dám không?"
NAM: "Ông định cho em làm gì ạ?"
ÔNG HÙNG: "Vận chuyển thư. Công việc đơn giản, nhưng điều cần là kiên nhân."
NAM: "Con sẽ làm ạ."
ÔNG HÙNG: "Tốt. Hôm nay cậu phải giao 10 lá thư đến làng Bình An. 
           Túi thư đây. Mỗi lơi thư có tên người nhận. Hãy tìm đúng người nhé."
```

#### ACT 2: CON ĐƯỜNG ĐẦU TIÊN
```
[CON ĐƯỜNG - NAM ĐI BỘKHÁM PHÁ]
- Nam bắt đầu đi, khám phá landscape
- Thấy các dấu tích chiến tranh
- Encounter 1: Bà Già Quán Nước
  BÀ LIÊN: "Cậu là người vận chuyển thư mới phải không? Nên cẩn thận."
  NAM: "Bà cẩn thận cái gì?"
  BÀ LIÊN: "Mầu địch tuần tra gần diễn khu. Tuy chương này con đường an toàn, nhưng..."
  NAM: "Vâng, em sẽ cẩn thận."

- Encounter 2: Trẻ em chơi
  - Trẻ con chơi trong bụi bặm, không sợ hãi gì
  - Optional: Nói chuyện với chúng

- Challenges:
  * Vượt qua gỗ mục (timing puzzle)
  * Leo qua đá to
  * Nhảy qua hố bom
```

#### ACT 3: LAM BÌNH AN - NHIỆM VỤ
```
[LÀNG BÌNH AN - GẶP ANH SƠN]
NAM: "Xin lỗi, có phải ông là... Sơn Trần Văn?"
ANH SƠN: "Vâng, tôi là. Cậu là người mang thư?"
NAM: "Vâng, thư từ trạm liên lạc ạ."
ANH SƠN: "(Nhận thư, mở ra)" 
(Đôi mắt sáng lên, nước mắt chảy)
"Em gái tôi... em gái tôi sinh con rồi! Con trai! Tôi có con trai rồi!"
NAM: "(Nụ cười)"
ANH SƠN: "Cảm ơn cậu, thực sự cảm ơn từ đáy lòng. 
          Lá thư này... nó ngắn gọn nhưng nó là hy vọng của tôi."
NAM: "Đó là danh vụ của em."
```

#### ACT 4: NHẬN THỨC
```
[LÀNG BÌNH AN - NĂM QUAN SÁT]
- Nam ngồi bên bờ sông, quan sát
- Nhìn các gia đình chờ tin
- Nhìn trẻ em, người già
- Internal Monologue (Text on screen):
  "Mỗi lá thư đều mang theo hy vọng của một con người.
   Một tin tức từ người thân, từ một người yêu.
   Hay chỉ là lời tạm biệt cuối cùng."
```

#### ACT 5: BỤC VỀ & TUYÊN BỐ
```
[NHÀ TRẠM - GIAO BÁO CÁO]
ÔNG HÙNG: "(Kiểm tra túi thư) Tốt lắm. Cậu hoàn toàn nhiệm vụ đúng hạn."
NAM: "Vâng ông."
ÔNG HÙNG: "Hôm nay cậu khám phá ra gì?"
NAM: "Em... em thấy mọi người chơi tin người thân. Và khi họ nhận được thư..."
ÔNG HÙNG: "Thì hy vọng được sống thêm. Đúng không?"
NAM: "Vâng ông."
ÔNG HÙNG: "Cậu là người phù hợp cho công việc này. 
           Từ hôm nay, cậu là một người vận chuyển thư của chúng tôi."
NAM: "Vâng ông, em sẽ phản đấu."

[CUTSCENE CUỐI - NAM NHÌN SANG CHIẾN TRƯỜNG]
- Nam đứng trên một quả đồi nhỏ
- Phía xa, khói chiến trường vẫn bay
- Tiếng pháo vang xa xa
- Camera nhìn đứa lên bầu trời, sau đó quay về Nam
- Nam đặt tay lên ngực, một dòng thẻ xuất hiện:
  "Anh ơi, em sẽ làm được đúng không?"
```

---

### 🔷 CHƯƠNG 2: BÓNG TỐI CHIẾN TRANH

#### ACT 1: NHIỆM VỤ MỚI
```
[TRẠM LIÊN LẠC - NGÀY HÔM SAU]
ÔNG HÙNG: "Nam, hôm nay công việc sẽ khó hơn. 
           Cậu phải vận chuyển thư đến một căn cứ tiền tuyến.
           Con đường duy nhất đi qua rừng đêm, nơi có địch tuần tra."
NAM: "(Thốt lên) Rừng đêm? Địch tuần tra?"
ÔNG HÙNG: "Cậu có sợ không?"
NAM: "(Cắn môi, gật đầu) Không... em sẽ làm."
ÔNG HÙNG: "Tốt. Đây là túi thư đặc biệt - từ người lúa tiến tuyến gửi cho gia đình."
```

#### ACT 2: VÀO RỪNG & GẶP SỰ NGUY HIỂM
```
[RỪNG - ĐÊMĐẦU TIÊN]
- Nam bắt đầu vào rừng
- Âm u, chỉ có đèn pin tự mang
- Bên cạnh là tiếng chim đêm, tiếng gí queo
- Suddenly: Một nhóm lính địch xuất hiện (ai tuần tra)
- Mini-Cutscene: Nam nhanh chóng núp vào bụi

[STEALTH SEQUENCE]
- Người chơi phải tránh được 2-3 nhóm tuần tra
- Nếu bị phát hiện: Game Over, Retry
- Tiếng sách, tiếng nước tạo căng thẳng

[CĂN CỨ TIẾN TUYẾN - GẶPNHÂN VẬT QUAN TRỌNG]
NAM: "(Bứt cửa sắt) Em là người vận chuyển thư từ Trạm Liên Lạc."
SĨ QUAN: "Vào nhanh!"
(Nam vào, cửa đóng)

[HẦM - CUỘC GẶPVỚI NGƯỜI LÍNH TRẺ]
LÍNH TRẺ: "(Mặt xanh xao, tay run rẩy) Cậu là người vận chuyển?"
NAM: "Vâng ạ."
LÍNH TRẺ: "Cảm ơn... xin cảm ơn cậu rất nhiều.
           Tôi có một... tôi có một bức thư cần gửi đi.
           Nếu tôi không quay lại mai, hãy giúp tôi..."
NAM: "Ông sẽ quay lại mà."
LÍNH TRẺ: "(Cười cay đắc) Không... tôi biết tôi sẽ chết. Tôi cảm nhận được."
NAM: "..."
LÍNH TRẺ: "Đây là bức thư cho mẹ tôi, ở làng Cái Bé.
           Hãy bảo mẹ tôi rằng... tôi yêu mẹ rất nhiều."
(LÍNH TRẺ viết địa chỉ trên thư)

[INTENSE DIALOGUE]
LÍNH TRẺ: "Cậu có biết... cảm giác nào là sợ nhất khi chết không?
          Không phải là đau đớn. Đó là sợ mình sẽ bị quên lãng."
NAM: "Mẹ ông sẽ không quên ông."
LÍNH TRẺ: "Cảm ơn cậu... nếu chiến tranh kết thúc này, 
          hãy bảo người đó rằng... em đã sống."
```

#### ACT 3: MƯA GIÔNG & BOSS ENCOUNTER
```
[RỪNG LẦN THỨ HAI - CHỈ ĐẾN MƯA]
- Nam rời căn cứ, túi thư mới trên lưng
- Trời bắt đầu u ám
- Gió lớn, mưa từ từ
- Tiếng sấm vang xa
- Sudden: HEAVY RAIN & THUNDER STORM
  Game Mechanic: Boss Environmental
  - Người chơi bị mưa làm rối mắt (blur effect)
  - Sấm sét có thể gây hại (rare damage)
  - Cần chạy tìm nơi trú ẩn
  - Cutscene: Lightning Strike (implied danger)

[HẦM TRú ẨN - SHELTER]
- Nam chạy vào hang, nằm xuống trên đất ẩm ướt
- Mưa tiếp tục ngoài (ambiance sounds)
- Internal Monologue (Text):
  "Anh ơi, anh đang ở đâu bây giờ? Có phải anh cũng đang nằm trong đất 
   ướt như thế này, sợ hãi, chờ sự sống sót?"

[CUTSCENE: NAM NẬP ĐỊA & NAM ĐỊA RAIN STORM CLEARS]
- Mưa đi qua
- Bầu trời lại sáng
- Nam lô ra khỏi hang
- Cảnh quay: Nước bẩn chảy, cây gãy, đất nổi trộn
```

#### ACT 4: GIAO THƯĐẤUTIÊN & TRAGEDY
```
[CĂN CỨ TIẾP ĐÓN - GIAO THƯ]
- Nam chuẩn bị giao thư
- Nam gặp người nhận thư (SĨ QUAN KHÁC, sau Vũng Tàu Người Lính Trẻ)
SĨ QUAN: "Thư từ mẹ tôi!"
NAM: "Không... đây là...</
(Cải Cập Lính Trẻ)"
SĨ QUAN: "(Mặt thay đổi, hiểu được ý nghĩa)
          Anh ấy... Quân ơi, anh ấy..."
NAM: "Em... em xin lỗi."
(Yên tĩnh. Chỉ có tiếng gió & tiếng pháo xa xa)

[GIAO THƯCUỐI - NGƯỜI MẸ BÀ LAN]
- Nam phải tìm nhà Bà Lan (một phần của map ngoài chiến trường)
- Bà Lan ngồi chờ bên cử nhà nát
NAM: "Bà Lan phải không?"
BÀ LAN: "(Nhìn lên hy vọng) Anh con... anh con có quay lại không?"
NAM: "(Cúi đầu, không nói gì)"
BÀ LAN: "(Hiểu được, nước mắt đầu tiên rơi)"
NAM: "Bà, em mang lá thư cuối từ anh con. Anh muốn bà biết..."
BÀ LAN: "(Nhận lấy thư, mở ra bằng tay run rẩy)"
(Đọc, rồi khóc lớn, nằm lên người Nam)
BÀ LAN: "Tạ ơn... tạ ơn cậu đã đưa nó đến. Đó là lần cuối tôi nghe từ anh."
```

#### ACT 5: KẾT THÚC CHƯƠNG & REFLECTION
```
[NHÀ NAM - GẶPMÉNAM]
- Nam về nhà, buồn bã
- Mẹ gõ cửa hỏi
MẸ: "Con về rồi? Công việc hôm nay sao?"
NAM: "(Lâu lâu mới nói)
     Mẹ, em thấy... em thấy chiến tranh này không chỉ là total chiến đấu.
     Nó còn là chia cắt những gia đình, chiếm mất những người yêu."
MẸ: "Vâng, chiến tranh như vậy. Anh con cũng..."
NAM: "Em biết rồi, Mẹ. Em sẽ...em sẽ tiếp tục làm."
MẸ: "Tốt con. Bệ em sẽ tự hào về con."

[CUTSCENE CUỐI - DUSK AT THE MEMORIAL SITE]
- Nam đứng trên trong trời hoàng hôn
- Phía dưới là chốn nơi người lính trẻ hy sinh
- Camera pan sang những bộ xương nằm dưới đất (implied, không chi tiết)
- Text on Screen:
  "Chiến tranh chứng tỏ rằng tính mạng người rất dễ mất.
  Nhưng Nam bắt đầu hiểu, tình yêu là điều sống lâu nhất."
```

---

### 🔷 CHƯƠNG 3: LÁ THƯ CUỐI CÙNG

#### ACT 1: NHIỆM VỤ TÌM KIẾM
```
[TRẠM LIÊN LẠC - NGÀY ĐÓ]
ÔNG THIỆU (CỰC TRƯỞNG): "(Trìn hành bản đồ chiến trường) 
                        Sau trận chiến lớn hôm qua, 
                        một túi thư chứa đầy mối tin bị mất.
                        Chúng tôi biết nó bị gạt xuống trong rừng.
                        Cậu có thể tìm nó không?"
NAM: "Em sẽ tìm."
ÔNG THIỆU: "Vậy thì đi. Mỗi lá thư đó là từ lời tạm biệt cuối cùng của các anh hùng."
```

#### ACT 2: KHÁM PHÁ CHIẾN TRƯỜNG
```
[MAP: EXPLORATION - HẦM QUÂN SỰ ĐỔ NƯỚC]
- Nam nhập vào hầm bị bom phá
- Bóng tối, cần đèn (game mechanic: manage light)
- Gặp các obstacle:
  * Dầm xuyên
  * Hộp lựu đạn chưa nổ (tránh)
  * Xác người (environmental storytelling)

[PHÒNG CHỈ HUY - BÌNBẢN ĐỒ & HỘPTHƯ]
- Nam tìm thấy bàn chỉ huy nằm nước
- Trên bàn: Bản đồ, bây giờ
- **TỀM KIẾM CHÍNH:** Một túi da cũ chôn vùi dưới đống đổ nát
- Nam kéo nó ra

[PHÒNG NGỦCHIÊN SỸ - EMOTIONAL SCENE]
- Phòng full với xác và hành trang
- Ảnh gia đình, thư cũ, giày quân
- Có một điều: **LÁ THƯ VÀNG CŨ VỚI CHỈ DỰ NAM**
```

#### ACT 3: PHÁT HIỆN THẬT
```
[KỲCHIẾN TRƯỜNG - CHỖ NÀMANH TRẢI HY SINH]
[CUTSCENE: FLASHBACK - CHIẾN ĐẤU]
- Nam thấy lại cảnh nơi anh trai hy sinh (not explicit)
- Soliders fighting, grenade explosion, darkness
-THEN: Letter appears on screen

[READING THE LETTER - CLIMAX]
Letter from Tran Minh (Nam's brother):

---
Cái em nhân được thư này, có lẽ anh đã không thể trở về.

Hãy tha thứ cho anh. Anh biết anh không phải là người dũng cảm nhất,
nhưng khi đứng giữa chiến trường này, anh toàn ý thức được rằng,
nếu anh không bảo vệ đàn anh em, thì ai bảo vệ họ?

Em à, anh muốn em biết:

Anh yêu em từ ngày em sinh ra.
Anh yêu mẹ hơn bất cứ thứ gì trên thế giới.
Anh hy vọng em sẽ sống một cuộc đời bình yên,
không phải như anh - phải chọn giữa chiến tranh hay tình yêu.

Nếu em vẫn còn Mẹ, hãy nói với mẹ rằng:
"Anh yêu mẹ nhiều như anh yêu em."

Và hãy thay anh chăm sóc mẹ em.

Hãy sống thật tốt, em. Hãy sống thành công cho cả hai chúng ta.

Mãi yêu,
Anh
---

[NAM'S REACTION - CRYING]
- Nam nằm xuống trong bụi, lá thư trên tay
- Nước mắt chảy: "Anh... anh..."
- Text on Screen: "Nam finally understood: His brother was already gone."
```

#### ACT 4: NHIỆM VỤCUỐ CÙNG
```
[CỰC TRƯỞNG THIỆU - DELIVER FINAL LETTERS]
NAM: "(Trở về, túi thư trên tay) 
     Kích em đã tìm thấy. Còn có 9 lá thư cần được giao."
ÔNG THIỆU: "Tốt. Hãy giao những lá thư này. 
           Để những gia đình biết được những lần cuối của người thân."

[NĂM GIAO LẦN LƯỢT]
**Quest 1:** Giao cho người vợ
- Vợ anh lính (Chị Liên) nhận thư, khóc mất
- Cô có một đứa con nhỏ chỉ biết bố qua ảnh

**Quest 2:** Giao cho cha người lính
- Ông cha (Ông Hùng Cũ) cầm thư tay run rẩy
- Ông yên tĩnh, nhưng nước mắt chảy im lặng

**Quest 3:** Giao cho vợ Người lính khác
- Người vợ mang thai, chỉ nghe tiếng: "Ông sẽ chưa bao giờ gặp con"

[EACH QUEST: MINI-CUTSCENE FLASHBACK]
- **Flashback 1:** Cảnh anh lính gặp vợ
- **Flashback 2:** Cảnh ông cha dạy con
- **Flashback 3:** Cảnh anh lính bao lâu vợ

[FINAL QUEST: DELIVER TO MÈ - EMOTIONAL RESOLUTION]
```

#### ACT 5: ENDING & NEW BEGINNING
```
[MẸ NAM - AFTER FINDING THE LETTER]
- Nam về nhà
- Mẹ đã biết từ tin tức (không explciit)
- NAM: "(Đôi môi run rẩy) Anh... anh ơi."
- MẸ: "(Ôm Nam) Anh ta đã sống một cuộc sống có ý nghĩa, con."
- MẸ: "Anh đã chọn bảo vệ người khác. Anh hy sinh vì tình yêu."
- NAM: "(Nước mắt) Con... con sẽ tiếp tục công việc. 
       Con sẽ gửi những hy vọng đó cho những gia đình."

[TIME-SKIP: CHIẾN TRANH KẾT THÚC]
- Tiếng pháo yên tĩnh
- Mặt trời mọc
- Chiến tranh kết thúc

[FINAL SCENE - 5 NĂM SAU]
- Nam là TRƯỞNG TRẠM THƯ (người lớn lên)
- Ông Thiệu về hưu, định lại địa vị cho Nam
- Nam ngồi ở bàn cũ của Ông Hùng
- Người vận chuyển thư mới đi ra (continuation)

[FINAL CUTSCENE: NAM ON THE FAMILIAR ROAD]
- Nam đi trên con đường cũ, hôm nay không phải vận chuyển, chỉ đi bộ
- Bầu trời lên sám với mặt trời mọc
- Tiếng chim hát
- Đất cơm xanh tươi trở lại
- Text on screen:
  "Chiến tranh chia cắt con người,
   nhưng hy vọng luôn tìm được đường để đến nơi cần đến."

[VOICE-OVER: ĐỌC THƯ CỦA ANH TRẢ]
- Voice từ anh trai Minh:
  "Nếu em nhận được thư này... 
   hãy nhớ: anh luôn ở bên em."

[CREDITS ROLL]
- Ảnh các nhân vật (ending state)
- Bà Lan với cảnh tươi sáng
- Chị Liên với con trai
- Các người vận chuyển thư khác
- Nam, trưởng trạm, nhìn bình minh

FADE TO BLACK

THE END
```

---

## 🎮 GAMEPLAY & MECHANICS

### Core Mechanics

#### 1. **MOVEMENT SYSTEM**
- **Walk:** Standard movement, quiet
- **Run:** Faster but uses stamina, makes noise
- **Jump:** Vault over obstacles, precise timing
- **Climb:** Scale slopes and structures
- **Crouch:** Reduce detection radius
- **Stamina System:** Recovery over time

#### 2. **STEALTH SYSTEM (Chapter 2)**
- **Detection Range:** Visual cone from guards
- **Line of Sight:** Guards only detect within direct vision
- **Noise Level:** Footsteps, breaking branches add detection
- **Stealth Kills:** None (pacifist game) - only escape
- **Hiding Spots:** Bushes, trees, shadows

#### 3. **QUEST SYSTEM**
```
Quest Types:
1. DELIVERY QUESTS: Find NPC, give item
2. EXPLORATION QUESTS: Find items in area
3. SURVIVAL QUESTS: Reach point without detection
4. EMOTIONAL QUESTS: Witness scene, gain understanding
```

#### 4. **DIALOGUE SYSTEM**

**Choice-Based Dialogue:**
- Simple YES/NO choices in key moments
- No massive branching (linear story)
- Choices affect emotion/perception, not main story

**Example:**
```
SOLDIER (CHAPTER 2):
"If you don't return, can you... help me?"
[Choice 1] "Of course, I promise"
  → +Resolve +Trust
[Choice 2] "I don't know if I can..."
  → +Fear -Confidence
(Both lead to same outcome, different stat changes)
```

#### 5. **INVENTORY SYSTEM**
- **Mail Bag:** Main inventory, limited slots (10)
- **Letters:** Individual items, trackable
- **Personal Item:** Optional collectible (increases connection)

**UI Shows:**
- Recipient name
- Status (delivered/pending)
- Optional: Letter content preview

---

### Chapter-Specific Mechanics

#### CHAPTER 1: EXPLORATION & NAVIGATION
**Puzzles:**
- **Rotting Wood Jump:** Precise timing to jump across fragile wood
- **Boulder Climb:** Scale broken boulders
- **Bomb Crater Puzzle:** Navigate around blast craters

**Learning Curve:**
- Tutorial elements woven in
- No fail state for movement (can retry)
- Time pressure: Sunset mechanic

**Checkpoints:**
- Auto-save at major locations
- Manual save at resting points

---

#### CHAPTER 2: STEALTH & SURVIVAL
**Stealth Mechanics:**
```
DETECTION STATES:
1. CLEAR: No threat detected
2. SUSPICIOUS: Guard heard something (10 sec window)
3. ALERT: Guard investigating!
4. HOSTILE: Guard in combat/alarm
```

**Guard Behavior:**
- Pattern-based (predictable patrol routes)
- React to sound (radius-based)
- Communication (alarm spreads alert)
- Recovery: Suspicion fades if hidden long enough

**Weather Boss:**
```
RAIN STORM DIFFICULTY:
- Duration: ~3 minutes
- Visibility: Reduced 50%
- Audio: Muffled (guards can't hear)
- Danger: Exposed areas → shelter is "safe"
- Fail Condition: If in open when lightning strikes = restart section
```

**Stealth Failure Consequence:**
- Restart from checkpoint
- Emotional cost: (Internal monologue about fear)
- No death penalty (immersive, not punishing)

---

#### CHAPTER 3: EXPLORATION & ENVIRONMENTAL PUZZLE
**Exploration Mechanics:**
```
SEARCH SYSTEM:
- Hold [E] to examine object
- Find clues/letters automatically
- No combination locks (linear exploration)
- Environmental story through objects
```

**Progression Gate:**
- Can't progress without finding key letter
- But exploration is guided (breadcrumbs)
- Multiple paths to same destination (player agency)

**Emotional Puzzle:**
- Piece together story through Found Documents
- Sequencing doesn't matter (can read finale first)
- Understanding grows with each letter

---

### Progression

#### STATS/PROGRESSION SYSTEM
```
NAM'S EMOTIONAL STATE:
┌─────────────────────┐
│ Determination: 30/100│  (How resolved to complete quest)
│ Compassion: 50/100  │  (How empathetic to others' suffering)
│ Courage: 40/100    │  (How brave in dangerous situations)
│ Wisdom: 20/100     │  (How aware of war's nature)
└─────────────────────┘

Quest Completion affects these:
- Compassion += | When witness suffering
- Courage += | When escape danger
- Wisdom += | When find truth
- Determination += | Per quest completed
```

**No Skill Tree:** Stats are auto-adjusted by story events
**No Combat Upgrades:** No weapons, no leveling
**No Economy:** No buy/sell system

---

## 🖼️ UI/UX DESIGN

### Main Menu
```
┌──────────────────────────┐
│   NGƯỜI ĐƯA THƯ         │ (Title - handwritten style)
│                          │
│  [NEW GAME]             │
│  [CONTINUE]             │
│  [SETTINGS]             │
│  [CREDITS]              │
│  [EXIT]                 │
└──────────────────────────┘
```

### HUD (In-Game)
```
TOP-LEFT:
┌─────────────────────┐
│ Chapter 1: Con Đường│
│ Hy Vọng             │
│ Quest: Deliver mail │
└─────────────────────┘

TOP-RIGHT:
┌──────────────┐
│ Time: 14:30 │
│ Weather: ☀  │
└──────────────┘

BOTTOM-LEFT:
┌─────────────────────────┐
│ MAIL BAG (3/10)        │
│ □ Letter to Tran Van   │
│ □ Letter to Linh Phuong│
│ □ Letter to Tran Hang  │
└─────────────────────────┘

BOTTOM-CENTER:
[E] Pick up / [Q] Interact / [X] Cancel
```

### Dialogue Box
```
┌──────────────────────────────────────┐
│ UNCLE HUNG (Trạm Trưởng)            │
│                                      │
│ "Cậu là con trai người góa phụ      │
│  ở làng Mỏ Khí phải không?"         │
│                                      │
│ [Yes, Uncle] [No, someone else]     │
└──────────────────────────────────────┘
```

### Quest Log
```
ACTIVE QUESTS:
├─ [MAIN] Deliver mail to Binh An
│  └─ Status: 1/10 delivered
│  └─ Quest Giver: Trạm Trưởng Hùng
│  └─ Reward: 50,000 đ + Fame
│
├─ [SIDE] Find Uncle Sung
│  └─ Status: Incomplete
│  └─ Reward: +1 Compassion
```

### Map System
```
MINI MAP (Bottom-Right):
┌─────────────────┐
│ ⊕ = You        │
│ ▼ = Quest Goal │
│ ✉ = Mail       │
│ △ = Hazard     │
│ ♦ = NPC        │
└─────────────────┘

FULL MAP (Pause Menu):
- Hand-drawn style
- Markers for discovered locations
- Fog of war for unexplored areas
```

### Pause Menu
```
┌─────────────────────────────────────┐
│          GAME PAUSED               │
│                                     │
│  [RESUME]                          │
│  [SAVE]                            │
│  [LOAD]                            │
│  [SETTINGS]                        │
│  [QUIT TO MENU]                    │
│                                     │
│  Stats:                            │
│  └─ Compassion: ████░░░░░░ 50/100 │
│  └─ Courage: ███░░░░░░░░░ 40/100  │
└─────────────────────────────────────┘
```

---

## 🎨 ART & AUDIO

### Visual Style

#### Art Direction
- **Style:** Realistic 3D with painterly post-processing
- **Color Palette:**
  - Chapter 1: Warm, dusty oranges & browns (hope)
  - Chapter 2: Cool blues & grays (fear, isolation)
  - Chapter 3: Desaturated with hints of green (recovery)
- **Camera:** Third-person, cinematic angles
- **Lighting:** Volumetric fog, dynamic shadows

#### Character Design
```
NAM:
- Simple model: ~500-800 tris
- Wearing torn clothes
- Expressive face (key direction points):
  * Nervous (Chapter 1)
  * Haunted (Chapter 2)
  * Resolved (Chapter 3)

NPCS:
- Modular design for reusability
- Distinct silhouettes (fat/thin/old/young)
- Expressive animations for key moments
```

#### Environment

**Chapter 1 - Daytime/Safe:**
- Green vegetation (rice fields)
- Golden sunlight filtering through trees
- Destroyed houses (concrete, wood)
- Dust particles in light rays

**Chapter 2 - Nighttime/Danger:**
- Dark forest with minimal light
- Pale moonlight, searchlight beams
- Fog and mist
- Dynamic storm (rain, lightning)

**Chapter 3 - Aftermath:**
- Destroyed bunkers, trenches
- War debris (helmets, weapons)
- Burned vegetation, blackened earth
- Sunrise slowly illuminating destruction

---

### Audio Design

#### Music/Ambient

**Chapter 1 Theme:**
- Soft piano, traditional Vietnamese instruments (đàn tranh)
- Hopeful but melancholic
- Duration: Loop ~3 min

**Chapter 2 Theme:**
- Tense strings, sparse notes
- Sudden silences create anticipation
- Rain/storm sounds override music
- Duration: Loop ~2 min

**Chapter 3 Theme:**
- Ambient, minimal
- Focus on environmental sounds
- Emotional peak at letter reading

#### Sound Effects

**Footsteps:**
- Dirt/gravel: Light crunch
- Tile/concrete: Sharp click
- Grass: Soft rustling
- Water: Splashing, dripping

**Ambient:**
- Bird calls (morning/dusk)
- Wind through trees
- Distant pháo (gunfire)
- Rain, thunder, lightning

**UI Sounds:**
- Letter pickup: Soft paper crinkle
- Quest complete: Gentle chime
- Stealth detected: Warning beep

#### Voice Acting

**Language:** Vietnamese
**Key Characters Voiced:**
- Nam (minimal, mostly silent)
- Important NPCs (Ông Hùng, Bà Lan, Người lính trẻ)
- Internal thoughts (text-only, occasional narration)

**Acting Direction:**
- Emotional, naturalistic
- No over-acting fantasy sci-fi style
- Pauses, hesitations, realistic dialogue flow

---

## 📊 TECHNICAL SPECIFICATIONS

### Engine & Tools
- **Engine:** Unity 2022 LTS+
- **Scripts:** C#
- **Timeline:** 15-20 weeks production
- **Team:** 3-5 people minimum

### Performance Target
- **Platform:** PC (Windows/Linux/Mac)
- **Resolution:** 1920x1080 @ 60fps
- **Memory:** ~2GB RAM minimum
- **Storage:** ~5GB SSD

---

## 📝 CHAPTER BREAKDOWN SUMMARY

| Aspect | Ch.1 | Ch.2 | Ch.3 |
|--------|------|------|------|
| **Theme** | Hope | Trauma | Acceptance |
| **Time of Day** | Day | Night | Day→Dusk |
| **Main Mechanic** | Parkour/Exploration | Stealth | Exploration |
| **Enemy Threat** | None | High | None |
| **Length** | 15-20 min | 20-25 min | 25-30 min |
| **Main NPC Deaths** | 0 | 1 (boy) | Multiple (revealed) |
| **Quest Count** | 4 main | 1 main | 3 side + 1 main |
| **Emotional Arc** | Intro → Realization | Despair → Acceptance | Truth → Resolution |

---

**END OF GDD**

Tài liệu này cung cấp framework hoàn chỉnh cho team phát triển game. Bạn có thể bắt đầu với:

1. **Cấu trúc script/scene** trong Unity theo các chapter
2. **Character models** cho Nam, NPCs chính
3. **Environment assets** theo từng chapter
4. **Dialogue system** implementation
5. **Stealth mechanics** trong Chapter 2

Muốn tôi tạo thêm chi tiết technical hay code template cho Unity không?
