IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'RunningJournal')
DROP DATABASE [RunningJournal];

CREATE DATABASE [RunningJournal]
GO

USE [RunningJournal]
GO

CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	[Username] [nvarchar](50) NOT NULL UNIQUE
)
GO

CREATE TABLE [dbo].[JournalEntry](
	[EntryId] [int] IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	[UserId] [int] NOT NULL REFERENCES [dbo].[User](UserId),
	[Time] [datetimeoffset](7) NOT NULL,
	[Distance] [int] NOT NULL,
	[Duration] [time] NOT NULL
)
GO

INSERT INTO [dbo].[User] VALUES ('Andrew')

INSERT INTO [dbo].JournalEntry (UserId, Duration, Distance, Time)
VALUES (1, '00:24:00.000', 5000, '2019-01-11T07:15:50.0000000-06:00')