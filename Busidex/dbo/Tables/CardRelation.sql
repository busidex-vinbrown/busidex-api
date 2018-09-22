CREATE TABLE [dbo].[CardRelation] (
    [CardRelationId] BIGINT IDENTITY (1, 1) NOT NULL,
    [CardId]         BIGINT NOT NULL,
    [RelatedCardId]  BIGINT NOT NULL,
    [Deleted]        BIT    CONSTRAINT [DF_CardRelation_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_CardRelation] PRIMARY KEY CLUSTERED ([CardRelationId] ASC),
    CONSTRAINT [FK_CardRelation_Card] FOREIGN KEY ([CardId]) REFERENCES [dbo].[Card] ([CardId]),
    CONSTRAINT [FK_CardRelation_Card1] FOREIGN KEY ([RelatedCardId]) REFERENCES [dbo].[Card] ([CardId])
);

