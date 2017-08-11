USE [NineTapTour.NineTapDb]
GO

/****** Object: Table [dbo].[Members] Script Date: 8/9/2017 10:25:11 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Members] (
    [FirstName]        NVARCHAR (MAX)  NULL,
    [LastName]         NVARCHAR (MAX)  NULL,
    [MiddleInitial]    NVARCHAR (MAX)  NULL,
    [DateOfBirth]      DATETIME        NOT NULL,
    [Gender]           INT             NOT NULL,
    [IsSenior]         BIT             NOT NULL,
    [IsActive]         BIT             NOT NULL,
    [JoinDate]         DATETIME        NOT NULL,
    [City]             NVARCHAR (MAX)  NULL,
    [State]            NVARCHAR (MAX)  NULL,
    [PostalCode]       NVARCHAR (MAX)  NULL,
    [PrimaryPhone]     NVARCHAR (MAX)  NULL,
    [SecondaryPhone]   NVARCHAR (MAX)  NULL,
    [Notes]            NVARCHAR (MAX)  NULL,
    [Email]            NVARCHAR (MAX)  NULL,
    [Id]               INT             IDENTITY (1, 1) NOT NULL,
    [Number]           INT             NOT NULL,
    [SSN]              CHAR (11)       NULL,
    [Street]           NVARCHAR (MAX)  NULL,
    [Average]          INT             NULL,
    [Handicap]         INT             NULL,
    [Bonus]            INT             NULL,
    [RejoinDate]       DATETIME        NULL,
    [LastBowled]       DATETIME        NULL,
    [Referrals]        INT             NULL,
    [LastPayment]      DATETIME        NULL,
    [IsLifetimeMember] BIT             NOT NULL,
    [StartAvg]         INT             NULL,
    [MoneyEarned]      DECIMAL (18, 2) NOT NULL,
	[AdjAVG] INT NULL
	
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_MemberNumber]
    ON [dbo].[Members]([Number] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MemberSSN]
    ON [dbo].[Members]([SSN] ASC);


GO
ALTER TABLE [dbo].[Members]
    ADD CONSTRAINT [PK_dbo.Members] PRIMARY KEY CLUSTERED ([Id] ASC);


