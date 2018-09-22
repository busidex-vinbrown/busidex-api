


create PROCEDURE [dbo].[usp_AddSharedGroup]
	@GroupId as bigint,
	@SendFrom as bigint,
	@Email as varchar(200),
	@ShareWith as bigint,
	@SharedDate as datetime,
	@Accepted as bit,
	@Declined as bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [dbo].[SharedGroup]
           ([GroupId]
           ,[SendFrom]
           ,[Email]
           ,[ShareWith]
           ,[SharedDate]
           ,[Accepted]
           ,[Declined])
     VALUES
           (@GroupId
           ,@SendFrom
           ,@Email
           ,@ShareWith
           ,@SharedDate
           ,@Accepted
           ,@Declined)

		DECLARE @newSharedGroupId as bigint
		Select @newSharedGroupId = SCOPE_IDENTITY()

		Select @newSharedGroupId as SharedGroupId
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_AddSharedGroup] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

