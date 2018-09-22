-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE usp_busidex_Membership_GetUsernameByEmail
	-- Add the parameters for the stored procedure here
	@Email varchar(128),
	@ApplicationName varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @appId uniqueidentifier
	declare @userId bigint

	select @appId = ApplicationId from aspnet_Applications where ApplicationName = @ApplicationName
	--select @userId = UserId from busidex_Users where userName = @userName

    
	SELECT Username
	FROM busidex_Users bu	with(nolock)
	inner join busidex_membership bm
	on bu.UserId = bm.UserId
	WHERE bm.Email = @Email AND bu.ApplicationId = @appId
	

END