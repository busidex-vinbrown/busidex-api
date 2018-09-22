


CREATE PROCEDURE [dbo].[usp_GetAllCaptchas]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [CaptchaId]
      ,[CaptchaText]
      ,[Deleted]
  FROM [dbo].[Captcha]
	where Deleted = 0

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetAllCaptchas] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

