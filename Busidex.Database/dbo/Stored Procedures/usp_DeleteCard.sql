



CREATE PROCEDURE [dbo].[usp_DeleteCard]
	@CardId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE [dbo].[Card] SET
           [Updated] = GETDATE()
           ,[Deleted] = 1 
	WHERE CardId = @CardId
     
      
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_DeleteCard] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

