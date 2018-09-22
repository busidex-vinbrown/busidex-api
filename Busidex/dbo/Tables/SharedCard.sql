CREATE TABLE [dbo].[SharedCard] (
    [SharedCardId] BIGINT        IDENTITY (1, 1) NOT NULL,
    [CardId]       BIGINT        NOT NULL,
    [SendFrom]     BIGINT        NOT NULL,
    [Email]        VARCHAR (200) NOT NULL,
    [ShareWith]    BIGINT        NULL,
    [SharedDate]   DATE          NOT NULL,
    [Accepted]     BIT           CONSTRAINT [DF_SharedCard_Accepted] DEFAULT ((0)) NOT NULL,
    [Declined]     BIT           CONSTRAINT [DF_SharedCard_Declined] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_SharedCard] PRIMARY KEY CLUSTERED ([SharedCardId] ASC),
    CONSTRAINT [FK_SharedCard_Card] FOREIGN KEY ([CardId]) REFERENCES [dbo].[Card] ([CardId])
);

