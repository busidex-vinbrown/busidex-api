
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[usp_UpdateSuggestionVoteCount]
@SuggestionId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   
	UPDATE [dbo].[Suggestion] 
		   SET [Votes] = [Votes] + 1
			WHERE SuggestionId = @SuggestionId
  AND deleted = 0

   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_UpdateSuggestionVoteCount] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

