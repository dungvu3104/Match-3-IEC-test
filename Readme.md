
# Match-3 Unity Game



## I. Performance Improvements (3 changes)

### 1. Object Pooling — Cell Background

- `ObjectPool.cs` — generic object pool
- `CellBackgroundPool.cs` — pool riêng cho cell background
- Được tạo ở `GameManager.cs`, gọi `Initialize()` để spawn đúng số cell trong `GameSettings`
- Reference được pass: `GameManager` → `BoardController` → `Board`

> **Kết quả:** không còn `Instantiate`/`Destroy` mỗi khi vào level

### 2. Object Pooling — Item View (sprite của các viên match-3)

- `ItemViewPool.cs` — pool sprite cho tất cả **10 loại item** (7 normal + 3 bonus) dùng `Dictionary<string, Queue<Transform>>`
- Prefab được cache lại nên `Resources.Load` chỉ gọi **1 lần** mỗi loại
- Được tạo ở `GameManager.cs`, gọi `Initialize()` để pre-spawn `boardSizeX × boardSizeY` instance mỗi loại
- Reference được pass: `GameManager` → `BoardController` → `Board`
- `Board.cs` gọi `item.SetViewPool()` trước mỗi `SetView()` (trong `Fill`, `FillGapsWithNewItems`, `ConvertNormalToBonus`)
- `Item.cs` dùng `pool.Get()` thay cho `Instantiate`, và `pool.Return()` thay cho `Destroy`

> **Kết quả:** không còn `Instantiate`/`Destroy` liên tục khi player phá item và spawn mới

### 3. Bỏ GetComponent mỗi lần swap

- **Trước:** mỗi lần swap, `SetSortingLayerHigher/Lower()` gọi `View.GetComponent<SpriteRenderer>()` × 2
- **Sau:** thêm field `m_spriteRenderer`, cache **1 lần** duy nhất trong `SetView()`
- An toàn vì `SpriteRenderer` luôn tồn tại cùng với `View`

> **Kết quả:** zero `GetComponent` call trong gameplay

---

## II. Theme System (ScriptableObject)

- `Theme.cs` (ScriptableObject) chứa `ThemeName` + mảng **7 Sprite** tương ứng 7 loại normal item
- Mỗi Theme SO là 1 bộ skin — tạo bao nhiêu theme tùy ý: **Create → Match3 → Theme**
- `GameManager.cs` có `List<Theme>` (kéo thả SO vào) và `m_selectedThemeIndex` (chọn bằng index trong Inspector)
- `SelectedTheme` property trả về theme đang chọn; nếu index không hợp lệ → trả `null` (dùng sprite gốc)
- Chain: `GameManager` → `BoardController` → `Board` → `NormalItem`
- `NormalItem.cs`: `SetTheme()` + `ApplyThemeSprite()` swap sprite sau `SetView()`

> **Kết quả:** đổi theme chỉ cần thay `selectedThemeIndex` trong Inspector, không sửa code

---

## III. Reset Button (chơi lại level)

- Thêm nút **Reset** trong `UIPanelGame` để chơi lại level hiện tại mà không cần quay về main menu
- Flow giống pattern các nút khác (`btnPause`, `btnClose`):

```
UIPanelGame.btnReset → OnClickReset() → UIMainManager.ResetLevel() → GameManager.RestartLevel()
```

- `GameManager.cs`: thêm `m_currentLevelMode` lưu mode khi `LoadLevel()` được gọi
  - `RestartLevel()`: `ClearLevel()` → huỷ `m_levelCondition` → `LoadLevel(m_currentLevelMode)`
- `UIMainManager.cs`: thêm `ResetLevel()` gọi `m_gameManager.RestartLevel()`
- `UIPanelGame.cs`: thêm `[SerializeField] Button btnReset` + null check + `OnDestroy()` cleanup

---

## IV. Smart FillGapsWithNewItems

> Item mới **khác 4 ô xung quanh** + **ưu tiên type ít nhất** trên board

- **Trước:** `FillGapsWithNewItems()` gọi `GetRandomNormalType()` — hoàn toàn ngẫu nhiên, có thể trùng neighbor → match vô nghĩa
- **Sau:** thêm 2 method mới trong `Board.cs`:
  - `CountNormalTypes()` — đếm mỗi `eNormalType` trên toàn board → `Dictionary<eNormalType, int>`
  - `GetLeastCountTypeExceptNeighbours(cell, counts)` — exclude type 4 neighbor → chọn type có count thấp nhất → nếu nhiều type cùng min → random
- `FillGapsWithNewItems()` gọi `CountNormalTypes()` **1 lần** đầu, mỗi lần place → `counts[chosenType]++`
- **Fallback:** nếu tất cả 7 type đều bị exclude → dùng `GetRandomNormalType()` như cũ

> **Kết quả:** item mới không trùng neighbor + phân bố type cân bằng trên board


---
## V. Project organization
Cách tổ chức folder hiện tại
> **Pros:**  phù hợp với làm cá nhân, game nhỏ  
> **Cons:** khó làm việc nhóm, scale game lớn

Cách tổ chức folder mới
```
Assets/
├── _Game/
│   ├── Art/
│   │   └── Textures/
│   │       └── Fish/
│   ├── Code/
│   │   ├── Board/
│   │   ├── Controllers/
│   │   ├── Editor/
│   │   ├── Models/
│   │   │   └── SO/
│   │   ├── UI/
│   │   ├── Utilities/
│   │   └── Utility/
│   └── Designer/
│       ├── Prefabs/
│       └── Scenes/
├── Plugins/
│   └── All plugins
└── Resources/
    ├── prefabs/
    └── Theme/
```

Dễ chia 3 vai trò: **Code**, **Art**, **Designer** -> dễ làm việc nhóm hơn 
Dễ scale project lớn hơn

---

## Additional Notes

- Đưa `Update()` của `BoardController.cs` lên đầu file
- Toàn bộ code xử lý input trong `Update()` được tách vào hàm `HandleInput()` rồi gọi lại → dễ đọc hơn

- Đưa game setting và prefab vào folder Designer, để Designer dễ chỉnh sửa

---