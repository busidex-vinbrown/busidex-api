-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE usp_busidex_Membership_UpdatePassword
	-- Add the parameters for the stored procedure here
	@Password varchar(255),
	@LastPasswordChangedDate datetime,
	@Username varchar(255),
	@ApplicationName varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @appId uniqueidentifier
	declare @userId bigint

	select @appId = ApplicationId from aspnet_Applications where ApplicationName = @ApplicationName
	select @userId = UserId from busidex_Users where userName = @userName

    
	UPDATE busidex_membership 
	SET Password = @Password, 
	LastPasswordChangedDate = @LastPasswordChangedDate
	WHERE UserId = @userId AND ApplicationId = @appId AND IsLockedOut = 0
	

END
