CREATE TABLE [dbo].[Tag] (
    [TagId]   BIGINT       IDENTITY (1, 1) NOT NULL,
    [Text]    VARCHAR (50) NOT NULL,
    [Deleted] BIT          CONSTRAINT [DF_Tag_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Tag] PRIMARY KEY CLUSTERED ([TagId] ASC)
);






GO


