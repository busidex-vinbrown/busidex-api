CREATE TABLE [dbo].[StateCode] (
    [StateCodeId] INT          IDENTITY (1, 1) NOT NULL,
    [Code]        VARCHAR (2)  NOT NULL,
    [Name]        VARCHAR (40) NOT NULL,
    [Deleted]     BIT          CONSTRAINT [DF_StateCode_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_StateCode] PRIMARY KEY CLUSTERED ([StateCodeId] ASC)
);

