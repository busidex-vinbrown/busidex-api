

									 
CREATE PROCEDURE usp_AddUserCard
	@CardId as bigint,
	@UserId as bigint,
	@OwnerId as bigint,
	@SharedById as bigint,
	@Notes as varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [dbo].[UserCard]
           ([CardId]
           ,[UserId]
           ,[OwnerId]
           ,[SharedById]
           ,[Created]
           ,[Notes]
           ,[Deleted])
     VALUES
           (@CardId
           ,@UserId
           ,@OwnerId
           ,@SharedById
           ,GETDATE()
           ,@Notes
           ,0)

		DECLARE @newUserCardId as bigint
		Select @newUserCardId = SCOPE_IDENTITY()

		Select @newUserCardId as UserCardId
END