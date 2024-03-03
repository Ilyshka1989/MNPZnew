CREATE PROCEDURE [dbo].[sp_InsertOperation]
(
    @UserId int,
    @Date DATETIME,
    @InAmount decimal(14,4),
    @OutAmount decimal(14,4),
    @Remainder decimal(14,4),
    @CurrencyIn INT,
    @CurrencyOut INT,
    @ExchangeRate INT,
    @IsExchange INT)
AS
    INSERT INTO Operation (InAmount, OutAmount, Remainder, CurrencyIn , CurrencyOut, [Date], ExchangeRate, IsExchange, UserId)
VALUES (@InAmount,@OutAmount,@Remainder, @CurrencyIn, @CurrencyOut, @Date, @ExchangeRate,@IsExchange, @UserId) 

