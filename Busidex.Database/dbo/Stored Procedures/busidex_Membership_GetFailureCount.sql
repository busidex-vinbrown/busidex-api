-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[busidex_Membership_GetFailureCount]
	-- Add the parameters for the stored procedure here
	@userName varchar(100), 
	@appName varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @appId uniqueidentifier
	declare @userId bigint

	select @appId = ApplicationId from aspnet_Applications where ApplicationName = @appName
	select @userId = UserId from busidex_Users where userName = @userName

    -- Insert statements for procedure here
	SELECT FailedPasswordAttemptCount, 
        FailedPasswordAttemptWindowStart, 
        FailedPasswordAnswerAttemptCount, 
        FailedPasswordAnswerAttemptWindowStart 
        FROM busidex_Membership 
        WHERE UserId = @userId AND ApplicationId = @appId
END
