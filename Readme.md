Cách tổ chức hiện tại khó scale lớn và làm việc nhóm
tôi sẽ tổ chức lại như sau
_Game

dễ làm việc vs các team hơn


chuyển update của board controller lên trên cùng, 
chuyển toàn bộ input trong update của input controller vào input handle funtion ( dễ đọc hơn)

I. 
1. làm obejct pooling cho cell background
ObjectPool.cs là code cho obejct pool nói chung
CellBackGround.cs code để spawn cell background
CellBackGround.cs đc tạo ở GameManager.cs và đc  gọi Initialte để spanw đúng số cell trong game setting
CellBackGround.cs reference đc pass cho BoardCController.cs và đc pass lại cho Board.cs
-> không còn Instantiate/Destroy mỗi khi vào level

2. làm object pooling cho item view (sprite của các viên match-3)
ItemViewPool.cs pool sprite cho tất cả 10 loại item (7 normal + 3 bonus) dùng Dictionary<string, Queue<Transform>>
Prefab được cache lại nên Resources.Load chỉ gọi 1 lần mỗi loại, không gọi lại mỗi lần spawn
ItemViewPool.cs được tạo ở GameManager.cs và gọi Initialize để pre-spawn boardSizeX * boardSizeY instance mỗi loại
ItemViewPool.cs reference được pass cho BoardController.cs rồi pass lại cho Board.cs
Board.cs gọi item.SetViewPool(m_itemViewPool) trước mỗi SetView() (trong Fill, FillGapsWithNewItems, ConvertNormalToBonus)
Item.cs dùng pool.Get() thay cho Instantiate, và pool.Return() thay cho Destroy trong ExplodeView và Clear
Kết quả: không còn Instantiate/Destroy liên tục khi player phá item và item spawn mới từ trên 

3. cache SpriteRenderer trong Item.cs — bỏ GetComponent mỗi lần swap
Trước: mỗi lần player swap 2 viên, SetSortingLayerHigher() và SetSortingLayerLower() gọi View.GetComponent<SpriteRenderer>() 2 lần
Sau: thêm field m_spriteRenderer, cache 1 lần duy nhất trong SetView() khi View được tạo
SetSortingLayerHigher/Lower dùng m_spriteRenderer trực tiếp thay vì GetComponent
An toàn vì SpriteRenderer luôn tồn tại cùng lifecycle với View — không bao giờ bị stale
Kết quả: zero GetComponent call trong gameplay, truy cập trực tiếp qua cached reference

II. Theme system cho normal item
Tạo Theme.cs (ScriptableObject) chứa ThemeName và mảng 7 Sprite tương ứng 7 loại normal item
Mỗi Theme SO là 1 bộ skin — tạo bao nhiêu theme tùy ý bằng cách right-click Create -> Match3 -> Theme
GameManager.cs có list m_themes (kéo thả các Theme SO vào) và m_selectedThemeIndex (chọn theme nào bằng index trong Inspector)
SelectedTheme property trả về theme đang chọn, nếu index không hợp lệ hoặc list rỗng thì trả null (dùng sprite gốc của prefab)
Theme reference được pass theo chain: GameManager -> BoardController -> Board -> NormalItem
NormalItem.cs có SetTheme() nhận theme và ApplyThemeSprite() swap sprite trên SpriteRenderer sau khi SetView()
Board.cs gọi item.SetTheme() và item.ApplyThemeSprite() trong Fill() và FillGapsWithNewItems()
Kết quả: đổi theme chỉ cần thay đổi selectedThemeIndex trong Inspector, không cần sửa code

III. Reset Button (chơi lại level)
Thêm nút Reset trong UIPanelGame để player chơi lại level hiện tại mà không cần quay về main menu
Flow hoạt động giống pattern các nút khác (btnPause, btnClose):
  UIPanelGame.btnReset → OnClickReset() → UIMainManager.ResetLevel() → GameManager.RestartLevel()
GameManager.cs: thêm field m_currentLevelMode lưu mode (TIMER/MOVES) khi LoadLevel() được gọi
  RestartLevel() gọi ClearLevel() dọn board, huỷ m_levelCondition cũ, rồi gọi lại LoadLevel(m_currentLevelMode)
UIMainManager.cs: thêm ResetLevel() gọi thẳng m_gameManager.RestartLevel()
UIPanelGame.cs: thêm [SerializeField] Button btnReset, wire listener trong Awake (có null check để không crash nếu chưa gán)
  OnDestroy() cleanup cả 2 listener (btnPause + btnReset)
Bước Unity Editor: tạo Button trong Canvas/UIPanelGame, kéo vào slot btnReset trong Inspector

IV

V
