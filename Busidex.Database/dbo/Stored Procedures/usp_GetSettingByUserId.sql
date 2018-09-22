


									 
create PROCEDURE [dbo].[usp_GetSettingByUserId]
	@UserId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [SettingsId]
      ,[UserId]
      ,[StartPage]
      ,[Updated]
      ,[Deleted]
      ,[AllowGoogleSync]
  FROM [dbo].[Settings]
  where [UserId] = @UserId 
  and Deleted = 0

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetSettingByUserId] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

