-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE usp_GetEmailTemplateByCode 
	 @code varchar(15)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [EmailTemplateId]
      ,[Code]
      ,[Subject]
      ,[Body]
  FROM [dbo].[EmailTemplate]	with(nolock)
  where code = @code

END