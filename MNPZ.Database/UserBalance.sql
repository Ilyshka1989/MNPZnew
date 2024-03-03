CREATE TABLE [dbo].[UserBalance]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1),
    [UserId]    INT NOT NULL,
    [Balance]   DECIMAL (14,4) NOT NULL,
    [Currency]   INT NOT NULL,


)
