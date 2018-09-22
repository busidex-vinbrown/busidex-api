CREATE TABLE [dbo].[Page] (
    [PageId]         INT          IDENTITY (1, 1) NOT NULL,
    [Action]         VARCHAR (50) NOT NULL,
    [ControllerName] VARCHAR (50) NOT NULL,
    [Title]          VARCHAR (50) NULL,
    [Deleted]        BIT          CONSTRAINT [DF_Page_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Page] PRIMARY KEY CLUSTERED ([PageId] ASC)
);

