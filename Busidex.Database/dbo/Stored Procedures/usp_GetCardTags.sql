

CREATE PROCEDURE usp_GetCardTags
	@CardId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [CardTagId]
      ,[CardId]
      ,ct.[TagId]
	  ,t.Text
  FROM [dbo].[CardTag] ct WITH(NOLOCK)
  INNER JOIN [dbo].Tag t WITH(NOLOCK)
  ON ct.TagId = t.TagId
  WHERE ct.Deleted = 0 AND t.Deleted = 0
  AND ct.CardId = @CardId

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetCardTags] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

