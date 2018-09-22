-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE busidex_Membership_UpdateFailedPasswordAnswerCount
	-- Add the parameters for the stored procedure here
	@failCount int,
	@WindowStart Datetime, 
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
	UPDATE busidex_Membership 
    SET FailedPasswordAnswerAttemptCount = @failCount,
    FailedPasswordAnswerAttemptWindowStart =  @WindowStart
    WHERE UserId = @userId AND ApplicationId = @appId
	end
