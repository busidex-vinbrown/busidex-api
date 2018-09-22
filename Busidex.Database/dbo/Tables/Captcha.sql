CREATE TABLE [dbo].[Captcha] (
    [CaptchaId]   INT       IDENTITY (1, 1) NOT NULL,
    [CaptchaText] CHAR (17) NOT NULL,
    [Deleted]     BIT       CONSTRAINT [DF_Captcha_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Captcha] PRIMARY KEY CLUSTERED ([CaptchaId] ASC)
);

