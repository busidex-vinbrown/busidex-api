CREATE TABLE [dbo].[EmailAddress] (
    [EmailAddressId] INT            IDENTITY (1, 1) NOT NULL,
    [BusinessId]     INT            NOT NULL,
    [Email]          NVARCHAR (250) NOT NULL,
    [Created]        DATETIME       NOT NULL,
    [Updated]        DATETIME       NOT NULL,
    [Deleted]        BIT            CONSTRAINT [DF_EmailAddress_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_EmailAddress] PRIMARY KEY CLUSTERED ([EmailAddressId] ASC),
    CONSTRAINT [FK_EmailAddress_Business] FOREIGN KEY ([BusinessId]) REFERENCES [dbo].[Business] ([BusinessId])
);

