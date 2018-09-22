CREATE TABLE [dbo].[PhoneNumberType] (
    [PhoneNumberTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]              NVARCHAR (50) NOT NULL,
    [Deleted]           BIT           CONSTRAINT [DF_PhoneNumberType_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_PhoneNumberType] PRIMARY KEY CLUSTERED ([PhoneNumberTypeId] ASC)
);

