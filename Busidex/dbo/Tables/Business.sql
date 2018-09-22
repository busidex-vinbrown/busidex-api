CREATE TABLE [dbo].[Business] (
    [BusinessId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (250) NOT NULL,
    [Address1]   NVARCHAR (200) NOT NULL,
    [Address2]   NVARCHAR (200) NULL,
    [City]       NVARCHAR (100) NOT NULL,
    [State]      NVARCHAR (10)  NOT NULL,
    [Zip]        NVARCHAR (10)  NOT NULL,
    [Created]    DATETIME       NOT NULL,
    [Updated]    DATETIME       NOT NULL,
    [Deleted]    BIT            CONSTRAINT [DF_Business_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Business] PRIMARY KEY CLUSTERED ([BusinessId] ASC)
);

