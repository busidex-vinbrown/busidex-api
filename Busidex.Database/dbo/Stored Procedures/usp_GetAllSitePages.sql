

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetAllSitePages]			

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   
	SELECT [PageId]
      ,[Action]
      ,[ControllerName]
      ,[Title]
      ,[Deleted]
  FROM [dbo].[Page]	with(nolock)
  WHERE Deleted = 0

   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetAllSitePages] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

