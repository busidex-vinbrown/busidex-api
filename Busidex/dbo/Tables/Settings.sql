CREATE TABLE [dbo].[Settings] (
    [SettingsId] BIGINT IDENTITY (1, 1) NOT NULL,
    [UserId]     BIGINT NOT NULL,
    [StartPage]  INT    NULL,
    [Updated]    DATE   NOT NULL,
    [Deleted]    BIT    CONSTRAINT [DF_Settings_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED ([SettingsId] ASC),
    CONSTRAINT [FK_Settings_busidex_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[busidex_Users] ([UserId]),
    CONSTRAINT [FK_Settings_Page] FOREIGN KEY ([StartPage]) REFERENCES [dbo].[Page] ([PageId])
);

