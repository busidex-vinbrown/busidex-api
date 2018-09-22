

CREATE PROCEDURE [dbo].[usp_updateGroupSharedById]
	-- Add the parameters for the stored procedure here
	@GroupId bigint,
	@UserId bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE UserGroupCard
	SET SharedById = @UserId 
	where GroupId = @GroupId AND UserId = @UserId
    
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_updateGroupSharedById] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

