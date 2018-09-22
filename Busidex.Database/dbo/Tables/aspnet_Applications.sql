﻿CREATE TABLE [dbo].[aspnet_Applications] (
    [ApplicationName]        NVARCHAR (256)   NOT NULL,
    [LoweredApplicationName] NVARCHAR (256)   NOT NULL,
    [ApplicationId]          UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Description]            NVARCHAR (256)   NULL,
    PRIMARY KEY CLUSTERED ([ApplicationId] ASC),
    UNIQUE NONCLUSTERED ([ApplicationName] ASC),
    UNIQUE NONCLUSTERED ([LoweredApplicationName] ASC)
);

