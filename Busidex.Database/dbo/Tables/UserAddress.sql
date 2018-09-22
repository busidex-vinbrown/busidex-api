CREATE TABLE [dbo].[UserAddress] (
    [UserAddressId] BIGINT            IDENTITY (1, 1) NOT NULL,
    [UserId]        BIGINT            NOT NULL,
    [Address1]      VARCHAR (50)      NOT NULL,
    [Address2]      VARCHAR (50)      NULL,
    [City]          VARCHAR (150)     NOT NULL,
    [State]         VARCHAR (2)       NOT NULL,
    [ZipCode]       CHAR (10)         NULL,
    [Region]        VARCHAR (50)      NULL,
    [Country]       VARCHAR (100)     NULL,
    [GeoLocation]   [sys].[geography] NULL,
    CONSTRAINT [PK_UserAddress] PRIMARY KEY CLUSTERED ([UserAddressId] ASC)
);

