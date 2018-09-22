


									 
CREATE PROCEDURE [dbo].[usp_AddUserAccount]
	@UserId as bigint,
	@AccountTypeId as int,
	@Notes as varchar(1000),
	@ActivationToken as uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [dbo].[UserAccount]
           ([UserId]
           ,[AccountTypeId]
           ,[Created]
           ,[Active]
           ,[Notes]
           ,[ActivationToken])
     VALUES
           (@UserId
           ,@AccountTypeId
           ,getdate()
           ,0
           ,@Notes
           ,@ActivationToken)

		DECLARE @newUserAccountId as bigint
		Select @newUserAccountId = SCOPE_IDENTITY()

		SELECT	ua.[UserAccountId]
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
			where UserAccountId = @newUserAccountId

END

--GRANT Execute on usp_AddUserAccount to vinbrown2
--GRANT Execute on usp_AddUserAccount to vinbrown2_BUSIDEX_WUSR
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_AddUserAccount] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];


GO
GRANT EXECUTE
    ON OBJECT::[dbo].[usp_AddUserAccount] TO [vinbrown2_admin]
    AS [dbo];

