CREATE TABLE [dbo].[AccountHistory] (
    [AccountHistoryId] BIGINT   IDENTITY (1, 1) NOT NULL,
    [UserId]           BIGINT   NOT NULL,
    [AccountTypeId]    INT      NOT NULL,
    [Active]           BIT      NOT NULL,
    [Updated]          DATETIME NOT NULL,
    CONSTRAINT [PK_AccountHistory] PRIMARY KEY CLUSTERED ([AccountHistoryId] ASC)
);

