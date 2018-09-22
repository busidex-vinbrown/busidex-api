CREATE TABLE [dbo].[Category] (
    [CategoryId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]       VARCHAR (150) NOT NULL,
    [Deleted]    BIT           CONSTRAINT [DF_Category_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([CategoryId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Idx_Name]
    ON [dbo].[Category]([Name] ASC);

