CREATE TYPE 
    [dbo].[UserBalanceItem] AS TABLE(  
    [Currency] [int] NOT NULL,  
    [Amount] [decimal](14,4) NOT NULL  
  )  