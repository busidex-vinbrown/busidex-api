
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getCardById]
@cardId bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
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
			c.[OwnerId],
			c.[Deleted],
			c.[FrontFileId],
			c.[FrontImage],
			c.[BackFileId],
			c.[BackImage],
			c.[Updated],
			c.[Created]
  FROM [dbo].[Card] C WITH(NOLOCK)
  WHERE c.CardId = @cardId
  AND c.deleted = 0

   
END