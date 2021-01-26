SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_Monthly_Spent_Amount_By_User]
	 @userId nvarchar(50) = null
	,@currency nvarchar(3) = null
AS
BEGIN
	SET NOCOUNT ON;

	SELECT SUM(T.Amount)
	FROM [dbo].[Transactions] T
	WHERE 
		UserId = @userId
		AND Currency = @currency
		AND ((SELECT MONTH(GETDATE())) = (SELECT MONTH(T.Date)) 
			AND (SELECT YEAR(GETDATE())) = (SELECT YEAR(T.Date)))
END
GO
