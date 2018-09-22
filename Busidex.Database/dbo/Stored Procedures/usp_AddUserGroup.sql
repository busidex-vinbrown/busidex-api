  --alter table UserGroup alter column PersonId bigint null

									 
CREATE PROCEDURE usp_AddUserGroup
	@GroupId as uniqueidentifier,
	@CardId as bigint,
	@UserId as bigint,
	@PersonId as bigint,
	@SharedById as bigint,
	@Description as varchar(1000),
	@Notes as varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [dbo].[UserGroup]
           ([GroupId]
           ,[CardId]
           ,[UserId]
           ,[PersonId]
           ,[SharedById]
           ,[Created]
           ,[Description]
           ,[Notes]
           ,[Deleted])
     VALUES
           (@GroupId
           ,@CardId
           ,@UserId
           ,@PersonId
		   ,@SharedById
           ,GetDate()
           ,@Description
           ,@Notes
           ,0)
							 							 
		DECLARE @newUserGroupId as bigint
		Select @newUserGroupId = SCOPE_IDENTITY()

		Select @newUserGroupId as UserGroupId
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_AddUserGroup] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

