
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[usp_GetActivePlans] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT	act.AccountTypeId,
			act.Name,
			act.Description,
			act.Active,
			act.DisplayOrder
	FROM dbo.[AccountType] act WITH(NOLOCK)
	WHERE act.[Active] = 1

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetActivePlans] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

