

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getAllCards]

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
      c.[FrontFileId],
      c.[BackFileId],
	  c.[Created],
	  c.[Updated]
  FROM [dbo].[Card] c	WITH(NOLOCK)
  WHERE c.[CardId] > 1 
  AND c.[deleted] = 0

   
END

GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_getAllCards] TO [vinbrown2]
--    AS [dbo];

