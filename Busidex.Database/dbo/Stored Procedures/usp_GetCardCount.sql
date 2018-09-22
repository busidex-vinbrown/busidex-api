
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetCardCount] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select Count(CardId) as CardCount
	FROM dbo.[Card] c WITH(NOLOCK)
	WHERE c.Deleted = 0
	GROUP BY CardId

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetCardCount] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

