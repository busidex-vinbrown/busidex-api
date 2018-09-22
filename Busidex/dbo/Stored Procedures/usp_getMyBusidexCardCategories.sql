-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getMyBusidexCardCategories] 
	@cardIds varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select 
		c.[Name] as CategoryName,
		c.[CategoryId],
		cc.[CardId],
		cc.[CardCategoryId]
	from dbo.[CardCategory] cc	with(nolock)
	inner join dbo.udf_List2Table(@cardIds, ',') lst
	on cc.[CardId] = lst.item
	inner join dbo.[Category] c	with(nolock)
	on c.[CategoryId] = cc.[CategoryId]
	where c.Deleted = 0
	and cc.Deleted = 0
	
END
GRANT EXECUTE
    ON OBJECT::[dbo].[usp_getMyBusidexCardCategories] TO [vinbrown2_BUSIDEX_WUSR]