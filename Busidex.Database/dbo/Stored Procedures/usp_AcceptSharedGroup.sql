



CREATE PROCEDURE [dbo].[usp_AcceptSharedGroup]
	@UserId as bigint, 
	@GroupId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE dbo.SharedGroup
	Set Accepted = 1
	Where ShareWith = @UserId and GroupId = @GroupId

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_AcceptSharedGroup] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

