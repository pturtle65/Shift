CREATE TABLE [Job](
    [JobID] INTEGER PRIMARY KEY AUTOINCREMENT, 
    [AppID] varchar(100), 
    [UserID] varchar(100), 
    [ProcessID] varchar(100), 
    [JobType] varchar(50), 
    [JobName] varchar(100), 
    [InvokeMeta] TEXT, 
    [Parameters] TEXT, 
    [Command] varchar(50), 
    [Status] INTEGER, 
    [Error] TEXT, 
    [Start] datetime, 
    [End] datetime, 
    [Created] datetime);

CREATE INDEX [IX_ProcessID] ON [Job]([ProcessID] ASC);

CREATE TABLE [JobProgress](
	[JobID] INTEGER PRIMARY KEY AUTOINCREMENT REFERENCES Job([JobID]) ON DELETE CASCADE ON UPDATE CASCADE,
	[Percent] INTEGER NULL,
	[Note] TEXT NULL,
	[Data] TEXT NULL);

CREATE VIEW [JobView] AS
SELECT [Job].*, 
       [JobProgress].[Percent], 
       [JobProgress].[Note], 
       [JobProgress].[Data]
FROM   [Job]
       LEFT OUTER JOIN [JobProgress] ON [Job].[JobID] = [JobProgress].[JobID];
