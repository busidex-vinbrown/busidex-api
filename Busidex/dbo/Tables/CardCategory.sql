CREATE TABLE [dbo].[CardCategory] (
    [CardCategoryId] INT    IDENTITY (1, 1) NOT NULL,
    [CardId]         BIGINT NOT NULL,
    [CategoryId]     INT    NOT NULL,
    [Deleted]        BIT    CONSTRAINT [DF_CardCategory_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_CardCategory] PRIMARY KEY CLUSTERED ([CardCategoryId] ASC),
    CONSTRAINT [FK_CardCategory_Card] FOREIGN KEY ([CardId]) REFERENCES [dbo].[Card] ([CardId]),
    CONSTRAINT [FK_CardCategory_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([CategoryId])
);

