CREATE TRIGGER [dbo].[OperationAdd]
ON Operation
AFTER  INSERT ,UPDATE, DELETE 
AS

IF EXISTS(SELECT * FROM inserted WHERE IsExchange = 1)
BEGIN 
    Update b
    SET b.Balance  = b.Balance + i.InAmount - i.Remainder  
    FROM UserBalance b
    JOIN Inserted i ON i.CurrencyIn = b.Currency

    Update b
    SET b.Balance  = b.Balance - i.OutAmount  
    FROM UserBalance b
    JOIN Inserted i ON i.CurrencyOut = b.Currency
END

IF EXISTS(SELECT * FROM deleted)
BEGIN 
    Update b
    SET b.Balance  = b.Balance - i.InAmount + i.Remainder  
    FROM UserBalance b
    JOIN deleted i ON i.CurrencyIn= b.Currency

    Update b
    SET b.Balance  = b.Balance + i.OutAmount  
    FROM UserBalance b
    JOIN deleted i ON i.CurrencyOut = b.Currency
END
