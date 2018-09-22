-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getCardsByNameKeyword]
@name varchar(150)
AS
BEGIN
	SET NOCOUNT ON;

	declare @criteria1 varchar(151), @criteria2 varchar(151)

	select @criteria1 = '%' + @name
	select @criteria2 = @name + '%'

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
  AND (c.[Name] LIKE @criteria1 OR c.[Name] LIKE @criteria2 )

   
END