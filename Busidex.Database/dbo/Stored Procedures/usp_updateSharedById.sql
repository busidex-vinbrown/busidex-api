
CREATE PROCEDURE [dbo].[usp_updateSharedById]
	-- Add the parameters for the stored procedure here
	@CardId bigint,
	@UserId bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE UserCard
	SET SharedById = @UserId 
	where CardId = @CardId AND UserId = @UserId
    
END
