CREATE TABLE [dbo].[busidex_UsersInRoles] (
    [UserId] BIGINT           NOT NULL,
    [RoleId] UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[busidex_Users] ([UserId])
);

