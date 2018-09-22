CREATE TABLE [dbo].[FeeSchedule] (
    [FeeScheduleId] INT            IDENTITY (1, 1) NOT NULL,
    [AccountTypeId] INT            NOT NULL,
    [Amount]        DECIMAL (6, 2) CONSTRAINT [DF_FeeSchedule_Amount] DEFAULT ((0)) NOT NULL,
    [ActiveFrom]    DATETIME       NOT NULL,
    [ActiveUntil]   DATETIME       NULL,
    [Deteted]       BIT            CONSTRAINT [DF_FeeSchedule_Deteted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_FeeSchedule] PRIMARY KEY CLUSTERED ([FeeScheduleId] ASC),
    CONSTRAINT [FK_FeeSchedule_AccountType] FOREIGN KEY ([AccountTypeId]) REFERENCES [dbo].[AccountType] ([AccountTypeId])
);

