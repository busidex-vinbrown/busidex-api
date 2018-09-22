CREATE TABLE [dbo].[UserGroupCard] (
    [UserGroupCardId] BIGINT        IDENTITY (1, 1) NOT NULL,
    [GroupId]         BIGINT        NOT NULL,
    [CardId]          BIGINT        NULL,
    [UserId]          BIGINT        NOT NULL,
    [PersonId]        BIGINT        NULL,
    [SharedById]      BIGINT        NULL,
    [Created]         DATETIME      NOT NULL,
    [Updated]         DATETIME      NOT NULL,
    [Notes]           VARCHAR (MAX) NULL,
    [Deleted]         BIT           CONSTRAINT [DF_UserGroupCard_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_UserGroupCard] PRIMARY KEY CLUSTERED ([UserGroupCardId] ASC),
    CONSTRAINT [FK_UserGroupCard_busidex_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[busidex_Users] ([UserId]),
    CONSTRAINT [FK_UserGroupCard_busidex_Users1] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[busidex_Users] ([UserId]),
    CONSTRAINT [FK_UserGroupCard_Card] FOREIGN KEY ([CardId]) REFERENCES [dbo].[Card] ([CardId]),
    CONSTRAINT [FK_UserGroupCard_Group] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Group] ([GroupId])
);

