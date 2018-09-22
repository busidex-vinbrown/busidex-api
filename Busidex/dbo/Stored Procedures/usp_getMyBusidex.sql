-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getMyBusidex]
@userId bigint,
@includeImages bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	declare @myBusidex table (
		SharedById bigint null,
		Notes varchar(Max) null,
		[CardId] [bigint] NOT NULL,
		[Name] [varchar](150) NULL,
		[Title] [varchar](150) NULL,
		[FrontType] [varchar](10) NOT NULL,
		[FrontOrientation] [varchar](1) NULL,
		[BackType] [varchar](10) NULL,
		[BackOrientation] [varchar](1) NOT NULL,
		[Searchable] [bit] NOT NULL,
		[CompanyName] [varchar](150) NULL,
		[Email] [varchar](150) NULL,
		[Url] [varchar](250) NULL,
		[CreatedBy] [bigint] NULL,
		[OwnerId] [bigint] NULL,
		[Deleted] [bit] NOT NULL,
		[FrontFileId] [uniqueidentifier] NULL,
		[BackFileId] [uniqueidentifier] NULL,
		[FrontImage] varbinary(max),
		[BackImage] varbinary(max)
	)
	
	insert into @myBusidex
	SELECT
		uc.[SharedById]
		,uc.[Notes]
		,c.[CardId]
      ,c.[Name]
      ,c.[Title]
      ,c.[FrontType]
      ,c.[FrontOrientation]
      ,c.[BackType]
      ,c.[BackOrientation]
      ,c.[Searchable]
      ,c.[CompanyName]
      ,c.[Email]
      ,c.[Url]
      ,c.[CreatedBy]
      ,c.[OwnerId]
      ,c.[Deleted]
      ,c.[FrontFileId]
      ,c.[BackFileId]
	  ,CASE 
		WHEN @includeImages = 1 THEN c.[FrontImage] ELSE null
	   END
	  ,CASE 
		WHEN @includeImages = 1 THEN c.[BackImage] ELSE null
	   END
  FROM [dbo].[Card] C WITH(NOLOCK)
  INNER JOIN [dbo].[UserCard] uc WITH(NOLOCK)
  ON c.cardId = uc.cardId
  WHERE uc.userId = @userId 
  AND c.deleted = 0
  AND uc.deleted = 0

  select * from @myBusidex

  --select Number, Extension from PhoneNumber pn
  --inner join @myBusidex mb 
  --on pn.CardId = mb.CardId
   
END