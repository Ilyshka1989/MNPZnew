CREATE PROCEDURE [dbo].[sp_GetOperationsByUserId]
    @UserId int 
AS
    SELECT  o.InAmount,
    o.OutAmount,
    o.Remainder,
    o.CurrencyOut,
    o.CurrencyIn,
    o.[Date], 
    o.IsExchange,
    o.ExchangeRate,
    u.[Name] 
    FROM Operation o 
    JOIN [User] u ON u.Id = o.UserId 
    WHERE u.Id = @UserId

