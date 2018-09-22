

CREATE PROCEDURE usp_addCard
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
	@FrontFileId as uniqueidentifier,
	@BackFileId	 as uniqueidentifier,
	@DisplayType as varchar(3),
	@Markup as varchar(2000)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [dbo].[Card]
           ([Name]
           ,[Title]
           ,[FrontImage]
           ,[FrontType]
           ,[FrontOrientation]
           ,[BackImage]
           ,[BackType]
           ,[BackOrientation]
           ,[BusinessId]
           ,[Searchable]
           ,[CompanyName]
           ,[Email]
           ,[Url]
           ,[Created]
           ,[CreatedBy]
           ,[OwnerId]
           ,[OwnerToken]
           ,[Updated]
           ,[Deleted]
           ,[FrontFileId]
           ,[BackFileId]
		   ,[DisplayType]
		   ,[Markup])
     VALUES
           (@Name,
			@Title,
			@FrontImage,
			@FrontType,
			@FrontOrientation,
			@BackImage,
			@BackType,
			@BackOrientation,
			@BusinessId,
			@Searchable,
			@CompanyName,
			@Email,
			@Url,
			GETDATE(),
			@CreatedBy,
			@OwnerId,
			@OwnerToken,
			GETDATE(),
			0,
			@FrontFileId,
			@BackFileId,
			@DisplayType,
			@Markup	)

		DECLARE @newCardId as bigint
		Select @newCardId = SCOPE_IDENTITY()

		Select @newCardId as CardId
END
GO
