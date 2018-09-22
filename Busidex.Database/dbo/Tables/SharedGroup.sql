CREATE TABLE [dbo].[SharedGroup] (
    [SharedGroupId] BIGINT        IDENTITY (1, 1) NOT NULL,
    [GroupId]       BIGINT        NOT NULL,
    [SendFrom]      BIGINT        NOT NULL,
    [Email]         VARCHAR (200) NOT NULL,
    [ShareWith]     BIGINT        NULL,
    [SharedDate]    DATE          NOT NULL,
    [Accepted]      BIT           CONSTRAINT [DF_SharedGroup_Accepted] DEFAULT ((0)) NOT NULL,
    [Declined]      BIT           CONSTRAINT [DF_SharedGroup_Declined] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_SharedGroup] PRIMARY KEY CLUSTERED ([SharedGroupId] ASC),
    CONSTRAINT [FK_SharedGroup_Group] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Group] ([GroupId])
);

