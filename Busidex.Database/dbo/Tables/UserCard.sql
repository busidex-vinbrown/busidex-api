CREATE TABLE [dbo].[UserCard] (
    [UserCardId] BIGINT        IDENTITY (1, 1) NOT NULL,
    [CardId]     BIGINT        NOT NULL,
    [UserId]     BIGINT        NOT NULL,
    [OwnerId]    BIGINT        NULL,
    [SharedById] BIGINT        NULL,
    [Created]    DATE          NOT NULL,
    [Notes]      VARCHAR (MAX) NULL,
    [Deleted]    BIT           CONSTRAINT [DF_UserCard_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_UserCard] PRIMARY KEY CLUSTERED ([UserCardId] ASC),
    CONSTRAINT [FK_UserCard_busidex_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[busidex_Users] ([UserId]),
    CONSTRAINT [FK_UserCard_busidex_Users1] FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[busidex_Users] ([UserId]),
    CONSTRAINT [FK_UserCard_Card] FOREIGN KEY ([CardId]) REFERENCES [dbo].[Card] ([CardId])
);

