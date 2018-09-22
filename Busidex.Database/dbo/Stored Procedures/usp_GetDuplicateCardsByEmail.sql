

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetDuplicateCardsByEmail]
@cardId bigint,
@email as varchar(150)
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
      c.[BackFileId]
  FROM [dbo].[Card] c	WITH(NOLOCK)
  WHERE c.[CardId] > 1 
  AND c.[deleted] = 0
  AND c.[cardId] <> @cardId
  AND ISNULL(c.[ownerId], 0) > 0 
  AND c.[EMail] = @email

   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetDuplicateCardsByEmail] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

