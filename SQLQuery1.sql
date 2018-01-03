ALTER TABLE Detail_Bill_Export
ADD CONSTRAINT UC_Detail_Bill_Export UNIQUE (Bill_Eport_id,Book_id);

ALTER TABLE Detail_Bill_Import
ADD CONSTRAINT UC_Detail_Bill_Import UNIQUE (Bill_Import_id,Book_id);

ALTER TABLE Bill_Export
ADD FOREIGN KEY (Agency_id) REFERENCES Agency(Agency_id);

ALTER TABLE Bill_Import
ADD FOREIGN KEY (NXB_id) REFERENCES NXB(NXB_id);

ALTER TABLE Bill_Export
ADD FOREIGN KEY (User_id) REFERENCES Users(id);

ALTER TABLE Bill_Import
ADD FOREIGN KEY (User_id) REFERENCES Users(id);

ALTER TABLE Detail_Bill_Import
ADD FOREIGN KEY (Book_id) REFERENCES Book(Book_id);

ALTER TABLE Detail_Bill_Export
ADD FOREIGN KEY (Book_id) REFERENCES Book(Book_id);

ALTER TABLE Book
ADD FOREIGN KEY (Field_id) REFERENCES Fields(Field_id);

ALTER TABLE Book
ADD FOREIGN KEY (NXB_id) REFERENCES NXB(NXB_id);

ALTER TABLE Inventory_Book
ADD FOREIGN KEY (Book_id) REFERENCES Book(Book_id);

ALTER TABLE Debt_Agency
ADD FOREIGN KEY (Agency_id) REFERENCES Agency(Agency_id);

ALTER TABLE Detail_Bill_Import
ADD FOREIGN KEY (Bill_Import_id) REFERENCES Bill_Import(Bill_Import_id);

ALTER TABLE Detail_Bill_Export
ADD FOREIGN KEY (Bill_Export_id) REFERENCES Bill_Export(Bill_Export_id);

ALTER TABLE Detail_Bill_Import
ADD FOREIGN KEY (Book_id) REFERENCES Book(Book_id);

SET IDENTITY_INSERT dbo.Detail OFF

ALTER TABLE Report_Agency
ADD FOREIGN KEY (Agency_id) REFERENCES Agency(Agency_id);

ALTER TABLE Detail_Report_Agency
ADD FOREIGN KEY (Report_id) REFERENCES Report_Agency(Report_id);

ALTER TABLE Detail_Report_Agency
ADD FOREIGN KEY (Book_id) REFERENCES Book(Book_id);

ALTER TABLE Debt_Agency
ADD FOREIGN KEY (Agency_id) REFERENCES Agency(Agency_id);

ALTER TABLE Debt_NXB
ADD FOREIGN KEY (NXB_id) REFERENCES NXB(NXB_id);

ALTER TABLE Inventory_Agency
ADD FOREIGN KEY (Agency_id) REFERENCES Agency(Agency_id);

ALTER TABLE Inventory_Agency
ADD FOREIGN KEY (Book_id) REFERENCES Book(Book_id);


ALTER TABLE Report_NXB
ADD FOREIGN KEY (NXB_id) REFERENCES NXB(NXB_id);


ALTER TABLE Detail_Report_NXB
ADD FOREIGN KEY (Report_id) REFERENCES Report_NXB(Report_id);

ALTER TABLE Detail_Report_NXB
ADD FOREIGN KEY (Book_id) REFERENCES Book(Book_id);
