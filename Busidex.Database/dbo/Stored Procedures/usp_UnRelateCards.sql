

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[usp_UnRelateCards]
	-- Add the parameters for the stored procedure here
	@OwnerId bigint,
	@RelatedCardId bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


UPDATE  cr 
SET cr.Deleted = 1
FROM [dbo].[CardRelation] cr
INNER JOIN dbo.Card c
	ON cr.CardId = c.CardId
WHERE c.OwnerId = @OwnerId
AND cr.RelatedCardId = @RelatedCardId
	
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_UnRelateCards] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

