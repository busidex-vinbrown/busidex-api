


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[usp_GetBusigroupById]
@groupId bigint
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
	AND g.[GroupId] = @groupId
  

   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetBusigroupById] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

