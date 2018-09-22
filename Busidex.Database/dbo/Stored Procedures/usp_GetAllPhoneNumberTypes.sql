
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[usp_GetAllPhoneNumberTypes] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [PhoneNumberTypeId]
		  ,[Name]
		  ,[Deleted]
	  FROM [dbo].[PhoneNumberType]	 with(nolock)
	  where deleted = 0

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetAllPhoneNumberTypes] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

