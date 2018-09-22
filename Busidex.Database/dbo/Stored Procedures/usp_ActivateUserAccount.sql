


CREATE PROCEDURE usp_ActivateUserAccount
	@UserAccountId bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE [dbo].[UserAccount] SET
           [Active] = 1
	WHERE UserAccountId = @UserAccountId
    
	select @@rowcount as RowsUpdated 
      
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_ActivateUserAccount] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

