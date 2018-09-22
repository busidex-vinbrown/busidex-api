CREATE TABLE [dbo].[Group] (
    [GroupId]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserId]      BIGINT         NOT NULL,
    [Description] VARCHAR (1000) NOT NULL,
    [Created]     DATETIME       NOT NULL,
    [Updated]     DATETIME       NOT NULL,
    [Notes]       VARCHAR (MAX)  NULL,
    [Deleted]     BIT            CONSTRAINT [DF_Group_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED ([GroupId] ASC),
    CONSTRAINT [FK_Group_busidex_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[busidex_Users] ([UserId])
);

