CREATE TABLE [dbo].[TempCard] (
    [TempCardId] INT             IDENTITY (1, 1) NOT NULL,
    [FrontImage] VARBINARY (MAX) NOT NULL,
    [BackImage]  VARBINARY (MAX) NULL,
    [FrontType]  VARCHAR (10)    NOT NULL,
    [BackType]   VARCHAR (10)    NULL,
    [Created]    DATETIME        NOT NULL,
    CONSTRAINT [PK_TempCard] PRIMARY KEY CLUSTERED ([TempCardId] ASC)
);

