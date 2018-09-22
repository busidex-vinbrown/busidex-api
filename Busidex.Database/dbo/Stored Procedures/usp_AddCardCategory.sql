


CREATE PROCEDURE usp_AddCardCategory
	@CardId as bigint,
	@CategoryId as int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [dbo].[CardCategory]
           ([CardId]
           ,[CategoryId]
           ,[Deleted])
     VALUES
           (@CardId,
            @CategoryId,
           0)

		DECLARE @newCardCategoryId as bigint
		Select @newCardCategoryId = SCOPE_IDENTITY()

		Select @newCardCategoryId as CardCategoryId
END
GO
