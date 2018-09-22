

create PROCEDURE [dbo].[usp_UpdateUserAddress]
	@UserAddressId as bigint,
	@Address1 as varchar(50),
	@Address2 as varchar(50),
	@City as varchar(150),
	@State as varchar(2),
	@Zipcode as char(10),
	@Region as varchar(50),
	@Country as varchar(100),
	@Latitude as float,
	@Longitude as float
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @geolocation as Geography;
	SET @geolocation = geography::STPointFromText('POINT(' + CAST(@Longitude AS VARCHAR(10)) + ' ' + CAST(@Latitude AS VARCHAR(10)) + ')', 4326);

	UPDATE [dbo].[UserAddress]
    SET		Address1 = @Address1
           ,Address2 = @Address2
           ,City = @City
           ,State = @State
           ,ZipCode = @Zipcode
           ,Region = @Region
           ,Country = @Country
           ,GeoLocation = @geolocation
	WHERE UserAddressId = @UserAddressId

	
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_UpdateUserAddress] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

