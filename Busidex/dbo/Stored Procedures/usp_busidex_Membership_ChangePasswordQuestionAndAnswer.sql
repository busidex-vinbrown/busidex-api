-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE usp_busidex_Membership_ChangePasswordQuestionAndAnswer
	-- Add the parameters for the stored procedure here
	@Question varchar(255),
	@Answer varchar(255),
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
	SET PasswordQuestion = @Question, 
	PasswordAnswer = @Answer
	WHERE UserId = @userId AND ApplicationId = @appId
	

END