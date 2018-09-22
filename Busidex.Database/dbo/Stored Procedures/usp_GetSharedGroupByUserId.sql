


CREATE PROCEDURE [dbo].[usp_GetSharedGroupByUserId]
	@UserId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [SharedGroupId]
      ,[GroupId]
      ,[SendFrom]
      ,[Email]
      ,[ShareWith]
      ,[SharedDate]
      ,[Accepted]
      ,[Declined]
  FROM [dbo].[SharedGroup] WITH(nolock)
  where Accepted = 0
  AND Declined = 0
  AND ShareWith = @UserId

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetSharedGroupByUserId] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

