CREATE PROCEDURE [dbo].[busidex_Membership_ChangePasswordQuestionAndAnswer]
    @ApplicationName       nvarchar(256),
    @UserName              nvarchar(256),
    @NewPasswordQuestion   nvarchar(256),
    @NewPasswordAnswer     nvarchar(128)
AS
BEGIN
    DECLARE @UserId bigint
    SELECT  @UserId = NULL
    SELECT  @UserId = u.UserId
    FROM    dbo.busidex_Membership m, dbo.busidex_Users u, dbo.aspnet_Applications a
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId
    IF (@UserId IS NULL)
    BEGIN
        RETURN(1)
    END

    UPDATE dbo.busidex_Membership
    SET		PasswordQuestion = ISNULL(@NewPasswordQuestion, PasswordQuestion), 
			PasswordAnswer = ISNULL(@NewPasswordAnswer, PasswordAnswer)
    WHERE  UserId=@UserId
    RETURN(0)
END