CREATE TABLE [dbo].[Customers] (
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [FirstName]  NVARCHAR (50)    NOT NULL,
    [LastName]   NVARCHAR (50)    NOT NULL,
    [Address]    NVARCHAR (50)    NULL,
    [PostalCode] NVARCHAR (50)    NULL,
    [City]       NVARCHAR (50)    NULL,
    [Country]    NVARCHAR (50)    NULL,
    [Telephone]  NVARCHAR (50)    NULL,
    [Email]      NVARCHAR (50)    NULL
);

