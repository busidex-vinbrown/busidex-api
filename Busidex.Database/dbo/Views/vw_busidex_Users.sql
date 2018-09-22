
CREATE VIEW [dbo].[vw_busidex_Users]
  AS SELECT [dbo].[busidex_Users].[ApplicationId], 
  [dbo].[busidex_Users].[UserId], 
  [dbo].[busidex_Users].[UserName], 
  [dbo].[busidex_Users].[LoweredUserName], 
  [dbo].[busidex_Users].[MobileAlias], 
  [dbo].[busidex_Users].[IsAnonymous], 
  [dbo].[busidex_Users].[LastActivityDate]
  FROM [dbo].[busidex_Users]

