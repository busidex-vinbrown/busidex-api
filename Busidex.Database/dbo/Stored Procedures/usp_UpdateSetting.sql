   
CREATE PROCEDURE [dbo].[usp_UpdateSetting]
	@SettingsId	as bigint,
	@StartPage as int = null,
	@AllowGoogleSync as bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

UPDATE		[dbo].[Settings]  
SET         [StartPage]		=	@StartPage
           ,[Updated]		=	GETDATE()
           ,[AllowGoogleSync]	= @AllowGoogleSync
WHERE		SettingsId = @SettingsId

    
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_UpdateSetting] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

