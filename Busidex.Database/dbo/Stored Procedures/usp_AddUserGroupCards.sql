
  --alter table UserGroup alter column PersonId bigint null

									 
CREATE PROCEDURE [dbo].[usp_AddUserGroupCards]
	@GroupId as bigint,
	@CardIds as varchar(1000),
	@UserId as bigint	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @today datetime = getdate()

    INSERT INTO [dbo].[UserGroupCard]
           ([GroupId]
           ,[CardId]
           ,[UserId]
           ,[PersonId]
           ,[SharedById]
           ,[Created]
		   ,[Updated]
           ,[Notes]
           ,[Deleted])
     SELECT @GroupId,
			lst.item,
			@UserId,
			c.OwnerId,
			null,
			@today,
			@today,
			null,
			0
	 FROM [dbo].[udf_List2Table](@CardIds, ',') lst
	 INNER JOIN dbo.[card] c
			ON lst.item = c.cardId
							 							 
	select @groupId as GroupId
		
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_AddUserGroupCards] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

