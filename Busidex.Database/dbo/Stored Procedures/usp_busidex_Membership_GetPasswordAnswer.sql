-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE usp_busidex_Membership_GetPasswordAnswer
	-- Add the parameters for the stored procedure here
	@Username varchar(128),
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

    
	SELECT PasswordAnswer, IsLockedOut 
	FROM busidex_Membership
	WHERE UserId = @userId AND ApplicationId = @appId
	

END
