

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetPageByViewName]			
   @Action as varchar(50)
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
  AND Action = @Action

   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetPageByViewName] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

