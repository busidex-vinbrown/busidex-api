


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetBusigroup]
@groupId bigint,
@includeImages bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    
	SELECT
		 ug.UserGroupCardId
	    ,ug.[SharedById]
		,ug.[GroupId]
		--,g.[Description]
		,ug.[Notes]
		,c.[CardId]
      ,c.[Name]
      ,c.[Title]
      ,c.[FrontType]
      ,c.[FrontOrientation]
      ,c.[BackType]
      ,c.[BackOrientation]
      ,c.[Searchable]
      ,c.[CompanyName]
      ,c.[Email]
      ,c.[Url]
      ,c.[CreatedBy]
      ,c.[OwnerId]
      ,c.[Deleted]
      ,c.[FrontFileId]
      ,c.[BackFileId]	  
	  ,CASE 
		WHEN @includeImages = 1 THEN c.[Markup] ELSE null
	   END AS [Markup]
	  ,CASE 
		WHEN @includeImages = 1 THEN c.[FrontImage] ELSE null
	   END AS [FrontImage]
	  ,CASE 
		WHEN @includeImages = 1 THEN c.[BackImage] ELSE null
	   END AS [BackImage]
  FROM [dbo].[Card] C WITH(NOLOCK)
  INNER JOIN [dbo].[UserGroupCard] ug WITH(NOLOCK)
  ON c.cardId = ug.cardId
  INNER JOIN [dbo].[Group] g WITH(NOLOCK)
  ON ug.GroupId = g.GroupId
  WHERE c.deleted = 0
	AND ug.deleted = 0 
	AND ug.GroupId = @groupId
	--(SELECT _ug.GroupId FROM [dbo].[UserGroupCard] _ug WITH(NOLOCK) WHERE _ug.userId = @userId AND _ug.deleted = 0) 
  

   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetBusigroup] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

