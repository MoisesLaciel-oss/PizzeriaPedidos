USE master;
GO

IF DB_ID('PizzeriaDB') IS NOT NULL
BEGIN
    ALTER DATABASE PizzeriaDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE PizzeriaDB;
END
GO

CREATE DATABASE PizzeriaDB;
GO

USE PizzeriaDB;
GO

CREATE TABLE Sizes (
    Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Sizes PRIMARY KEY,
    Name          NVARCHAR(50)  NOT NULL,
    Inches        INT           NULL,
    Slices        INT           NULL,
    CreatedDate   DATETIME2     NOT NULL CONSTRAINT DF_Sizes_CreatedDate DEFAULT (GETDATE()),
    ModifiedDate  DATETIME2     NULL,
    IsDeleted     BIT           NOT NULL CONSTRAINT DF_Sizes_IsDeleted DEFAULT (0),
    DeletedDate   DATETIME2     NULL
);

CREATE TABLE Ingredients (
    Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Ingredients PRIMARY KEY,
    Name          NVARCHAR(100) NOT NULL,
    ExtraPrice    DECIMAL(10,2) NOT NULL CONSTRAINT DF_Ingredients_ExtraPrice DEFAULT (0),
    IsAvailable   BIT           NOT NULL CONSTRAINT DF_Ingredients_IsAvailable DEFAULT (1),
    CreatedDate   DATETIME2     NOT NULL CONSTRAINT DF_Ingredients_CreatedDate DEFAULT (GETDATE()),
    ModifiedDate  DATETIME2     NULL,
    IsDeleted     BIT           NOT NULL CONSTRAINT DF_Ingredients_IsDeleted DEFAULT (0),
    DeletedDate   DATETIME2     NULL
);

CREATE TABLE Products (
    Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Products PRIMARY KEY,
    Name          NVARCHAR(100) NOT NULL,
    Description   NVARCHAR(300) NULL,
    Category      INT           NOT NULL CONSTRAINT DF_Products_Category DEFAULT (1),
    IsActive      BIT           NOT NULL CONSTRAINT DF_Products_IsActive DEFAULT (1),
    CreatedDate   DATETIME2     NOT NULL CONSTRAINT DF_Products_CreatedDate DEFAULT (GETDATE()),
    ModifiedDate  DATETIME2     NULL,
    IsDeleted     BIT           NOT NULL CONSTRAINT DF_Products_IsDeleted DEFAULT (0),
    DeletedDate   DATETIME2     NULL
);

CREATE TABLE ProductPrices (
    Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProductPrices PRIMARY KEY,
    ProductId     INT           NOT NULL CONSTRAINT FK_ProductPrices_Products REFERENCES Products(Id),
    SizeId        INT           NOT NULL CONSTRAINT FK_ProductPrices_Sizes REFERENCES Sizes(Id),
    Price         DECIMAL(10,2) NOT NULL CONSTRAINT CK_ProductPrices_Price CHECK (Price > 0),
    CreatedDate   DATETIME2     NOT NULL CONSTRAINT DF_ProductPrices_CreatedDate DEFAULT (GETDATE()),
    ModifiedDate  DATETIME2     NULL,
    IsDeleted     BIT           NOT NULL CONSTRAINT DF_ProductPrices_IsDeleted DEFAULT (0),
    DeletedDate   DATETIME2     NULL
);

CREATE TABLE ProductIngredients (
    Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProductIngredients PRIMARY KEY,
    ProductId     INT           NOT NULL CONSTRAINT FK_ProductIngredients_Products REFERENCES Products(Id),
    IngredientId  INT           NOT NULL CONSTRAINT FK_ProductIngredients_Ingredients REFERENCES Ingredients(Id),
    CreatedDate   DATETIME2     NOT NULL CONSTRAINT DF_ProductIngredients_CreatedDate DEFAULT (GETDATE()),
    ModifiedDate  DATETIME2     NULL,
    IsDeleted     BIT           NOT NULL CONSTRAINT DF_ProductIngredients_IsDeleted DEFAULT (0),
    DeletedDate   DATETIME2     NULL
);

CREATE TABLE Customers (
    Id                INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Customers PRIMARY KEY,
    FirstName         NVARCHAR(100) NOT NULL,
    LastName          NVARCHAR(100) NOT NULL,
    Phone             NVARCHAR(20)  NOT NULL,
    Email             NVARCHAR(150) NULL,
    Address           NVARCHAR(300) NULL,
    AddressReference  NVARCHAR(300) NULL,
    CreatedDate       DATETIME2     NOT NULL CONSTRAINT DF_Customers_CreatedDate DEFAULT (GETDATE()),
    ModifiedDate      DATETIME2     NULL,
    IsDeleted         BIT           NOT NULL CONSTRAINT DF_Customers_IsDeleted DEFAULT (0),
    DeletedDate       DATETIME2     NULL
);

CREATE TABLE Orders (
    Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Orders PRIMARY KEY,
    CustomerId    INT           NOT NULL CONSTRAINT FK_Orders_Customers REFERENCES Customers(Id),
    OrderDate     DATETIME2     NOT NULL,
    Type          INT           NOT NULL,
    Status        INT           NOT NULL,
    Subtotal      DECIMAL(12,2) NOT NULL,
    DeliveryFee   DECIMAL(12,2) NOT NULL CONSTRAINT DF_Orders_DeliveryFee DEFAULT (0),
    Tax           DECIMAL(12,2) NOT NULL,
    Total         DECIMAL(12,2) NOT NULL,
    Notes         NVARCHAR(500) NULL,
    CreatedDate   DATETIME2     NOT NULL CONSTRAINT DF_Orders_CreatedDate DEFAULT (GETDATE()),
    ModifiedDate  DATETIME2     NULL,
    IsDeleted     BIT           NOT NULL CONSTRAINT DF_Orders_IsDeleted DEFAULT (0),
    DeletedDate   DATETIME2     NULL
);

CREATE TABLE OrderDetails (
    Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_OrderDetails PRIMARY KEY,
    OrderId       INT           NOT NULL CONSTRAINT FK_OrderDetails_Orders REFERENCES Orders(Id),
    ProductId     INT           NOT NULL CONSTRAINT FK_OrderDetails_Products REFERENCES Products(Id),
    SizeId        INT           NOT NULL CONSTRAINT FK_OrderDetails_Sizes REFERENCES Sizes(Id),
    Quantity      INT           NOT NULL CONSTRAINT CK_OrderDetails_Quantity CHECK (Quantity > 0),
    UnitPrice     DECIMAL(10,2) NOT NULL,
    Amount        DECIMAL(12,2) NOT NULL,
    CreatedDate   DATETIME2     NOT NULL CONSTRAINT DF_OrderDetails_CreatedDate DEFAULT (GETDATE()),
    ModifiedDate  DATETIME2     NULL,
    IsDeleted     BIT           NOT NULL CONSTRAINT DF_OrderDetails_IsDeleted DEFAULT (0),
    DeletedDate   DATETIME2     NULL
);

CREATE TABLE Deliveries (
    Id             INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Deliveries PRIMARY KEY,
    OrderId        INT           NOT NULL CONSTRAINT FK_Deliveries_Orders REFERENCES Orders(Id),
    Address        NVARCHAR(300) NOT NULL,
    Courier        NVARCHAR(100) NULL,
    Status         INT           NOT NULL,
    DepartureDate  DATETIME2     NULL,
    DeliveredDate  DATETIME2     NULL,
    CreatedDate    DATETIME2     NOT NULL CONSTRAINT DF_Deliveries_CreatedDate DEFAULT (GETDATE()),
    ModifiedDate   DATETIME2     NULL,
    IsDeleted      BIT           NOT NULL CONSTRAINT DF_Deliveries_IsDeleted DEFAULT (0),
    DeletedDate    DATETIME2     NULL,
    CONSTRAINT UQ_Deliveries_OrderId UNIQUE (OrderId)
);
GO

INSERT INTO Sizes (Name, Inches, Slices) VALUES
 ('Personal',  8,  4),
 ('Medium',   12,  8),
 ('Large',    14, 10),
 ('Family',   16, 12),
 ('Single', NULL, NULL);

INSERT INTO Ingredients (Name, ExtraPrice) VALUES
 ('Mozzarella cheese', 75),
 ('Tomato sauce',       0),
 ('Pepperoni',         90),
 ('Ham',               85),
 ('Pineapple',         60),
 ('Corn',              50),
 ('Bell peppers',      50),
 ('Onion',             40),
 ('Mushrooms',         80),
 ('Basil',             40);

INSERT INTO Products (Name, Description, Category) VALUES
 ('Margherita Pizza', 'Tomato sauce, mozzarella and basil', 1),
 ('Pepperoni Pizza',  'Tomato sauce, mozzarella and pepperoni', 1),
 ('Hawaiian Pizza',   'Tomato sauce, mozzarella, ham and pineapple', 1),
 ('Veggie Pizza',     'Mozzarella, corn, bell peppers, onion and mushrooms', 1),
 ('Soda 20 oz',       'Bottled soft drink', 2),
 ('Garlic Bread',     'Six pieces with garlic butter', 3);

INSERT INTO ProductPrices (ProductId, SizeId, Price) VALUES
 (1, 1, 295), (1, 2, 495), (1, 3, 645), (1, 4, 795),
 (2, 1, 345), (2, 2, 575), (2, 3, 745), (2, 4, 895),
 (3, 1, 345), (3, 2, 575), (3, 3, 745), (3, 4, 895),
 (4, 1, 325), (4, 2, 545), (4, 3, 695), (4, 4, 845),
 (5, 5, 85),
 (6, 5, 175);

INSERT INTO ProductIngredients (ProductId, IngredientId) VALUES
 (1, 1), (1, 2), (1, 10),
 (2, 1), (2, 2), (2, 3),
 (3, 1), (3, 2), (3, 4), (3, 5),
 (4, 1), (4, 2), (4, 6), (4, 7), (4, 8), (4, 9);

INSERT INTO Customers (FirstName, LastName, Phone, Email, Address, AddressReference) VALUES
 ('Ana',  'Pérez', '809-555-0101', 'ana.perez@example.com',  'Calle 1 #10, Ensanche Ozama', 'Frente al colmado'),
 ('Luis', 'Gómez', '829-555-0102', 'luis.gomez@example.com', 'Av. Principal #25, Villa Mella', NULL);
GO
