

create PROCEDURE usp_AddTag
	@Text as varchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [dbo].[Tag]
           (Text)
     VALUES
           (@Text)

	DECLARE @newTagId as bigint
	Select @newTagId = SCOPE_IDENTITY()

	Select @newTagId as TagId
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_AddTag] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

