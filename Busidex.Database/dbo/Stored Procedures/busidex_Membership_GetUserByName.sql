

CREATE PROCEDURE [dbo].[busidex_Membership_GetUserByName]
    @ApplicationName      nvarchar(256),
    @UserName             nvarchar(256),
    @CurrentTimeUtc       datetime,
    @UpdateLastActivity   bit = 0
AS
BEGIN
    DECLARE @UserId bigint

    --IF (@UpdateLastActivity = 1)
    --BEGIN
    --    -- select user ID from aspnet_users table
    --    SELECT TOP 1 @UserId = u.UserId
    --    FROM    dbo.aspnet_Applications a, dbo.busidex_Users u, dbo.busidex_Membership m
    --    WHERE    LOWER(@ApplicationName) = a.LoweredApplicationName AND
    --            u.ApplicationId = a.ApplicationId    AND
    --            LOWER(@UserName) = u.LoweredUserName AND u.UserId = m.UserId

    --    IF (@@ROWCOUNT = 0) -- Username not found
    --        RETURN -1

    --    UPDATE   dbo.busidex_Users
    --    SET      LastActivityDate = @CurrentTimeUtc
    --    WHERE    @UserId = UserId

    --    SELECT m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
    --            m.CreateDate, m.LastLoginDate, u.LastActivityDate, m.LastPasswordChangedDate,
    --            u.UserId, m.IsLockedOut, m.LastLockoutDate
    --    FROM    dbo.aspnet_Applications a, dbo.busidex_Users u, dbo.busidex_Membership m
    --    WHERE  @UserId = u.UserId AND u.UserId = m.UserId 
    --END
    --ELSE
    --BEGIN
        SELECT TOP 1 u.UserId, 
			m.Email, 
			m.PasswordQuestion, 
			m.Comment, 
			m.IsApproved,
            m.CreateDate, 
			m.LastLoginDate, 
			u.LastActivityDate,
            m.LastPasswordChangedDate, 
			u.UserName, 
			m.IsLockedOut,
            m.LastLockoutDate
        --FROM    dbo.aspnet_Applications a, dbo.busidex_Users u, dbo.busidex_Membership m
		FROM    dbo.busidex_Users u	   WITH(NOLOCK)
		INNER JOIN dbo.busidex_Membership m WITH(NOLOCK)
		ON	u.UserId = m.UserId
        WHERE    --LOWER(@ApplicationName) = a.LoweredApplicationName AND
                --u.ApplicationId = a.ApplicationId    AND
                --LOWER(@UserName) = u.LoweredUserName --AND u.UserId = m.UserId
				@UserName = u.UserName

        IF (@@ROWCOUNT = 0) -- Username not found
            RETURN -1
    END

    RETURN 0
--END
GO
GRANT EXECUTE
    ON OBJECT::[dbo].[busidex_Membership_GetUserByName] TO [aspnet_Membership_ReportingAccess]
    AS [dbo];

