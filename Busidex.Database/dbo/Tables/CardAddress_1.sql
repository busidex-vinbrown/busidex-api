CREATE TABLE [dbo].[CardAddress] (
    [CardAddressId] BIGINT            IDENTITY (1, 1) NOT NULL,
    [CardId]        BIGINT            NOT NULL,
    [Address1]      VARCHAR (50)      NOT NULL,
    [Address2]      VARCHAR (50)      NULL,
    [City]          VARCHAR (150)     NOT NULL,
    [State]         VARCHAR (2)       NOT NULL,
    [ZipCode]       CHAR (10)         NULL,
    [Region]        VARCHAR (50)      NULL,
    [Country]       VARCHAR (100)     NULL,
    [Deleted]       BIT               CONSTRAINT [DF_CardAddress_Deleted] DEFAULT ((0)) NOT NULL,
    [GeoLocation]   [sys].[geography] NULL,
    CONSTRAINT [PK_CardAddress] PRIMARY KEY CLUSTERED ([CardAddressId] ASC)
);

