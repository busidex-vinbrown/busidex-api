

create PROCEDURE usp_DeleteTag
	@TagId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    Update [dbo].Tag
	SET Deleted = 1
	WHERE  TagId = @TagId

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_DeleteTag] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

