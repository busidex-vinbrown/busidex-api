CREATE TABLE [dbo].[EmailTemplate] (
    [EmailTemplateId] INT           IDENTITY (1, 1) NOT NULL,
    [Code]            VARCHAR (15)  NOT NULL,
    [Subject]         VARCHAR (150) NOT NULL,
    [Body]            VARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_EmailTemplate] PRIMARY KEY CLUSTERED ([EmailTemplateId] ASC)
);

