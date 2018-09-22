

CREATE PROCEDURE usp_GetAllStateCodes
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    Select StateCodeId, Code, Name
	from dbo.[StateCode]
	where Deleted = 0

END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetAllStateCodes] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

