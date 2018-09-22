

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getMyBusidex]
@userId bigint,
@includeImages bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    

	---- get the user's cards
	--;WITH OwnedCardsCTE (CardId, FrontFileId)
	--AS
	---- Define the CTE query.
	--(
	--	SELECT CardId, FrontFileId
	--	FROM dbo.Card c with(nolock)
	--	WHERE c.OwnerId = @userId
	--	AND c.Deleted = 0
	--)
	--Select cr.CardRelationId,
	--	cr.CardId,
	--	cr.RelatedCardId,
	--	c.FrontFileId as RelatedCardImageId
	--FROM OwnedCardsCTE c	
	--inner join dbo.UserCard uc with(nolock)
	--on c.CardId = uc.CardId
	--inner join dbo.CardRelation cr with(nolock)
	--on cr.RelatedCardId = uc.CardId
	--where uc.UserId = @userId
	--AND uc.deleted = 0
	--AND cr.deleted = 0

	Select cr.CardRelationId,
		cr.CardId,
		cr.RelatedCardId,
		c.FrontFileId as RelatedCardImageId
	FROM dbo.UserCard uc with(nolock)
	inner join dbo.CardRelation cr with(nolock)
	on cr.RelatedCardId = uc.CardId
	inner join dbo.Card c with(nolock)
	on uc.cardId = c.cardId
	where uc.UserId = @userId
	AND uc.deleted = 0
	AND cr.deleted = 0

	SELECT
		uc.UserCardId,
		uc.[SharedById]
		,uc.[Notes]
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
		WHEN @includeImages = 1 THEN c.[FrontImage] ELSE null
	   END
	  ,CASE 
		WHEN @includeImages = 1 THEN c.[BackImage] ELSE null
	   END
  FROM [dbo].[Card] C WITH(NOLOCK)
  INNER JOIN [dbo].[UserCard] uc WITH(NOLOCK)
  ON c.cardId = uc.cardId
  WHERE uc.userId = @userId 
  AND c.deleted = 0
  AND uc.deleted = 0

   
END

GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_getMyBusidex] TO [vinbrown2]
--    AS [dbo];

