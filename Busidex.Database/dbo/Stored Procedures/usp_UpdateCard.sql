



CREATE PROCEDURE [dbo].[usp_updateCard]
	@CardId as bigint,
	@Name as varchar(150),
	@Title as varchar(150),
	@FrontImage as varbinary(max),
	@FrontType as varchar(10),
	@FrontOrientation as varchar(1),
	@BackImage as varbinary(max),
	@BackType as varchar(10),
	@BackOrientation as varchar(1),
	@BusinessId as int,
	@Searchable as bit,
	@CompanyName as varchar(150),
	@Email as varchar(150),
	@Url as varchar(250),
	@CreatedBy as bigint,
	@OwnerId as bigint,
	@OwnerToken as uniqueidentifier,
	@Deleted as bit = 0,
	@FrontFileId as uniqueidentifier = null,
	@BackFileId	 as uniqueidentifier = null,
	@DisplayType as varchar(3),
	@Markup as varchar(2000)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE [dbo].[Card] SET
           [Name] = @Name
           ,[Title] = @Title
           ,[FrontImage] = @FrontImage
           ,[FrontType] = @FrontType
           ,[FrontOrientation] = @FrontOrientation
           ,[BackImage] = @BackImage
           ,[BackType] = @BackType
           ,[BackOrientation] = @BackOrientation
           ,[BusinessId] = @BusinessId
           ,[Searchable] = @Searchable
           ,[CompanyName] = @CompanyName
           ,[Email] = @Email
           ,[Url] = @Url
           ,[CreatedBy] = @CreatedBy
           ,[OwnerId] = @OwnerId
           ,[OwnerToken] = @OwnerToken
           ,[Updated] = GETDATE()
           ,[Deleted] = @Deleted
           ,[FrontFileId] = @FrontFileId
           ,[BackFileId] = @BackFileId
		   ,[DisplayType] = @DisplayType
		   ,[Markup] = @Markup  
	WHERE CardId = @CardId
     
      
END
GO
