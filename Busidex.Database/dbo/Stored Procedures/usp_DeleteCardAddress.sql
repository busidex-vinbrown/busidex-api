

create PROCEDURE [dbo].[usp_DeleteCardAddress]
	@CardAddressId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    Update [dbo].CardAddress
	SET Deleted = 1
	WHERE CardAddressId = @CardAddressId

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_DeleteCardAddress] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

