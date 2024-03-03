CREATE PROCEDURE [dbo].[sp_GetUserBalances]
    @UserId int
AS
    SELECT b.Balance, b.Currency 
    FROM UserBalance b
    Where UserId = @UserID 

RETURN 0
