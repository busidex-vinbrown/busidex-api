

  --alter table UserGroup alter column PersonId bigint null

									 
CREATE PROCEDURE [dbo].[usp_RemoveUserGroupCards]
	@GroupId as bigint,
	@CardIds as varchar(1000),
	@UserId as bigint	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @today datetime = getdate()

    UPDATE ugc
	SET ugc.[deleted] = 1,
		ugc.[Updated] = @today
	FROM [dbo].[UserGroupCard] ugc
	INNER JOIN [dbo].[udf_List2Table](@CardIds, ',') as lst
			ON ugc.CardId = lst.item AND ugc.[GroupId] = @GroupId
    
							 							 
	select @groupId as GroupId
		
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_RemoveUserGroupCards] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

