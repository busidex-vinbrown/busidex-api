
create PROCEDURE usp_UpdateCardFileId
	@CardId as bigint,
	@FrontFileId as uniqueidentifier,
	@BackFileId  as uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    Update Card
	SET FrontFileId = @FrontFileId,
		BackFileId = @BackFileId
	WHERE CardId = @CardId

END
GO
