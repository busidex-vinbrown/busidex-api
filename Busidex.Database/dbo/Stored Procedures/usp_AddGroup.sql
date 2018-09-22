
  --alter table UserGroup alter column PersonId bigint null

									 
CREATE PROCEDURE [dbo].[usp_AddGroup]
	@UserId as bigint,
	@Description as varchar(1000),
	@Notes as varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @today  datetime = GetDate()

    INSERT INTO [dbo].[Group] (
            [UserId]
		   ,[Description]
           ,[Created]
		   ,[Updated]
           ,[Notes]
           ,[Deleted])
     VALUES
           (
           @UserId
		   ,@Description
		   ,@today
           ,@today
           ,@Notes
           ,0)
							 							 
		DECLARE @newGroupId as bigint
		Select @newGroupId = SCOPE_IDENTITY()

		Select @newGroupId as GroupId
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_AddGroup] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

