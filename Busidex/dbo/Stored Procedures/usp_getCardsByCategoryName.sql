-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getCardsByCategoryName]
@category varchar(150)
AS
BEGIN
	SET NOCOUNT ON;

	declare @criteria1 varchar(151), @criteria2 varchar(151)

	select @criteria1 = '%' + @category
	select @criteria2 = @category + '%'
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
  INNER JOIN [dbo].[CardCategory] cc
  ON c.[CardId] = cc.[CardId]
  INNER JOIN [dbo].[Category] cat
  ON cc.CategoryId = cat.CategoryId
  WHERE c.[CardId] > 1 
  AND c.[deleted] = 0
  AND cc.Deleted = 0
  AND cat.Deleted = 0
  AND (cat.[Name] Like @criteria1 or cat.[Name] Like @criteria2)

   
END