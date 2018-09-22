

create PROCEDURE usp_AddSharedCard
	@CardId as bigint,
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

    INSERT INTO [dbo].[SharedCard]
           ([CardId]
           ,[SendFrom]
           ,[Email]
           ,[ShareWith]
           ,[SharedDate]
           ,[Accepted]
           ,[Declined])
     VALUES
           (@CardId
           ,@SendFrom
           ,@Email
           ,@ShareWith
           ,@SharedDate
           ,@Accepted
           ,@Declined)

		DECLARE @newSharedCardId as bigint
		Select @newSharedCardId = SCOPE_IDENTITY()

		Select @newSharedCardId as SharedCardId
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_AddSharedCard] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

