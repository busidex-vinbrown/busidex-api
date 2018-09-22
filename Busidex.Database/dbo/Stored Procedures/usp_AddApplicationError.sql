


									 
create PROCEDURE [dbo].[usp_AddApplicationError]	
	@message as varchar(2000),
	@innerException as varchar(2000),
	@stackTrace as varchar(max),
	@userId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [dbo].[ApplicationError]
           ([Message]
           ,[InnerException]
           ,[StackTrace]
           ,[ErrorDate]
           ,[UserId])
     VALUES
           (@message
           ,@innerException
           ,@stackTrace
           ,getdate()
           ,@userId)

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_AddApplicationError] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

