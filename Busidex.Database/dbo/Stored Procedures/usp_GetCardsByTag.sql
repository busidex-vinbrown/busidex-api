
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetCardsByTag]
@tag varchar(150),
@latitude float,
@longitude float,
@radiusInMiles int
AS
BEGIN
	SET NOCOUNT ON;
	
	declare @UserLocation geography
	SET @UserLocation = geography::STPointFromText('POINT(' + CAST(@longitude AS VARCHAR(10)) + ' ' + CAST(@latitude AS VARCHAR(10)) + ')', 4326)

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
  left outer JOIN CardAddress ca with(nolock)
  ON c.CardId = ca.CardId
  INNER JOIN [dbo].[CardTag] ct
  ON c.[CardId] = ct.[CardId]
  INNER JOIN [dbo].[Tag] tag
  ON ct.TagId = tag.TagId
  WHERE c.[CardId] > 1 
  AND c.[deleted] = 0
  AND ct.Deleted = 0
  AND tag.Deleted = 0
  AND tag.[Text] = @tag
  AND ISNULL(@UserLocation.STDistance(ca.GeoLocation), 0) <= (@radiusInMiles*1609.344)

   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetCardsByTag] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

