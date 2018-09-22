CREATE TABLE [dbo].[ApplicationError] (
    [ApplicationErrorId] BIGINT         IDENTITY (1, 1) NOT NULL,
    [Message]            VARCHAR (2000) NULL,
    [InnerException]     VARCHAR (3000) NULL,
    [StackTrace]         VARCHAR (MAX)  NULL,
    [ErrorDate]          DATETIME       NULL,
    [UserId]             BIGINT         NULL,
    CONSTRAINT [PK_ApplicationError] PRIMARY KEY CLUSTERED ([ApplicationErrorId] ASC)
);

