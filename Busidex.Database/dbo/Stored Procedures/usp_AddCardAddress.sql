

create PROCEDURE usp_AddCardAddress
	@CardId as bigint,
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

INSERT INTO [dbo].[CardAddress]
           ([CardId]
           ,[Address1]
           ,[Address2]
           ,[City]
           ,[State]
           ,[ZipCode]
           ,[Region]
           ,[Country]
           ,[GeoLocation])
     VALUES
           (@CardId
           ,@Address1
           ,@Address2
           ,@City
           ,@State
           ,@Zipcode
           ,@Region
           ,@Country
           ,@geolocation)

	DECLARE @newAddressId as bigint
	Select @newAddressId = SCOPE_IDENTITY()

	Select @newAddressId as AddressId
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_AddCardAddress] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

