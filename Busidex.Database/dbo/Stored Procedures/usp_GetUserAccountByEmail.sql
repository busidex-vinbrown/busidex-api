


									 
create PROCEDURE [dbo].[usp_GetUserAccountByEmail]
	@email as varchar(256)
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
  INNER JOIN dbo.[busidex_Membership] bm WITH(NOLOCK)
  ON ua.UserId = bm.UserId
  WHERE bm.Email = @email


END