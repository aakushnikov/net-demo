CREATE TABLE [dbo].[Patterns]
(
	[Id] INT NOT NULL  IDENTITY, 
    [Value] NVARCHAR(MAX) NOT NULL, 
	[Hash] NVARCHAR(256) NOT NULL,
    [Timestamp] DATETIME NOT NULL DEFAULT Getutcdate(), 
    [Type] INT NOT NULL DEFAULT 0, 
    PRIMARY KEY ([Hash])
    
)
GO

CREATE NONCLUSTERED INDEX [IX_FK_Patterns_Id_Value]
	ON [dbo].[Patterns]([Id] ASC) INCLUDE([Value]);