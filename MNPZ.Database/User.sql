CREATE TABLE [dbo].[User]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(50) NOT NULL ,
    [Login] NVARCHAR(MAX) NOT NULL ,
    [Password] NVARCHAR(MAX) NOT NULL ,
    [IsOperator] BIT NOT NULL ,
)
