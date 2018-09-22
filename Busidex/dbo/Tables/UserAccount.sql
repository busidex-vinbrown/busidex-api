CREATE TABLE [dbo].[UserAccount] (
    [UserAccountId]   BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserId]          BIGINT           NOT NULL,
    [AccountTypeId]   INT              NOT NULL,
    [Created]         DATETIME         CONSTRAINT [DF_UserAccount_Created] DEFAULT (getdate()) NOT NULL,
    [Active]          BIT              CONSTRAINT [DF_UserAccount_Active] DEFAULT ((1)) NOT NULL,
    [Notes]           VARCHAR (1000)   NULL,
    [ActivationToken] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_UserAccount] PRIMARY KEY CLUSTERED ([UserAccountId] ASC),
    CONSTRAINT [FK_UserAccount_AccountType] FOREIGN KEY ([AccountTypeId]) REFERENCES [dbo].[AccountType] ([AccountTypeId]),
    CONSTRAINT [FK_UserAccount_busidex_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[busidex_Users] ([UserId])
);

