create database PhatHanhSach

go

create table NXB
(
	[NXB_id] [int] primary key IDENTITY(1,1) NOT NULL,
	[NXB_name] [nvarchar](150) NULL,
	[Address] [nvarchar](255) NULL,
	[AcountNumber] [varchar](15) NULL,
	[Phone] [varchar](11)NULL,
	[Email] [varchar](35)NULL,
)
go

create table Agency
(
	[Agency_id] [int] primary key IDENTITY(1,1) NOT NULL,
	[Agency_name] [nvarchar](150) NULL,
	[Adress] [nvarchar](255) NULL,
	[Phone] [varchar](11) NULL,
)
go

create table Fields
(
	[Field_id] [int] primary key IDENTITY(1,1) NOT NULL,
	[Field_name] [nvarchar](150) NOT NULL,
)
go

create table Book
(
	[Book_id] [int] primary key IDENTITY(1,1) NOT NULL,
	[Book_name] [nvarchar](255) NULL,
	[Author] [nvarchar](150) NULL,
	[Field_id] [int] NOT NULL,
	[Quantity] [int] NULL,
	[Description] [nvarchar](max) NULL,
	[Cost_Export] [float] NULL,
	[NXB_id] [int] NULL,
	[image] [varchar](100) NULL,
	[Cost_Import] [float] NULL,

	foreign key ([NXB_id]) references NXB([NXB_id]),
	foreign key ([Field_id]) references Fields([Field_id]),
)
go

create table Bill_Import
(
	[Bill_Import_id] [int] primary key IDENTITY(1,1) NOT NULL,
	[NXB_id] [int] NOT NULL,
	[Total] [float] NOT NULL,
	[Deliver] [nvarchar](100) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,

	foreign key ([NXB_id]) references NXB([NXB_id]),
)
go

create table Detail_Bill_Import
(
	[id] [int] primary key IDENTITY(1,1) NOT NULL,
	[Bill_Import_id] [int] NOT NULL,
	[Book_id] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Cost] [float] NULL,
	[Total] [float] NULL,

	foreign key([Bill_Import_id]) references Bill_Import([Bill_Import_id]),
	foreign key([Book_id]) references Book([Book_id]),
)
go

create table Bill_Export
(
	[Bill_Export_id] [int] primary key IDENTITY(1,1) NOT NULL,
	[Agency_id] [int] NOT NULL,
	[Total] [float] NOT NULL,
	[Recipients] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,

	foreign key([Agency_id]) references Agency([Agency_id]),
)
go

create table Detail_Bill_Export
(
	[id] [int] primary key IDENTITY(1,1) NOT NULL,
	[Bill_Export_id] [int] NOT NULL,
	[Book_id] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Cost] [float] NOT NULL,
	[Total] [float] NOT NULL,

	foreign key([Bill_Export_id]) references Bill_Export([Bill_Export_id]),
	foreign key([Book_id]) references Book([Book_id]),
)
go

create table Inventory_Book
(
	[id] [int] primary key IDENTITY(1,1) NOT NULL,
	[Book_id] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,

	foreign key([Book_id]) references Book([Book_id]),
)
go

create table Inventory_Agency
(
	[id] [int] primary key IDENTITY(1,1) NOT NULL,
	[Agency_id] [int] NOT NULL,
	[Book_id] [int] NOT NULL,
	[deliver_quantity] [int] NULL,
	[repay_quantity] [int] NULL,
	[UpdatedDate] [datetime] NULL,

	foreign key([Book_id]) references Book([Book_id]),
	foreign key([Agency_id]) references Agency([Agency_id]),
)
go

create table Debt_Agency
(
	[id] [int] primary key IDENTITY(1,1) NOT NULL,
	[Agency_id] [int] NOT NULL,
	[debt] [float] NOT NULL,
	[repay] [float] NOT NULL,
	[update_date] [datetime] NULL,

	foreign key([Agency_id]) references Agency([Agency_id]),
)
go

create table Report_Agency
(
	[Report_id] [int] primary key IDENTITY(1,1) NOT NULL,
	[Agency_id] [int] NULL,
	[Total] [float] NULL,
	[CreatedDate] [datetime] NULL,

	foreign key([Agency_id]) references Agency([Agency_id]),
)
go

create table Detail_Report_Agency
(
	[id] [int] primary key IDENTITY(1,1) NOT NULL,
	[Report_id] [int] NOT NULL,
	[Book_id] [int] NOT NULL,
	[quantity] [int] NOT NULL,

	foreign key([Report_id]) references Report_Agency([Report_id]),
	foreign key([Book_id]) references Book([Book_id]),
)
go

create table Debt_NXB
(
	[id] [int] primary key IDENTITY(1,1) NOT NULL,
	[NXB_id] [int] NOT NULL,
	[debt] [float] NOT NULL,
	[repay] [float] NOT NULL,
	[update_date] [datetime] NOT NULL,

	foreign key([NXB_id]) references NXB([NXB_id]),
)
go

create table Report_NXB
(
	[Report_id] [int] primary key IDENTITY(1,1) NOT NULL,
	[NXB_id] [int] NOT NULL,
	[total] [float] NOT NULL,
	[update_date] [datetime] NULL,
	[status] [int] NOT NULL,

	foreign key([NXB_id]) references NXB([NXB_id]),
)

INSERT [dbo].[Agency] ( [Agency_name], [Adress], [Phone]) VALUES ( N'Đại lý A', N'Thành phố hồ chí minh', N'01234567891')
INSERT [dbo].[Agency] ( [Agency_name], [Adress], [Phone]) VALUES ( N'Đại lý B', N'Bình Dương', N'01287541267')
INSERT [dbo].[Agency] ( [Agency_name], [Adress], [Phone]) VALUES ( N'Đại lý C', N'Đà Nẵng', N'01258746235')

INSERT [dbo].[NXB] ( [NXB_name], [Address], [AcountNumber], [Phone], [Email]) VALUES ( N'NXB Trẻ', N'Thành phố Hồ Chí Minh', N'0158274625254', N'0123457824', N'nxbtre@gmail.com')
INSERT [dbo].[NXB] ( [NXB_name], [Address], [AcountNumber], [Phone], [Email]) VALUES ( N'NXB Thế Giới', N'Bình Dương', N'6462587615248', N'0123478568', N'nxbthegio@gmail.com')
INSERT [dbo].[NXB] ( [NXB_name], [Address], [AcountNumber], [Phone], [Email]) VALUES ( N'NXB Hội Nhà Văn', N'Vĩnh Long', N'2879425134462', N'0124785622', N'nxbhoinhavan@gmail.com')
INSERT [dbo].[NXB] ( [NXB_name], [Address], [AcountNumber], [Phone], [Email]) VALUES ( N'NXB Văn Hóa - Văn Nghệ', N'Bình Định', N'0578214235551', N'0147825635', N'nxbvhvn@gmail.com')

INSERT [dbo].[Fields] ( [Field_name]) VALUES ( N'Khoa Học')
INSERT [dbo].[Fields] ( [Field_name]) VALUES ( N'Công Nghệ')
INSERT [dbo].[Fields] ( [Field_name]) VALUES ( N'Xã Hội')
INSERT [dbo].[Fields] ( [Field_name]) VALUES ( N'Y Tế')
INSERT [dbo].[Fields] ( [Field_name]) VALUES ( N'Kinh Tế')

INSERT [dbo].[Book] ( [Book_name], [Author], [Field_id], [Quantity], [Description], [Cost_Export], [NXB_id], [image], [Cost_Import]) VALUES (N'Trên Đường Băng', N'Tony Buổi Sáng ', 3, 70, N'Trên Đường Băng tập hợp những bài viết được yêu thích trên facebook của Tony Buổi Sáng nhưng được chọn lọc và tổng hợp có mục đích, chủ đề nhằm mang đến kiến thưc, động lực và năng lượng cho bạn trẻ vào đời.
Cuốn Trên Đường Băng được chia làm 3 phần: “Chuẩn bị hành trang”, “Trong phòng chờ sân bay” và “Lên máy bay”, tương ứng với những quá trình một bạn trẻ phải trải qua trước khi “cất cánh” trên đường băng cuộc đời, bay vào bầu trời cao rộng.', 200000, 1, NULL, 140000)
INSERT [dbo].[Book] ( [Book_name], [Author], [Field_id], [Quantity], [Description], [Cost_Export], [NXB_id], [image], [Cost_Import]) VALUES ( N'Để Con Được Ốm', N'Uyên Bùi - BS. Trí Đoàn', 4, 40, N'Để con được ốm có thể coi là một cuốn nhật ký học làm mẹ thông qua những câu chuyện từ trải nghiệm thực tế mà chị Uyên Bùi đã trải qua từ khi mang thai đến khi em bé chào đời và trở thành một cô bé khỏe mạnh, vui vẻ. Cùng với những câu chuyện nhỏ thú vị của người mẹ là lời khuyên mang tính chuyên môn, giải đáp cụ thể từ bác sỹ Nguyễn Trí Đoàn, giúp hóa giải những hiểu lầm từ kinh nghiệm dân gian được truyền lại, cũng như lý giải một cách khoa học những thông tin chưa đúng đắn đang được lưu truyền hiện nay, mang đến góc nhìn đúng đắn nhất cho mỗi hiện tượng, sự việc với những kiến thức y khoa hiện đại được cập nhật liên tục. Cuốn sách sẽ giúp các bậc phụ huynh trang bị một số kiến thức cơ bản trong việc chăm sóc trẻ một cách khoa học và góp phần giúp các mẹ và những-người-sẽ-là-mẹ trở nên tự tin hơn trong việc chăm con, xua tan đi những lo lắng, để mỗi em bé ra đời đều được hưởng sự chăm sóc tốt nhất.', 150000, 2, NULL, 64000)
INSERT [dbo].[Book] ( [Book_name], [Author], [Field_id], [Quantity], [Description], [Cost_Export], [NXB_id], [image], [Cost_Import]) VALUES ( N'Nếu Biết Trăm Năm Là Hữu Hạn', N'Phạm Lữ Ân', 3, 80, N'Chỉ xuất hiện vỏn vẹn trong hơn bốn mươi bài viết trên chuyên mục Cảm thức của Bán nguyệt san 2! (số Chuyên đề của báo Sinh Viên Việt Nam), Phạm Lữ Ân là một tác giả đã âm thầm tạo nên hiện tượng đặc biệt trong văn hoá đọc của giới trẻ Việt nam hiện nay. Các bài viết của Phạm Lữ Ân được đăng tải, trích dẫn rất nhiều lần trên các trang web, trên blog cá nhân, đươc đọc trên Youtube, thành cảm hứng cho sáng tác ca khúc và cả kịch bản phim với những lời bình ưu ái.', 100000, 3, NULL, 60000)
INSERT [dbo].[Book] ( [Book_name], [Author], [Field_id], [Quantity], [Description], [Cost_Export], [NXB_id], [image], [Cost_Import]) VALUES ( N'Đắc Nhân Tâm', N'Dale Carnegie', 3 , 0 , N'Lời khuyên về cách thức cư xử, ứng xử và giao tiếp với mọi người để đạt được thành công trong cuộc sống', 250000,4,NULL,175000)
INSERT [dbo].[Book] ( [Book_name], [Author], [Field_id], [Quantity], [Description], [Cost_Export], [NXB_id], [image], [Cost_Import]) VALUES ( N'Think Like A Freak', N'LeVitt', 5,0,N'Tư duy như một kẻ lập dị', 100000,1,NULL,70000)
INSERT [dbo].[Book] ( [Book_name], [Author], [Field_id], [Quantity], [Description], [Cost_Export], [NXB_id], [image], [Cost_Import]) VALUES ( N'Tôi là một con lừa' , N'Lê Hoàng', 3, 0 , N'Nguyễn Phương Mai lên đường với trái tim trần trụi', 60000, 2,NULL, 30000)
INSERT [dbo].[Book] ( [Book_name], [Author], [Field_id], [Quantity], [Description], [Cost_Export], [NXB_id], [image], [Cost_Import]) VALUES ( N'Quá trẻ để chết' , N'Nhã Nam', 3, 0 , N'Hành trình trên đất Mĩ' , 65000, 2,NULL, 40000)
INSERT [dbo].[Book] ( [Book_name], [Author], [Field_id], [Quantity], [Description], [Cost_Export], [NXB_id], [image], [Cost_Import]) VALUES ( N'Your Name', N'Shinkai Makoto', 3, 0 , N'Cuộc đời của Mitsuha', 80000, 2 , NULL, 45000)
INSERT [dbo].[Book] ( [Book_name], [Author], [Field_id], [Quantity], [Description], [Cost_Export], [NXB_id], [image], [Cost_Import]) VALUES ( N'Cô gái trên tàu', N'Paula Hawkins', 3 , 0, N'Cuộc đời của Rachel', 120000, 3,NULL, 70000)
