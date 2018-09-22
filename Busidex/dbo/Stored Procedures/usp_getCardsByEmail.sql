-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE usp_getCardsByEmail
@email varchar(150)
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
  FROM [dbo].[Card] C	WITH(NOLOCK)
  WHERE c.[Email] = @email
  AND c.[CardId] > 1 
  AND c.[deleted] = 0

   
END