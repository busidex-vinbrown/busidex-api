CREATE TABLE [dbo].[Suggestion] (
    [SuggestionId] INT            IDENTITY (1, 1) NOT NULL,
    [Summary]      NVARCHAR (350) NOT NULL,
    [Details]      NVARCHAR (MAX) NOT NULL,
    [Votes]        INT            CONSTRAINT [DF_Suggestion_Votes] DEFAULT ((0)) NOT NULL,
    [CreatedBy]    BIGINT         NOT NULL,
    [Created]      DATETIME       NOT NULL,
    [Deleted]      BIT            CONSTRAINT [DF_Suggestion_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Suggestion] PRIMARY KEY CLUSTERED ([SuggestionId] ASC)
);

