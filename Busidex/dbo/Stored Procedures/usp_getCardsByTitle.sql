-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE usp_getCardsByTitle
@title varchar(150)
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
  AND c.[Title] = @title

   
END