





CREATE PROCEDURE [dbo].[usp_DeleteUserCard]
	@CardId as bigint,
	@UserId as bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE [dbo].[UserCard] SET
           [Deleted] = 1 
	WHERE CardId = @CardId
	And UserId = @UserId
     
    -- check to see if there are no references to this card. if not, delete the card
	declare @testCardId bigint

	select @testCardId = uc.cardid 
	from UserCard uc with(nolock)
	where uc.CardId = @cardId  
	and uc.deleted = 0

	if @testCardId = null
	begin
		update Card 
		set deleted = 1 
		where cardId = @CardId	
	end
END
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_DeleteUserCard] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

