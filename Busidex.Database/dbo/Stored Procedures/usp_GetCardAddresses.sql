

create PROCEDURE usp_GetCardAddresses
	@CardId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT ca.[CardAddressId]
      ,ca.[CardId]
      ,ca.[Address1]
      ,ca.[Address2]
      ,ca.[City]
      ,ca.[State]
      ,ca.[ZipCode]
      ,ca.[Region]
      ,ca.[Country]
      ,ca.[Deleted]
  FROM [dbo].CardAddress ca WITH(NOLOCK)
  WHERE ca.Deleted = 0 AND ca.CardId = @CardId

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetCardAddresses] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

