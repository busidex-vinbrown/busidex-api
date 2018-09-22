


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetMyBusigroups]
@userId bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    
	SELECT
		 g.[GroupId],
		 g.[UserId],
		 g.[Created],
		 g.[Updated],
		 g.[Description],
		 g.[Notes]
  FROM [dbo].[Group] g WITH(NOLOCK)
  WHERE g.deleted = 0
	AND g.[UserId] = @userId
  

   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetMyBusigroups] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

