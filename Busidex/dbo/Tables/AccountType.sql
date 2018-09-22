CREATE TABLE [dbo].[AccountType] (
    [AccountTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]          VARCHAR (50)  NOT NULL,
    [Description]   VARCHAR (180) NOT NULL,
    [Active]        BIT           CONSTRAINT [DF_AccountType_Active] DEFAULT ((1)) NOT NULL,
    [DisplayOrder]  INT           CONSTRAINT [DF_AccountType_DisplayOrder] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_AccountType] PRIMARY KEY CLUSTERED ([AccountTypeId] ASC)
);

