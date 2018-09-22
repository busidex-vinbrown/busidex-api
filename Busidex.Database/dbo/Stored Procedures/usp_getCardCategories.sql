-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[usp_getCardCategories] 
	@cardId bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select 
		c.[Name] as CategoryName,
		c.[CategoryId],
		cc.[CardCategoryId]
	from dbo.[CardCategory] cc	with(nolock)
	inner join dbo.[Category] c	with(nolock)
	on c.[CategoryId] = cc.[CategoryId]
	where cc.CardId = @cardId
	and c.Deleted = 0
	and cc.Deleted = 0

END
GRANT EXECUTE
    ON OBJECT::[dbo].[usp_getCardCategories] TO [vinbrown2_BUSIDEX_WUSR]
GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_getCardCategories] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

