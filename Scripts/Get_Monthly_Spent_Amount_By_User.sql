SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_Monthly_Spent_Amount_By_User]
	 @userId nvarchar(50) = null
	,@currency nvarchar(3) = null
	,@out decimal(13,4) OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;

	SELECT @out = SUM(T.Amount)
	FROM [dbo].[Transactions] T
	WHERE 
		UserId = @userId
		AND Currency = @currency
		AND ((SELECT MONTH(GETDATE())) = (SELECT MONTH(T.Date)) 
			AND (SELECT YEAR(GETDATE())) = (SELECT YEAR(T.Date)))

	 --If the user has no transactions then its 0, also works for null user but thats not a problem since the insert cannot be done on a null user
     SET @out = COALESCE(@out,0)			
END
