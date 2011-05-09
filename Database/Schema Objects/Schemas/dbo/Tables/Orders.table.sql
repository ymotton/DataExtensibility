CREATE TABLE [dbo].[Orders] (
    [OrderId]     UNIQUEIDENTIFIER NOT NULL,
    [CustomerId]  UNIQUEIDENTIFIER NOT NULL,
    [OrderDate]   DATETIME         NOT NULL,
    [ShippedDate] DATETIME         NULL
);

