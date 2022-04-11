CREATE TABLE [dbo].[Auth]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserName] NVARCHAR(250) NOT NULL, 
    [Password] NVARCHAR(250) NOT NULL, 
    [HostId] INT NOT NULL, 
    [LoginUrl] NVARCHAR(500) NOT NULL, 
    [LoginField] NVARCHAR(250) NOT NULL, 
    [PasswordField] NVARCHAR(250) NOT NULL, 
	[SuccessIfContainValue] NVARCHAR(250) NOT NULL, 	
    [RefreshTime] INT NOT NULL DEFAULT 1440, 
    [IsEnabled] BIT NOT NULL DEFAULT 1, 
    [FirstPageUrl] NVARCHAR(500) NULL, 
)
