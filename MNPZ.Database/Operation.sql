CREATE TABLE [dbo].[Operation]
(
    [Id]            INT              NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [UserId]        INT              NOT NULL,
    [Date]          DateTime         NOT NULL,
    [InAmount]      decimal(14,4)    NOT NULL,    
    [OutAmount]     decimal(14,4)    NOT NULL, 
    [Remainder]     decimal(14,4)    NOT NULL, 
    [ExchangeRate]   decimal(14,4)    NOT NULL, 
    [CurrencyIn]    INT              NOT NULL,
    [CurrencyOut]   INT              NULL,
    [IsExchange]    BIT              NOT NULL, 
    CONSTRAINT [FK_Operation_User] FOREIGN KEY ([UserId]) REFERENCES [User]([Id])
)

GO

CREATE INDEX [IX_Operation_CurrencyIn] ON [dbo].[Operation] ([CurrencyIn])
GO
CREATE INDEX [IX_Operation_CurrencyOut] ON [dbo].[Operation] ([CurrencyOut])
Go