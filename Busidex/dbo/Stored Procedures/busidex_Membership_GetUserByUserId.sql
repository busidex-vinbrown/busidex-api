CREATE PROCEDURE [dbo].[busidex_Membership_GetUserByUserId]
    @UserId               bigint,
    @CurrentTimeUtc       datetime,
    @UpdateLastActivity   bit = 0
AS
BEGIN
    IF ( @UpdateLastActivity = 1 )
    BEGIN
        UPDATE   dbo.busidex_Users 
        SET      LastActivityDate = @CurrentTimeUtc
        FROM     dbo.busidex_Users
        WHERE    @UserId = UserId

        IF ( @@ROWCOUNT = 0 ) -- User ID not found
            RETURN -1
    END

    SELECT  u.UserId, m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
            m.CreateDate, m.LastLoginDate, u.LastActivityDate,
            m.LastPasswordChangedDate, u.UserName, m.IsLockedOut,
            m.LastLockoutDate
    FROM    dbo.busidex_Users u, dbo.busidex_Membership m
    WHERE   @UserId = u.UserId AND u.UserId = m.UserId

    IF ( @@ROWCOUNT = 0 ) -- User ID not found
       RETURN -1

    RETURN 0
END
GO
GRANT EXECUTE
    ON OBJECT::[dbo].[busidex_Membership_GetUserByUserId] TO [aspnet_Membership_ReportingAccess]
    AS [dbo];

