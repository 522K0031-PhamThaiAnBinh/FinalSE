CREATE DATABASE test01;
USE test01;

-- Create Tables
CREATE TABLE [dbo].[Customers] (
    [CustomerID]  INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]   NVARCHAR (50)  NOT NULL,
    [LastName]    NVARCHAR (50)  NOT NULL,
    [Email]       NVARCHAR (100) NOT NULL,
    [PhoneNumber] NVARCHAR (20)  NOT NULL,
    [CreatedAt]   DATETIME       DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([CustomerID] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC)
);

CREATE TABLE [dbo].[MenuItems] (
    [MenuItemID]    INT             IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (100)  NOT NULL,
    [Description]   NVARCHAR (MAX)  NULL,
    [Price]         DECIMAL (10, 2) NOT NULL,
    [Category]      NVARCHAR (50)   NULL,
    [IsAvailable]   BIT             DEFAULT ((1)) NULL,
    [ImageFileName] NVARCHAR (255)  NULL,
    PRIMARY KEY CLUSTERED ([MenuItemID] ASC)
);

CREATE TABLE [dbo].[Reservations] (
    [ReservationID]       INT            IDENTITY (1, 1) NOT NULL,
    [CustomerID]          INT            NULL,
    [ReservationDate]     DATE           NOT NULL,
    [ReservationTime]     TIME (7)       NOT NULL,
    [NumberOfGuests]      INT            NOT NULL,
    [TableNumber]         INT            NULL,
    [Status]              NVARCHAR (20)  DEFAULT ('Confirmed') NULL,
    [SpecialInstructions] NVARCHAR (MAX) NULL,
    [CreatedAt]           DATETIME       DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([ReservationID] ASC),
    FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customers] ([CustomerID])
);

CREATE TABLE [dbo].[Orders] (
    [OrderID]       INT             IDENTITY (1, 1) NOT NULL,
    [ReservationID] INT             NULL,
    [TotalAmount]   DECIMAL (10, 2) NOT NULL,
    [OrderDate]     DATETIME        DEFAULT (getdate()) NULL,
    [Status]        NVARCHAR (20)   DEFAULT ('Pending') NULL,
    PRIMARY KEY CLUSTERED ([OrderID] ASC),
    CONSTRAINT [FK__Orders__Reservat__4222D4EF] FOREIGN KEY ([ReservationID]) REFERENCES [dbo].[Reservations] ([ReservationID]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[OrderDetails] (
    [OrderDetailID] INT             IDENTITY (1, 1) NOT NULL,
    [OrderID]       INT             NULL,
    [MenuItemID]    INT             NULL,
    [Quantity]      INT             NOT NULL,
    [SubTotal]      DECIMAL (10, 2) NOT NULL,
    PRIMARY KEY CLUSTERED ([OrderDetailID] ASC),
    FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Orders] ([OrderID]),
    FOREIGN KEY ([MenuItemID]) REFERENCES [dbo].[MenuItems] ([MenuItemID])
);

-- Insert Data
INSERT INTO [dbo].[Customers] ([FirstName], [LastName], [Email], [PhoneNumber])
VALUES
('John', 'Doe', 'john.doe@example.com', '123-456-7890'),
('Jane', 'Smith', 'jane.smith@example.com', '987-654-3210'),
('Robert', 'Johnson', 'robert.johnson@example.com', '555-123-4567'),
('Alice', 'Brown', 'alice.brown@example.com', '555-987-6543');

INSERT INTO [dbo].[MenuItems] ([Name], [Description], [Price], [Category], [IsAvailable], [ImageFileName])
VALUES
('Spaghetti Bolognese', 'Classic Italian pasta with rich meat sauce.', 12.99, 'Main Course', 1, 'spaghetti.jpg'),
('Caesar Salad', 'Fresh greens with Caesar dressing and croutons.', 7.99, 'Salad', 1, 'caesar_salad.jpg'),
('Grilled Chicken', 'Juicy grilled chicken breast served with mashed potatoes.', 14.49, 'Main Course', 1, 'grilled_chicken.jpg'),
('Chocolate Cake', 'Decadent chocolate cake with a rich ganache.', 5.99, 'Dessert', 1, 'chocolate_cake.jpg');

INSERT INTO [dbo].[Reservations] ([CustomerID], [ReservationDate], [ReservationTime], [NumberOfGuests], [TableNumber], [Status], [SpecialInstructions])
VALUES
(1, '2024-12-26', '18:30', 4, 5, 'Confirmed', 'Window seat'),
(2, '2024-12-27', '19:00', 2, 3, 'Confirmed', 'Allergy to nuts'),
(3, '2024-12-28', '20:00', 6, 8, 'Confirmed', 'Birthday celebration'),
(4, '2024-12-29', '17:45', 3, 4, 'Cancelled', 'N/A');

INSERT INTO [dbo].[Orders] ([ReservationID], [TotalAmount], [OrderDate], [Status])
VALUES
(1, 45.96, '2024-12-26 18:30', 'Pending'),
(2, 20.98, '2024-12-27 19:00', 'Completed'),
(3, 85.94, '2024-12-28 20:00', 'Pending'),
(4, 30.48, '2024-12-29 17:45', 'Cancelled');

INSERT INTO [dbo].[OrderDetails] ([OrderID], [MenuItemID], [Quantity], [SubTotal])
VALUES
(1, 1, 2, 25.98),  -- 2 Spaghetti Bolognese
(1, 2, 1, 7.99),   -- 1 Caesar Salad
(2, 3, 1, 14.49),  -- 1 Grilled Chicken
(2, 4, 1, 5.99),   -- 1 Chocolate Cake
(3, 1, 3, 38.97),  -- 3 Spaghetti Bolognese
(3, 4, 2, 11.98);  -- 2 Chocolate Cakes
