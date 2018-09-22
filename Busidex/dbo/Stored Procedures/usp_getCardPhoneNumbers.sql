
CREATE PROCEDURE usp_getCardPhoneNumbers 
	@cardIds varchar(max)
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
		ph.[Extension]
	from dbo.[PhoneNumber] ph	with(nolock)
	inner join dbo.udf_List2Table(@cardIds, ',') lst
	on ph.[CardId] = lst.item
	inner join dbo.[PhoneNumberType] pht	with(nolock)
	on ph.PhoneNumberTypeId = pht.PhoneNumberTypeId
	where ph.Deleted = 0
	and pht.Deleted = 0

END