CREATE TABLE [dbo].[Products] (
    [ProductId]   UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (50)    NOT NULL,
    [Description] NVARCHAR (50)    NULL,
    [UnitPrice]   DECIMAL (18, 2)  NULL
);

