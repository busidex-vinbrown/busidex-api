

create PROCEDURE usp_DeleteCardTag
	@CardId as bigint,
	@TagId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    Update [dbo].CardTag
	SET Deleted = 1
	WHERE CardId = @CardId AND TagId = @TagId

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_DeleteCardTag] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

