-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE isp_DeleteUserFromSystem
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @ApplicationName  nvarchar(256) = '/busidex',
    @UserName         nvarchar(256) = 'vinbrown2',-- 
    @TablesToDeleteFrom int = 8

    DECLARE @UserId               bigint
    SELECT  @UserId               = NULL

    SELECT  @UserId = u.UserId
    FROM    dbo.busidex_Users u, dbo.aspnet_Applications a
    WHERE   u.LoweredUserName       = LOWER(@UserName)
        AND u.ApplicationId         = a.ApplicationId
        AND LOWER(@ApplicationName) = a.LoweredApplicationName

    
    -- Delete from Membership table if (@TablesToDeleteFrom & 1) is set
    
        DELETE FROM dbo.busidex_Membership WHERE @UserId = UserId
    
    -- Delete from aspnet_UsersInRoles table if (@TablesToDeleteFrom & 2) is set
    
        DELETE FROM dbo.busidex_UsersInRoles WHERE @UserId = UserId
    
    -- Delete from aspnet_Profile table if (@TablesToDeleteFrom & 4) is set
        DELETE FROM dbo.busidex_Profile WHERE @UserId = UserId

    -- Delete from aspnet_PersonalizationPerUser table if (@TablesToDeleteFrom & 8) is set
        DELETE FROM dbo.busidex_PersonalizationPerUser WHERE @UserId = UserId
		
		DELETE from dbo.UserAccount where @UserId = UserId
		
		update dbo.card set OwnerId = null where OwnerId = @UserId

		DELETE FROM dbo.UserCard WHERE @UserId = UserId

		DELETE FROM dbo.Settings WHERE @UserId = UserId

    -- Delete from aspnet_Users table if (@TablesToDeleteFrom & 1,2,4 & 8) are all set
        DELETE FROM dbo.busidex_Users WHERE @UserId = UserId
END
