set IDENTITY_INSERT [dbo].[Users] ON
set IDENTITY_INSERT [dbo].[Teams] ON
set IDENTITY_INSERT [dbo].[Administrators] ON
set IDENTITY_INSERT [dbo].[CalendarEvents] ON

insert into [dbo].[Teams] (Id, Name) values (1, "Cryptographers")

insert into [dbo].[Users] (Id, Email, FullName, LastSignIn) values (1, 'x00149863@outlook.com', 'Pavel Ivanov', '2021-01-29T14:36:39.3900836', 1)
insert into [dbo].[Users] (Id, Email, FullName, LastSignIn) values (2, 'alice@ect.ie', 'Alice AliceS', '2021-01-29T14:36:39.3900836', 1)
insert into [dbo].[Users] (Id, Email, FullName, LastSignIn) values (3, 'bob@ect.ie', 'Bob BobS', '2021-01-27T12:37:35.9959879', 1)
insert into [dbo].[Users] (Id, Email, FullName, LastSignIn) values (4, 'trudy@ect.ie', 'Trudy TrudyS', '2021-01-27T12:37:35.9959879', 1)
insert into [dbo].[Users] (Id, Email, FullName, LastSignIn) values (5, 'noimagination13@outlook.com', 'Admin AdminS', '2021-01-29T14:36:39.3900836')

update [dbo].[Teams] set LeaderId = 1 where Id = 1

insert int [dbo].[Administrators] (Id, UserId) values (1, 5)

insert into [dbo].[CalendarEvents] (Id, Start, End, Subject, Organizer, AttendeesAsString, EctUserId) values (1, "2020-11-18T13:50:00.0000000", "2020-11-18T14:00:00.0000000", "Late lunch break")

set IDENTITY_INSERT [dbo].[Users] OFF
set IDENTITY_INSERT [dbo].[Teams] OFF
set IDENTITY_INSERT [dbo].[Administrators] OFF
set IDENTITY_INSERT [dbo].[CalendarEvents] OFF