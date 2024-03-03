CREATE PROCEDURE [dbo].[sp_UpdateUserBalance]
(
    @UserId int,
    @Amounts [dbo].[UserBalanceItem]  readonly
)
AS

BEGIN TRY
BEGIN TRAN 
    UPDATE b
    SET b.Balance = b.Balance + a.Amount
    FROM UserBalance b 
    JOIN @Amounts a ON a.Currency = b.Currency
    Where b.UserId = @UserId


    IF(Exists (Select TOP(1) 1 
    FROM @Amounts a  
    LEFT JOIN UserBalance b ON  a.Currency = b.Currency 
    WHERE b.Id IS NULL AND a.Amount <> 0 )) 
    BEGIN 
       INSERT INTO UserBalance  (Balance,Currency, UserId) 
        SELECT a.Amount, a.Currency, @UserId 
        FROM @Amounts a  
        LEFT JOIN UserBalance b ON  a.Currency = b.Currency 
        WHERE b.Id IS NULL AND a.Amount <> 0 
    END  

COMMIT TRAN
END TRY 
BEGIN CATCH 

ROLLBACK TRAN
THROW
END CATCH
