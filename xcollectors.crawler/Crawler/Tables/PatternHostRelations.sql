CREATE TABLE [dbo].[PatternHostRelations]
(
	[Id] INT NOT NULL  IDENTITY, 
    [PatternId] INT NOT NULL, 
    [HostId] INT NOT NULL, 
    [Url] NVARCHAR(500) NOT NULL, 
    [Timestamp] DATETIME NOT NULL DEFAULT Getutcdate(), 
    [EncodingCode] INT NULL, 
    [UseBrowser] BIT NULL, 
    PRIMARY KEY ([Url], [PatternId], [HostId])

)
