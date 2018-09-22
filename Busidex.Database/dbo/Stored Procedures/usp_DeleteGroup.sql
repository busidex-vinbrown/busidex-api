


  --alter table UserGroup alter column PersonId bigint null

									 
CREATE PROCEDURE [dbo].[usp_DeleteGroup]
	@GroupId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @today  datetime = GetDate()

    UPDATE [dbo].[Group] 
	SET     [Deleted] = 1
    WHERE [GroupId] = @GroupId
							 
	Select @GroupId as GroupId

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_DeleteGroup] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

