

CREATE PROCEDURE [dbo].[usp_GetUserAddress]
	@UserId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT ua.[UserAddressId]
      ,ua.[UserId]
      ,ua.[Address1]
      ,ua.[Address2]
      ,ua.[City]
      ,ua.[State]
      ,ua.[ZipCode]
      ,ua.[Region]
      ,ua.[Country]
	  ,ua.[GeoLocation].Lat as Latitude
	  ,ua.[GeoLocation].Long as Longitude
  FROM [dbo].UserAddress ua WITH(NOLOCK)
  WHERE ua.UserId = @UserId

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetUserAddress] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

