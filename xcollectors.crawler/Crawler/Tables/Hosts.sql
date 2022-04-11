CREATE TABLE [dbo].[Hosts]
(
	[Id] INT NOT NULL  IDENTITY, 
    [Name] NVARCHAR(200) NOT NULL, 
    [Url] NVARCHAR(500) NOT NULL, 
    [Timestamp] DATETIME NOT NULL DEFAULT Getutcdate(), 
    PRIMARY KEY ([Url])
)
