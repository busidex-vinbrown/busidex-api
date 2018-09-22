


									 
CREATE PROCEDURE [dbo].[usp_GetUserAccountByToken]
	@ActivationToken as uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [UserAccountId]
      ,ua.[UserId]
      ,ua.[AccountTypeId]
      ,ua.[Created]
      ,ua.[Active]
      ,ua.[Notes]
      ,ua.[ActivationToken]
	  ,acttype.[Name]
	  ,acttype.[Description]
	  ,acttype.[Active] as AccountTypeActive
	  ,acttype.DisplayOrder
  FROM [dbo].[UserAccount] ua WITH(NOLOCK)
  INNER JOIN dbo.[AccountType] acttype WITH(NOLOCK)
  ON ua.AccountTypeId = acttype.AccountTypeId
  WHERE ua.ActivationToken = @ActivationToken


END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetUserAccountByToken] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

