CREATE TABLE [dbo].[PhoneNumber] (
    [PhoneNumberId]     BIGINT       IDENTITY (1, 1) NOT NULL,
    [PhoneNumberTypeId] INT          NOT NULL,
    [CardId]            BIGINT       NOT NULL,
    [Number]            VARCHAR (50) NOT NULL,
    [Extension]         VARCHAR (8)  NULL,
    [Created]           DATETIME     NOT NULL,
    [Updated]           DATETIME     NOT NULL,
    [Deleted]           BIT          CONSTRAINT [DF_PhoneNumber_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_PhoneNumber] PRIMARY KEY CLUSTERED ([PhoneNumberId] ASC),
    CONSTRAINT [FK_PhoneNumber_Card] FOREIGN KEY ([CardId]) REFERENCES [dbo].[Card] ([CardId]),
    CONSTRAINT [FK_PhoneNumber_PhoneNumberType] FOREIGN KEY ([PhoneNumberTypeId]) REFERENCES [dbo].[PhoneNumberType] ([PhoneNumberTypeId])
);


GO
CREATE NONCLUSTERED INDEX [Idx_Number]
    ON [dbo].[PhoneNumber]([Number] ASC);

