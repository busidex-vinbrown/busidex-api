
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[usp_GetSuggestionById]
@SuggestionId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   
	SELECT	[SuggestionId],
			[Summary],
			[Details],
			[Votes],
			[CreatedBy],
			[Created],
			[Deleted]
  FROM [dbo].[Suggestion] s WITH(NOLOCK)
  WHERE s.SuggestionId = @SuggestionId
  AND s.deleted = 0

   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetSuggestionById] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

