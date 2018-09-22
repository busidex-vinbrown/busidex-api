

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].usp_GetMyBusidexImages
@userId bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    
	SELECT
		c.[FrontImage]
      ,c.[FrontFileId]
	  ,c.[FrontType]
  FROM [dbo].[Card] C WITH(NOLOCK)
  INNER JOIN [dbo].[UserCard] uc WITH(NOLOCK)
  ON c.cardId = uc.cardId
  WHERE uc.userId = @userId 
  AND c.deleted = 0
  AND uc.deleted = 0
  AND c.FrontImage IS NOT NULL
  AND c.FrontFileId IS NOT NULL

   
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_GetMyBusidexImages] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

