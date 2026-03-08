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

3.
