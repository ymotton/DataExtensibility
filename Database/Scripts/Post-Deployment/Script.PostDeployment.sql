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

