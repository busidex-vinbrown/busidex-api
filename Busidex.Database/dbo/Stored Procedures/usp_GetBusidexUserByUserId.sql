



									 
CREATE PROCEDURE [dbo].[usp_GetBusidexUserByUserId]
	@UserId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT bu.[ApplicationId]
      ,bu.[UserId]
      ,bu.[UserName]
      ,bu.[LoweredUserName]
      ,bu.[MobileAlias]
      ,bu.[IsAnonymous]
      ,bu.[LastActivityDate]
      ,bm.[Email]
  FROM [dbo].[busidex_Users] bu WITH(NOLOCK)
  INNER JOIN [dbo].[busidex_Membership] bm WITH(NOLOCK)
  ON	bu.UserId = bm.UserId
  WHERE bu.UserId = @UserId


END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetBusidexUserByUserId] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

