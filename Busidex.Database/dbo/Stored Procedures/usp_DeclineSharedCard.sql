


CREATE PROCEDURE [dbo].[usp_DeclineSharedCard]
	@UserId as bigint, 
	@CardId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE dbo.SharedCard
	Set Declined = 1
	Where ShareWith = @UserId and CardId = @CardId

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_DeclineSharedCard] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

