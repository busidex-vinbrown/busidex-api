CREATE TABLE [dbo].[Lizzidex] (
    [LizzidexId]  INT IDENTITY (1, 1) NOT NULL,
    [CoffeeCount] INT CONSTRAINT [DF_Lizzidex_CoffeeCount] DEFAULT ((0)) NOT NULL,
    [ThingCount]  INT CONSTRAINT [DF_Lizzidex_ThingCount] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Lizzidex] PRIMARY KEY CLUSTERED ([LizzidexId] ASC)
);

