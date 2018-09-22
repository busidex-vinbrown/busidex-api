
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetCardByOwnerToken]
@Token uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT TOP 1
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
  left outer JOIN CardAddress ca with(nolock)
  ON c.CardId = ca.CardId
  WHERE c.[OwnerToken] = @Token
  AND c.[CardId] > 1 
  AND c.[deleted] = 0
   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetCardByOwnerToken] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

