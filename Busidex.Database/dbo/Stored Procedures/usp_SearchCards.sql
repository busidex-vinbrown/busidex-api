



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_SearchCards]
@criteria varchar(150),
@latitude float,
@longitude float,
@radiusInMiles int
AS
BEGIN
	SET NOCOUNT ON;

	declare @UserLocation geography
	SET @UserLocation = geography::STPointFromText('POINT(' + CAST(@longitude AS VARCHAR(10)) + ' ' + CAST(@latitude AS VARCHAR(10)) + ')', 4326)

	declare @fuzz varchar(151)
	declare @frontFuzz varchar(151)
	declare @backFuzz varchar(151)

	select @fuzz = '%' + @criteria	 + '%'
	select @frontFuzz = '%' + @criteria
	select @backFuzz = @criteria + '%'

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
      c.[BackFileId],
	  c.[Markup],
	  c.[DisplayType]
  FROM [dbo].[Card] C	WITH(NOLOCK)
  left outer JOIN CardAddress ca with(nolock)
  ON c.CardId = ca.CardId
  WHERE 
  (
  c.[Email] = @criteria OR
  c.[Name] LIKE @fuzz OR
  c.[Title] LIKE @fuzz	OR
  c.[CompanyName] LIKE @fuzz
  )
  AND c.[CardId] > 1 
  AND c.[deleted] = 0
  AND ISNULL(@UserLocation.STDistance(ca.GeoLocation), -1) <= (ISNULL(@radiusInMiles, 0)*1609.344)

  --UNION

  --SELECT
	 -- c.[CardId],
  --    c.[Name],
  --    c.[Title],
  --    c.[FrontType],
  --    c.[FrontOrientation],
  --    c.[BackType],
  --    c.[BackOrientation],
  --    c.[Searchable],
  --    c.[CompanyName],
  --    c.[Email],
  --    c.[Url],
  --    c.[CreatedBy],
  --    c.[Deleted],
  --    c.[Deleted],
  --    c.[OwnerId],
  --    c.[FrontFileId],
  --    c.[BackFileId],
	 -- c.[Markup],
	 -- c.[DisplayType]
  --FROM [dbo].[Card] c	WITH(NOLOCK)
  --left outer JOIN CardAddress ca with(nolock)
  --ON c.CardId = ca.CardId
  --INNER JOIN [dbo].PhoneNumber p
  --ON c.[CardId] = p.[CardId]
  --WHERE c.[CardId] > 1 
  --AND c.[deleted] = 0
  --AND p.Number = @criteria
  --AND ISNULL(@UserLocation.STDistance(ca.GeoLocation), -1) <= (ISNULL(@radiusInMiles, 0)*1609.344)

   UNION

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
      c.[BackFileId],
	  c.[Markup],
	  c.[DisplayType]
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
  AND tag.[Text] like @fuzz 
  AND ISNULL(@UserLocation.STDistance(ca.GeoLocation), -1) <= (ISNULL(@radiusInMiles, 0)*1609.344)

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_SearchCards] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

