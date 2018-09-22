

CREATE PROCEDURE usp_GetSharedCardByUserId
	@UserId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [SharedCardId]
      ,[CardId]
      ,[SendFrom]
      ,[Email]
      ,[ShareWith]
      ,[SharedDate]
      ,[Accepted]
      ,[Declined]
  FROM [dbo].[SharedCard] WITH(nolock)
  where Accepted = 0
  AND Declined = 0
  AND ShareWith = @UserId

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetSharedCardByUserId] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

