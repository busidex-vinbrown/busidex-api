

create PROCEDURE usp_GetCardTagsByIds
	@CardIds as varchar(max)
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
  INNER JOIN udf_List2Table(@CardIds, ',') list
  on list.item = ct.CardId
  WHERE ct.Deleted = 0 AND t.Deleted = 0
  --AND ct.CardId = @CardId

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetCardTagsByIds] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

