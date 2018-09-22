



CREATE PROCEDURE [dbo].[usp_DeclineSharedGroup]
	@UserId as bigint, 
	@GroupId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE dbo.SharedGroup
	Set Declined = 1
	Where ShareWith = @UserId and GroupId = @GroupId

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_DeclineSharedGroup] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

