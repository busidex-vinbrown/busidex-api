


CREATE PROCEDURE usp_AddPhoneNumber
	@PhoneNumberTypeId as int,
	@CardId as bigint,
	@Number as varchar(50),
	@Extension as varchar(8)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [dbo].[PhoneNumber]
           ([PhoneNumberTypeId]
           ,[CardId]
           ,[Number]
           ,[Extension]
           ,[Created]
           ,[Updated]
           ,[Deleted])
     VALUES
           (@PhoneNumberTypeId,
		   @CardId,
		   @Number,
		   @Extension,
		   GETDATE(),
		   GETDATE(),
		   0 )

		DECLARE @newPhoneNumberId as bigint
		Select @newPhoneNumberId = SCOPE_IDENTITY()

		Select @newPhoneNumberId as PhoneNumberId
END