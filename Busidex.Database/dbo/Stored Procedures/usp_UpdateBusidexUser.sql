


									 
create PROCEDURE [dbo].[usp_UpdateBusidexUser]	
	@userId as bigint,
	@email as varchar(256),
	@userName as varchar(256)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE dbo.[busidex_Users]
	SET UserName = @userName,
		LoweredUserName = LOWER(@userName),
		LastActivityDate = GETDATE()
	WHERE UserId = @userId

	UPDATE dbo.[busidex_Membership]
	SET Email = @email,
		LoweredEmail = LOWER(@email)
	WHERE UserId = @userId

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_UpdateBusidexUser] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

