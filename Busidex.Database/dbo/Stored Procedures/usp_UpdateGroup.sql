

  --alter table UserGroup alter column PersonId bigint null

									 
CREATE PROCEDURE [dbo].[usp_UpdateGroup]
	@GroupId as bigint,
	@Description as varchar(1000),
	@Notes as varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @today  datetime = GetDate()

    UPDATE [dbo].[Group] 
	SET     [Description] = @Description
		   ,[Updated]	= @today
           ,[Notes]		= @Notes
    WHERE [GroupId] = @GroupId
							 
	Select @GroupId as GroupId

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_UpdateGroup] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

