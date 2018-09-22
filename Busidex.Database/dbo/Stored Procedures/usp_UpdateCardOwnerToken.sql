
	  
create PROCEDURE [dbo].[usp_UpdateCardOwnerToken]
	@CardId	as bigint,
	@OwnerToken as uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE [dbo].[Card]
		SET [OwnerToken] = @OwnerToken
			,[Updated] = GetDate()
	WHERE CardId = @CardId

 
	select @@rowcount as RowsUpdated 
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_UpdateCardOwnerToken] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

