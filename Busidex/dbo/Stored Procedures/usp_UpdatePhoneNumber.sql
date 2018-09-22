
CREATE PROCEDURE usp_UpdatePhoneNumber
	@PhoneNumberId as bigint,
	@PhoneNumberTypeId as int,
	@CardId as bigint,
	@Number as varchar(50),
	@Extension as varchar(8),
	@Deleted as bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE [dbo].[PhoneNumber]	SET
           [PhoneNumberTypeId]	= @PhoneNumberTypeId
           ,[CardId]= @CardId
           ,[Number] = @Number
           ,[Extension] = @Extension
           ,[Updated] = GETDATE()
           ,[Deleted] = @Deleted
	WHERE PhoneNumberId = @PhoneNumberId
   
END