CREATE TABLE [dbo].[EventSource] (
    [EventSourceId] INT          IDENTITY (1, 1) NOT NULL,
    [EventCode]     VARCHAR (10) NOT NULL,
    [Description]   VARCHAR (50) NOT NULL,
    [Active]        BIT          CONSTRAINT [DF_EventSource_Active] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_EventSource] PRIMARY KEY CLUSTERED ([EventSourceId] ASC)
);

