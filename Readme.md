Cách tổ chức hiện tại khó scale lớn và làm việc nhóm
tôi sẽ tổ chức lại như sau
_Game

dễ làm việc vs các team hơn


chuyển update của board controller lên trên cùng, 
chuyển toàn bộ input trong update của input controller vào input handle funtion ( dễ đọc hơn)

1. 
a. làm obejct pooling cho cell background
ObjectPool.cs là code cho obejct pool nói chung
CellBackGround.cs code để spawn cell background
CellBackGround.cs đc tạo ở GameManager.cs và đc  gọi Initialte để spanw đúng số cell trong game setting
CellBackGround.cs reference đc pass cho BoardCController.cs và đc pass lại cho Board.cs

b.
