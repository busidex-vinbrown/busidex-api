
-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

alter PROCEDURE usp_getCardPhoneNumbers 
	@cardIds varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select 
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
GO
