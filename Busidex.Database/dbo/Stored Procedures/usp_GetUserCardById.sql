

									 
create PROCEDURE [dbo].[usp_GetUserCardById]
	@userCardId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [UserCardId]
      ,[CardId]
      ,[UserId]
      ,[OwnerId]
      ,[SharedById]
      ,[Created]
      ,[Notes]
      ,[Deleted]
  FROM [dbo].[UserCard]	 WITH(nolock)
  where [UserCardId] = @userCardId 
  and Deleted = 0

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetUserCardById] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

