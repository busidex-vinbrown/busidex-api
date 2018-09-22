CREATE PROCEDURE [dbo].[usp_GetSharedCardsByUserId]
	@UserId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [SharedCardId]
      ,[CardId]
      ,[SendFrom]
	  ,bm.[Email] as SendFromEmail
      ,sc.[Email]
      ,[ShareWith]
      ,[SharedDate]
      ,[Accepted]
      ,[Declined]
  FROM [dbo].[SharedCard] sc WITH(nolock)
  INNER JOIN [dbo].busidex_Membership bm WITH(nolock)
  ON sc.SendFrom = bm.UserId
  where ISNULL(Accepted, 0) = 0
  AND ISNULL(Declined, 0) = 0
  AND ShareWith = @UserId

END