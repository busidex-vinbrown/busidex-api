

create PROCEDURE [dbo].[usp_UpdateUserGroupCard]
	@UserGroupCardId as bigint,
	@Notes as varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   UPDATE [dbo].[UserGroupCard]
   SET [Notes] = @Notes
 WHERE UserGroupCardId = @UserGroupCardId
   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_UpdateUserGroupCard] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

