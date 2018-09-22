

CREATE PROCEDURE usp_AddCardTag
	@CardId as bigint,
	@TagText as varchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @TagId bigint

	-- see if we have this tag already
	SELECT TOP 1 @TagId = TagId
	FROM dbo.Tag WITH(NOLOCK)
	WHERE Text  = @TagText 
	AND Deleted = 0
	 
	-- add the tag if it doesn't exist
	IF @TagId IS NULL
	BEGIN
		INSERT INTO [dbo].[Tag] 
			([Text] ,[Deleted])
		VALUES (@TagText ,0) 

		Select @TagId = SCOPE_IDENTITY()
	END

    INSERT INTO [dbo].[CardTag]
		(CardId, TagId)
    VALUES	(@CardId, @TagId)

	DECLARE @newCardTagId as bigint
	Select @newCardTagId = SCOPE_IDENTITY()

	Select @newCardTagId as CardTagId
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_AddCardTag] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];


GO


