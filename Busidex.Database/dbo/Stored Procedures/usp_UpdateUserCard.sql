
create PROCEDURE usp_UpdateUserCard
	@UserCardId as bigint,
	@Notes as varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   UPDATE [dbo].[UserCard]
   SET [Notes] = @Notes
 WHERE UserCardId = @UserCardId
   
END