

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getCustomContentById]
@ContentId bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   
	SELECT [ContentId]
      ,[PageContent]
  FROM [dbo].[CustomContent] c with(nolock)
  WHERE c.ContentId = @ContentId

   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_getCustomContentById] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

