

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getCardsByCompany]
@company varchar(50),
@latitude float,
@longitude float,
@radiusInMiles int
AS
BEGIN
	SET NOCOUNT ON;
	
	declare @UserLocation geography
	SET @UserLocation = geography::STPointFromText('POINT(' + CAST(@longitude AS VARCHAR(10)) + ' ' + CAST(@latitude AS VARCHAR(10)) + ')', 4326)

	declare @criteria varchar(50)
	select @criteria = '%' + @company	 + '%'

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
  WHERE c.[CardId] > 1 
  AND c.[deleted] = 0
  AND c.[CompanyName] LIKE @criteria
  AND ISNULL(@UserLocation.STDistance(ca.GeoLocation), -1) <= (ISNULL(@radiusInMiles, 0)*1609.344)

   
END

GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_getCardsByCompany] TO [vinbrown2]
--    AS [dbo];

