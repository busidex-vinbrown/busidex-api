
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[usp_GetAllUnownedCards]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT 
	  c.[CardId],
      c.[Name],
      c.[Title],
      c.[FrontType],
      c.[FrontOrientation],
      c.[BackType],
      c.[BackOrientation],
      c.[Searchable],
      c.[CompanyName],
      c.[Email],
      c.[Url],
      c.[CreatedBy],
      c.[Deleted],
      c.[Deleted],
      c.[OwnerId],
	  c.[OwnerToken],
      c.[FrontFileId],
      c.[BackFileId]
  FROM [dbo].[Card] C	WITH(NOLOCK)
  WHERE ISNULL(c.[OwnerId], 0) = 0
  AND c.[CardId] > 1 
  AND c.[deleted] = 0
   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetAllUnownedCards] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

