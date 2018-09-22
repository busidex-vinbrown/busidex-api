
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_getCardPhoneNumber] 
	@cardId bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select
		ph.[PhoneNumberId],
		ph.[PhoneNumberTypeId],
		pht.[Name],
		ph.[CardId],
		ph.[Number],
		ph.[Extension],
		ph.[Created],
		ph.[Updated],
		ph.[Deleted]
	from dbo.[PhoneNumber] ph	with(nolock)
	inner join dbo.[PhoneNumberType] pht	with(nolock)
	on ph.PhoneNumberTypeId = pht.PhoneNumberTypeId
	where ph.CardId = @cardId
	and ph.Deleted = 0
	and pht.Deleted = 0

END

GO
--GRANT EXECUTE
--    ON OBJECT::[dbo].[usp_getCardPhoneNumber] TO [vinbrown2_BUSIDEX_WUSR]
--    AS [dbo];

