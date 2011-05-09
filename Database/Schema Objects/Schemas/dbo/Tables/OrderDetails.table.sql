CREATE TABLE [dbo].[OrderDetails] (
    [OrderId]   UNIQUEIDENTIFIER NOT NULL,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [UnitPrice] DECIMAL (18, 2)  NULL,
    [Quantity]  DECIMAL (18, 4)  NULL,
    [Discount]  DECIMAL (18, 4)  NULL
);

