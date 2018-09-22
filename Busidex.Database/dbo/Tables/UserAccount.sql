CREATE TABLE [dbo].[UserAccount] (
    [UserAccountId]   BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserId]          BIGINT           NOT NULL,
    [AccountTypeId]   INT              NOT NULL,
    [Created]         DATETIME         CONSTRAINT [DF_UserAccount_Created] DEFAULT (getdate()) NOT NULL,
    [Active]          BIT              CONSTRAINT [DF_UserAccount_Active] DEFAULT ((1)) NOT NULL,
    [Notes]           VARCHAR (1000)   NULL,
    [ActivationToken] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_UserAccount] PRIMARY KEY CLUSTERED ([UserAccountId] ASC),
    CONSTRAINT [FK_UserAccount_AccountType] FOREIGN KEY ([AccountTypeId]) REFERENCES [dbo].[AccountType] ([AccountTypeId]),
    CONSTRAINT [FK_UserAccount_busidex_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[busidex_Users] ([UserId])
);




GO
CREATE TRIGGER trg_UserAccountChanged
   ON  dbo.UserAccount
   AFTER UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @userId bigint,
			@active bit,
			@accountTypeId int

    -- Insert statements for trigger here
	select @userId = INSERTED.userId, @active = INSERTED.active, @accountTypeId = INSERTED.accountTypeId from INSERTED
	
	Insert into AccountHistory(UserId, AccountTypeId, active, Updated)
		values(@userId, @accountTypeId, @active,  GetDate()) 

END