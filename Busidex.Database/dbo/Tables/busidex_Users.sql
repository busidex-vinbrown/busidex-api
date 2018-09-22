CREATE TABLE [dbo].[busidex_Users] (
    [ApplicationId]    UNIQUEIDENTIFIER NOT NULL,
    [UserId]           BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserName]         NVARCHAR (256)   NOT NULL,
    [LoweredUserName]  NVARCHAR (256)   NOT NULL,
    [MobileAlias]      NVARCHAR (16)    CONSTRAINT [DF__buisidex___Mobil__1F63A897] DEFAULT (NULL) NULL,
    [IsAnonymous]      BIT              CONSTRAINT [DF__buisidex___IsAno__2057CCD0] DEFAULT ((0)) NOT NULL,
    [LastActivityDate] DATETIME         NOT NULL,
    CONSTRAINT [PK__buisidex__1788CC4D1C873BEC] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK__buisidex___Appli__1E6F845E] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
);




GO
CREATE NONCLUSTERED INDEX [idx_LoweredUserName]
    ON [dbo].[busidex_Users]([LoweredUserName] ASC);

