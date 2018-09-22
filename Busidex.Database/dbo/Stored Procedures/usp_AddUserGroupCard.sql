

  --alter table UserGroup alter column PersonId bigint null

									 
CREATE PROCEDURE [dbo].[usp_AddUserGroupCard]
	@GroupId as bigint,
	@CardId as bigint,
	@UserId as bigint,
	@PersonId as bigint,
	@SharedById as bigint,
	@Notes as varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @today  datetime = GetDate()

    INSERT INTO [dbo].[UserGroupCard]
           ([GroupId]
           ,[CardId]
           ,[UserId]
           ,[PersonId]
           ,[SharedById]
           ,[Created]
		   ,[Updated]
           ,[Notes]
           ,[Deleted])
     VALUES
           (@GroupId
           ,@CardId
           ,@UserId
           ,@PersonId
		   ,@SharedById
           ,@today
		   ,@today
           ,@Notes
           ,0)
							 							 
		DECLARE @newUserGroupCardId as bigint
		Select @newUserGroupCardId = SCOPE_IDENTITY()

		Select @newUserGroupCardId as UserGroupCardId
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_AddUserGroupCard] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

