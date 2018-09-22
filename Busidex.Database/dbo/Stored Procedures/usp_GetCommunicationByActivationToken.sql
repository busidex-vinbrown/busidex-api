
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[usp_GetCommunicationByActivationToken]
@Token uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT [CommunicationId]
      ,[EmailTemplateId]
      ,[UserId]
      ,[Email]
      ,[DateSent]
      ,[Failed]
      ,[SentById]
      ,[OwnerToken]
  FROM [dbo].[Communication] c WITH(NOLOCK)
  WHERE c.[OwnerToken] = @Token
   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetCommunicationByActivationToken] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

