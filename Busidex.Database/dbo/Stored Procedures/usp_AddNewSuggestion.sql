

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[usp_AddNewSuggestion]
	@Summary nvarchar(350),
	@Details nvarchar(max),
	@UserId bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   
	INSERT INTO [dbo].[Suggestion]
           ([Summary]
           ,[Details]
           ,[Votes]
           ,[CreatedBy]
           ,[Created]
           ,[Deleted])
     VALUES
           (@Summary
           ,@Details
           ,1
           ,@UserId
           ,GetDate()
           ,0  )

   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_AddNewSuggestion] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

