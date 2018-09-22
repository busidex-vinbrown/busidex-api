CREATE TABLE [dbo].[busidex_Profile] (
    [UserId]               BIGINT   NOT NULL,
    [PropertyNames]        NTEXT    NOT NULL,
    [PropertyValuesString] NTEXT    NOT NULL,
    [PropertyValuesBinary] IMAGE    NOT NULL,
    [LastUpdatedDate]      DATETIME NOT NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[busidex_Users] ([UserId])
);

