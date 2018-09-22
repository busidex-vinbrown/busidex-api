CREATE TABLE [dbo].[Card] (
    [CardId]           BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (150)    NULL,
    [Title]            VARCHAR (150)    NULL,
    [FrontImage]       VARBINARY (MAX)  NULL,
    [FrontType]        VARCHAR (10)     NULL,
    [FrontOrientation] VARCHAR (1)      CONSTRAINT [DF_Card_Orientation] DEFAULT ('H') NULL,
    [BackImage]        VARBINARY (MAX)  NULL,
    [BackType]         VARCHAR (10)     NULL,
    [BackOrientation]  VARCHAR (1)      CONSTRAINT [DF_Card_BackOrientation] DEFAULT ('H') NULL,
    [BusinessId]       INT              NULL,
    [Searchable]       BIT              CONSTRAINT [DF_Card_Private] DEFAULT ((0)) NOT NULL,
    [CompanyName]      VARCHAR (150)    NULL,
    [Email]            VARCHAR (150)    NULL,
    [Url]              VARCHAR (250)    NULL,
    [Created]          DATETIME         NOT NULL,
    [CreatedBy]        BIGINT           NULL,
    [OwnerId]          BIGINT           NULL,
    [OwnerToken]       UNIQUEIDENTIFIER NULL,
    [Updated]          DATETIME         NOT NULL,
    [Deleted]          BIT              CONSTRAINT [DF_Card_Deleted] DEFAULT ((0)) NOT NULL,
    [FrontFileId]      UNIQUEIDENTIFIER NULL,
    [BackFileId]       UNIQUEIDENTIFIER NULL,
    [DisplayType]      VARCHAR (3)      NULL,
    [Markup]           VARCHAR (2000)   NULL,
    CONSTRAINT [PK_Card_1] PRIMARY KEY CLUSTERED ([CardId] ASC),
    CONSTRAINT [FK_Card_busidex_Users] FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[busidex_Users] ([UserId]),
    CONSTRAINT [FK_Card_Business] FOREIGN KEY ([BusinessId]) REFERENCES [dbo].[Business] ([BusinessId])
);




GO
CREATE NONCLUSTERED INDEX [Idx_Email]
    ON [dbo].[Card]([Email] ASC);


GO
CREATE NONCLUSTERED INDEX [Idx_Title]
    ON [dbo].[Card]([Title] ASC);


GO
CREATE NONCLUSTERED INDEX [Idx_Name]
    ON [dbo].[Card]([Name] ASC);


GO
CREATE NONCLUSTERED INDEX [Idx_CompanyName]
    ON [dbo].[Card]([CompanyName] ASC);

