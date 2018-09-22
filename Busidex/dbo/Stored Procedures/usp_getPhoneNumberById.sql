CREATE PROCEDURE [dbo].[usp_getPhoneNumberById]
	@id bigint
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
		ph.[Updated]
	from dbo.[PhoneNumber] ph	with(nolock)
	inner join dbo.[PhoneNumberType] pht	with(nolock)
	on ph.PhoneNumberTypeId = pht.PhoneNumberTypeId
	where ph.PhoneNumberId = @id
	and ph.Deleted = 0
	and pht.Deleted = 0

END