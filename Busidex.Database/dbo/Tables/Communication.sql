CREATE TABLE [dbo].[Communication] (
    [CommunicationId] BIGINT           IDENTITY (1, 1) NOT NULL,
    [EmailTemplateId] INT              NULL,
    [UserId]          BIGINT           NOT NULL,
    [Email]           VARCHAR (150)    NOT NULL,
    [DateSent]        DATETIME         NOT NULL,
    [Failed]          BIT              CONSTRAINT [DF_Communication_Failed] DEFAULT ((0)) NOT NULL,
    [SentById]        BIGINT           NULL,
    [OwnerToken]      UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Communication] PRIMARY KEY CLUSTERED ([CommunicationId] ASC),
    CONSTRAINT [FK_Communication_EmailTemplate] FOREIGN KEY ([EmailTemplateId]) REFERENCES [dbo].[EmailTemplate] ([EmailTemplateId])
);



