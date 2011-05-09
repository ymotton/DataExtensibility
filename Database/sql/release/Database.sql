/*
Deployment script for DataExtensibility
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "DataExtensibility"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS\MSSQL\DATA\"

GO
:on error exit
GO
USE [master]
GO
IF (DB_ID(N'$(DatabaseName)') IS NOT NULL
    AND DATABASEPROPERTYEX(N'$(DatabaseName)','Status') <> N'ONLINE')
BEGIN
    RAISERROR(N'The state of the target database, %s, is not set to ONLINE. To deploy to this database, its state must be set to ONLINE.', 16, 127,N'$(DatabaseName)') WITH NOWAIT
    RETURN
END

GO

IF NOT EXISTS (SELECT 1 FROM [master].[dbo].[sysdatabases] WHERE [name] = N'$(DatabaseName)')
BEGIN
    RAISERROR(N'You cannot deploy this update script to target QUADVERTEX\SQLEXPRESS. The database for which this script was built, DataExtensibility, does not exist on this server.', 16, 127) WITH NOWAIT
    RETURN
END

GO

IF (@@servername != 'QUADVERTEX\SQLEXPRESS')
BEGIN
    RAISERROR(N'The server name in the build script %s does not match the name of the target server %s. Verify whether your database project settings are correct and whether your build script is up to date.', 16, 127,N'QUADVERTEX\SQLEXPRESS',@@servername) WITH NOWAIT
    RETURN
END

GO

IF CAST(DATABASEPROPERTY(N'$(DatabaseName)','IsReadOnly') as bit) = 1
BEGIN
    RAISERROR(N'You cannot deploy this update script because the database for which it was built, %s , is set to READ_ONLY.', 16, 127, N'$(DatabaseName)') WITH NOWAIT
    RETURN
END

GO
USE [$(DatabaseName)]
GO
/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

GO
/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

DELETE FROM Customers;

INSERT INTO Customers (CustomerId, FirstName, LastName, Address, PostalCode, City, Country)
VALUES ('c493a891-fb3d-4c44-abea-62d1c2e4aa7d', 'Patsy D.', 'Thomas', '3784 Cantebury Drive', 'NY 10036', 'New York', 'United States');

INSERT INTO Customers (CustomerId, FirstName, LastName, Address, PostalCode, City, Country)
VALUES ('5c402b16-e4e9-4a5c-9c87-d8d8b64eedbb', 'Stacey T.', 'Vogelsang', '1509 Williams Lane', 'KS 67202', 'Wichita', 'United States');

INSERT INTO Customers (CustomerId, FirstName, LastName, Address, PostalCode, City, Country)
VALUES ('4618c0a3-d547-403d-b7a4-af56b0c66e87', 'Larry A.', 'Wagner', '3355 Friendship Lane', 'CA 94104', 'San Francisco', 'United States');

INSERT INTO Customers (CustomerId, FirstName, LastName, Address, PostalCode, City, Country)
VALUES ('3dd5ea86-7f75-4f14-87ba-9ac30a7b3ea1', 'Dennis T.', 'Miller', '4558 Ryder Avenue', 'WA 98208', 'Everett', 'United States');

INSERT INTO Customers (CustomerId, FirstName, LastName, Address, PostalCode, City, Country)
VALUES ('cfcb0c2a-5467-4a97-8748-00222e927e74', 'Karen G.', 'Pina', '3637 West Drive', 'IL 60457', 'Hickory Hills', 'United States');


GO
