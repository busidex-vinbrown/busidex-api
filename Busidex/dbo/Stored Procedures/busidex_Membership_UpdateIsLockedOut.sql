-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE busidex_Membership_UpdateIsLockedOut
	-- Add the parameters for the stored procedure here
	@IsLockedOut bit,
	@LastLockedOutDate Datetime, 
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
		SET IsLockedOut = @IsLockedOut, LastLockOutDate = @LastLockedOutDate
		WHERE UserId = @userId AND ApplicationId = @appId
end