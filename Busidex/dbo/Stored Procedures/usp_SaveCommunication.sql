-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE usp_SaveCommunication
	-- Add the parameters for the stored procedure here
	@EmailTemplateId int,
	@UserId bigint,
	@Email varchar(150),
	@DateSent datetime,
	@Failed bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

INSERT INTO [dbo].[Communication]
           ([EmailTemplateId]
           ,[UserId]
           ,[Email]
           ,[DateSent]
           ,[Failed])
     VALUES
           (@EmailTemplateId
           ,@UserId
           ,@Email
           ,@DateSent
           ,@Failed)
END