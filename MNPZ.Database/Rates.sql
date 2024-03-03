CREATE TABLE [dbo].[Rates]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CurInAmount]  decimal(14,4)   NOT NULL,
    [CurOutAmount] decimal(14,4)   NOT NULL,
    [CurIn] INT   NOT NULL,
    [CurOut] INT   NOT NULL,
 CONSTRAINT UC_Exchange UNIQUE ([CurIn],[CurOut])
)
