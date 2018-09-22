-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE busidex_Membership_UpdateFailedPasswordAnswerAttemptCount
	-- Add the parameters for the stored procedure here
	@Count int,
	@WindowStart Datetime, 
	@UserName varchar(100),
	@ApplicationName varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @appId uniqueidentifier
	declare @userId bigint

	select @appId = ApplicationId from aspnet_Applications where ApplicationName = @ApplicationName
	select @userId = UserId from busidex_Users where userName = @UserName

    -- Insert statements for procedure here
		UPDATE busidex_Membership 
		SET FailedPasswordAnswerAttemptCount = @Count
		WHERE UserId = @userId AND ApplicationId = @appId
end