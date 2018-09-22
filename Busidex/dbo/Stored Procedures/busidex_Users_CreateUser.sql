
CREATE PROCEDURE [dbo].[busidex_Users_CreateUser]
    @ApplicationId    uniqueidentifier,
    @UserName         nvarchar(256),
    @IsUserAnonymous  bit,
    @LastActivityDate DATETIME,
    @UserId           bigint OUTPUT
AS
BEGIN
    IF( @UserId IS NULL )
        SELECT @UserId = 0
    ELSE
    BEGIN
        IF( EXISTS( SELECT UserId FROM dbo.busidex_Users
                    WHERE @UserId = UserId ) )
            RETURN -1
    END

    INSERT dbo.busidex_Users (ApplicationId, UserName, LoweredUserName, IsAnonymous, LastActivityDate)
    VALUES (@ApplicationId, @UserName, LOWER(@UserName), @IsUserAnonymous, @LastActivityDate)

	select @UserId = SCOPE_IDENTITY()
    RETURN 0
END
GO
GRANT EXECUTE
    ON OBJECT::[dbo].[busidex_Users_CreateUser] TO [aspnet_Membership_FullAccess]
    AS [dbo];

