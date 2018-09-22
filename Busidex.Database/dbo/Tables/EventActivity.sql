CREATE TABLE [dbo].[EventActivity] (
    [EventActivityId] BIGINT IDENTITY (1, 1) NOT NULL,
    [EventSourceId]   INT    NOT NULL,
    [CardId]          BIGINT NOT NULL,
    [UserId]          BIGINT NOT NULL,
    CONSTRAINT [PK_EventActivity] PRIMARY KEY CLUSTERED ([EventActivityId] ASC),
    CONSTRAINT [FK_EventActivity_busidex_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[busidex_Users] ([UserId]),
    CONSTRAINT [FK_EventActivity_Card] FOREIGN KEY ([CardId]) REFERENCES [dbo].[Card] ([CardId]),
    CONSTRAINT [FK_EventActivity_EventSource] FOREIGN KEY ([EventSourceId]) REFERENCES [dbo].[EventSource] ([EventSourceId])
);


GO
CREATE NONCLUSTERED INDEX [IX_CardId]
    ON [dbo].[EventActivity]([CardId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EventSource]
    ON [dbo].[EventActivity]([EventSourceId] ASC);

