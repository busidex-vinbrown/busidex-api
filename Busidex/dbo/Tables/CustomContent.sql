CREATE TABLE [dbo].[CustomContent] (
    [ContentId]   INT           IDENTITY (1, 1) NOT NULL,
    [PageContent] VARCHAR (MAX) NULL,
    CONSTRAINT [PK_CustomContent] PRIMARY KEY CLUSTERED ([ContentId] ASC)
);

