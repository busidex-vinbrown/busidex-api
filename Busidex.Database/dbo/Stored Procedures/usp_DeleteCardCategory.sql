
create PROCEDURE usp_DeleteCardCategory
	@CardCategoryId as bigint,
	@Deleted as bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE [dbo].[CardCategory]
	SET [Deleted] = 1
	WHERE CardCategoryId = @CardCategoryId

END
GO