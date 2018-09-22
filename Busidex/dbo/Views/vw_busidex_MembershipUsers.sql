
CREATE VIEW [dbo].[vw_busidex_MembershipUsers]
  AS SELECT [dbo].[busidex_Membership].[UserId],
            [dbo].[busidex_Membership].[PasswordFormat],
            [dbo].[busidex_Membership].[MobilePIN],
            [dbo].[busidex_Membership].[Email],
            [dbo].[busidex_Membership].[LoweredEmail],
            [dbo].[busidex_Membership].[PasswordQuestion],
            [dbo].[busidex_Membership].[PasswordAnswer],
            [dbo].[busidex_Membership].[IsApproved],
            [dbo].[busidex_Membership].[IsLockedOut],
            [dbo].[busidex_Membership].[CreateDate],
            [dbo].[busidex_Membership].[LastLoginDate],
            [dbo].[busidex_Membership].[LastPasswordChangedDate],
            [dbo].[busidex_Membership].[LastLockoutDate],
            [dbo].[busidex_Membership].[FailedPasswordAttemptCount],
            [dbo].[busidex_Membership].[FailedPasswordAttemptWindowStart],
            [dbo].[busidex_Membership].[FailedPasswordAnswerAttemptCount],
            [dbo].[busidex_Membership].[FailedPasswordAnswerAttemptWindowStart],
            [dbo].[busidex_Membership].[Comment],
            [dbo].[busidex_Users].[ApplicationId],
            [dbo].[busidex_Users].[UserName],
            [dbo].[busidex_Users].[MobileAlias],
            [dbo].[busidex_Users].[IsAnonymous],
            [dbo].[busidex_Users].[LastActivityDate]
  FROM [dbo].[busidex_Membership] INNER JOIN [dbo].[busidex_Users]
      ON [dbo].[busidex_Membership].[UserId] = [dbo].[busidex_Users].[UserId]