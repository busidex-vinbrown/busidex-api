-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE usp_DeleteInactiveUserAccount
	-- Add the parameters for the stored procedure here
	
AS
BEGIN

	declare @Now DateTime = getdate(),
	 @UserId               bigint = 1
	 
	while @UserId > 0
	begin
		
		SELECT @UserId = ua.UserId	FROM dbo.UserAccount ua	WHERE @Now > DateAdd(d, 2, ua.Created)
			
		DELETE FROM dbo.busidex_Membership WHERE @UserId = UserId
		DELETE FROM dbo.busidex_UsersInRoles WHERE @UserId = UserId
		DELETE FROM dbo.busidex_Profile WHERE @UserId = UserId
		DELETE FROM dbo.busidex_PersonalizationPerUser WHERE @UserId = UserId
		DELETE from dbo.UserAccount where @UserId = UserId

	end
END