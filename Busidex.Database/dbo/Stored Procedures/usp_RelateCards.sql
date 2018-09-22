
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[usp_RelateCards]
	-- Add the parameters for the stored procedure here
	@OwnerId bigint,
	@RelatedCardId bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


INSERT INTO [dbo].[CardRelation]
           ([CardId]
           ,[RelatedCardId]
           )
SELECT c.CardId,
	   @RelatedCardId
FROM dbo.Card c
WHERE c.OwnerId = @OwnerId
AND c.Deleted = 0
AND NOT EXISTS (
				SELECT cardId 
				FROM dbo.CardRelation cr
				WHERE c.CardId = cr.CardId
				AND cr.RelatedCardId = @RelatedCardId
				AND cr.Deleted = 0
				)


END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_RelateCards] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

