




create PROCEDURE [dbo].[usp_AddSetting]
	@UserId	as bigint,
	@StartPage as int = null,
	@AllowGoogleSync as bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

INSERT INTO [dbo].[Settings]
           ([UserId]
           ,[StartPage]
           ,[Updated]
           ,[Deleted]
           ,[AllowGoogleSync])
     VALUES
           (@UserId
           ,@StartPage
           ,GetDate()
           ,0
           ,@AllowGoogleSync)

	DECLARE @newSettingsId as bigint
	Select @newSettingsId = SCOPE_IDENTITY()

	Select @newSettingsId as SettingsId
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_AddSetting] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

