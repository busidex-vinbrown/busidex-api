CREATE TABLE [dbo].[CardTag] (
    [CardTagId] BIGINT IDENTITY (1, 1) NOT NULL,
    [CardId]    BIGINT NOT NULL,
    [TagId]     BIGINT NOT NULL,
    [Deleted]   BIT    CONSTRAINT [DF_CardTag_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_CardTag] PRIMARY KEY CLUSTERED ([CardTagId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TagId]
    ON [dbo].[CardTag]([TagId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CardId]
    ON [dbo].[CardTag]([CardId] ASC);

