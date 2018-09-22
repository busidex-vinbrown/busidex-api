

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetCardRelations]
@cardId bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   
	SELECT	cr.[RelatedCardId]
	FROM [dbo].[CardRelation] cr WITH(NOLOCK)
    WHERE cr.CardId = @cardId
	AND cr.deleted = 0

   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetCardRelations] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

