delete from [dbo].[CommunicationPoints];
delete from [dbo].[CalendarEvents];
delete from [dbo].[ReceivedEmails];
delete from [dbo].[SentEmails];
delete from [dbo].[Administrators];
update [dbo].[Users] set MemberOfId = null;
delete from [dbo].[Teams];
delete from [dbo].[Users];

insert into [dbo].[CommunicationPoints] (Medium, Points) values ('Email (single)', 1)
insert into [dbo].[CommunicationPoints] (Medium, Points) values ('Meetings (10 minutes)', 3)
-- insert into [dbo].[CommunicationPoints] (Medium, Points) values ('Calls (10 minutes)', 0)
-- insert into [dbo].[CommunicationPoints] (Medium, Points) values ('Chat (10 messages)', 0)

set IDENTITY_INSERT [dbo].[Users] ON
insert into [dbo].[Users] (Id, Email, FullName, LastSignIn) values (1, 'test-ect@outlook.com', 'TestUser ECT', '2021-01-29T14:36:39.3900836')
set IDENTITY_INSERT [dbo].[Users] OFF

-- set IDENTITY_INSERT [dbo].[Teams] ON
-- insert into [dbo].[Teams] (Id, Name, LeaderId) values (1, 'Cryptographers',1)
-- set IDENTITY_INSERT [dbo].[Teams] OFF

set IDENTITY_INSERT [dbo].[Users] ON
insert into [dbo].[Users] (Id, Email, FullName, LastSignIn) values (2, 'alice@ect.ie', 'Alice AliceS', '2021-01-29T14:36:39.3900836')
insert into [dbo].[Users] (Id, Email, FullName, LastSignIn) values (3, 'bob@ect.ie', 'Bob BobS', '2021-01-27T12:37:35.9959879')
insert into [dbo].[Users] (Id, Email, FullName, LastSignIn) values (4, 'trudy@ect.ie', 'Trudy TrudyS', '2021-01-27T12:37:35.9959879')
insert into [dbo].[Users] (Id, Email, FullName, LastSignIn) values (5, 'x00149863@outlook.com', 'Pavel Ivanov', '2021-01-29T14:36:39.3900836')
set IDENTITY_INSERT [dbo].[Users] OFF

-- update [dbo].[Users] set MemberOfId = 1;

set IDENTITY_INSERT [dbo].[Administrators] ON
insert into [dbo].[Administrators] (Id, UserId) values (1, 1)
set IDENTITY_INSERT [dbo].[Administrators] OFF

-- After lunch breaks (11 - 15 Jan / one week)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-11T13:50:00.0000000', '2021-01-11T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 1)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-11T13:50:00.0000000', '2021-01-11T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-11T13:50:00.0000000', '2021-01-11T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-11T13:50:00.0000000', '2021-01-11T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)

insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-12T13:50:00.0000000', '2021-01-12T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 1)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-12T13:50:00.0000000', '2021-01-12T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-12T13:50:00.0000000', '2021-01-12T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-12T13:50:00.0000000', '2021-01-12T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)

insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-13T13:50:00.0000000', '2021-01-13T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 1)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-13T13:50:00.0000000', '2021-01-13T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-13T13:50:00.0000000', '2021-01-13T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-13T13:50:00.0000000', '2021-01-13T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)

insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-14T13:50:00.0000000', '2021-01-14T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 1)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-14T13:50:00.0000000', '2021-01-14T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-14T13:50:00.0000000', '2021-01-14T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-14T13:50:00.0000000', '2021-01-14T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)

insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-15T13:50:00.0000000', '2021-01-15T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 1)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-15T13:50:00.0000000', '2021-01-15T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-15T13:50:00.0000000', '2021-01-15T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-15T13:50:00.0000000', '2021-01-15T14:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)


-- Planning (once every 2 weeks / 15 Dec, 29 Dec, 12 Jan, 26 Jan)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2020-12-15T09:00:00.0000000', '2020-12-15T10:00:00.0000000', 'Planning', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 1)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2020-12-15T09:00:00.0000000', '2020-12-15T10:00:00.0000000', 'Planning', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2020-12-15T09:00:00.0000000', '2020-12-15T10:00:00.0000000', 'Planning', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2020-12-15T09:00:00.0000000', '2020-12-15T10:00:00.0000000', 'Planning', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)

insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2020-12-29T09:00:00.0000000', '2020-12-29T10:00:00.0000000', 'Planning', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 1)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2020-12-29T09:00:00.0000000', '2020-12-29T10:00:00.0000000', 'Planning', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2020-12-29T09:00:00.0000000', '2020-12-29T10:00:00.0000000', 'Planning', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2020-12-29T09:00:00.0000000', '2020-12-29T10:00:00.0000000', 'Planning', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)

insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-12T09:00:00.0000000', '2021-01-12T10:00:00.0000000', 'Planning', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 1)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-12T09:00:00.0000000', '2021-01-12T10:00:00.0000000', 'Planning', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-12T09:00:00.0000000', '2021-01-12T10:00:00.0000000', 'Planning', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-12T09:00:00.0000000', '2021-01-12T10:00:00.0000000', 'Planning', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)

insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-26T09:00:00.0000000', '2021-01-26T10:00:00.0000000', 'Planning', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 1)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-26T09:00:00.0000000', '2021-01-26T10:00:00.0000000', 'Planning', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-26T09:00:00.0000000', '2021-01-26T10:00:00.0000000', 'Planning', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-26T09:00:00.0000000', '2021-01-26T10:00:00.0000000', 'Planning', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)


-- New project discussion (once off / 26 Jan)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-26T10:00:00.0000000', '2021-01-26T11:20:00.0000000', 'New project discussion', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie>', 1)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-26T10:00:00.0000000', '2021-01-26T11:20:00.0000000', 'New project discussion', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie>', 2)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-26T10:00:00.0000000', '2021-01-26T11:20:00.0000000', 'New project discussion', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie>', 3)

-- Team breakfast (25 - 29 Jan)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-25T08:30:00.0000000', '2021-01-25T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 1)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-25T08:30:00.0000000', '2021-01-25T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-25T08:30:00.0000000', '2021-01-25T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-25T08:30:00.0000000', '2021-01-25T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)

insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-26T08:30:00.0000000', '2021-01-26T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 1)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-26T08:30:00.0000000', '2021-01-26T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-26T08:30:00.0000000', '2021-01-26T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-26T08:30:00.0000000', '2021-01-26T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)

insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-27T08:30:00.0000000', '2021-01-27T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 1)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-27T08:30:00.0000000', '2021-01-27T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-27T08:30:00.0000000', '2021-01-27T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-27T08:30:00.0000000', '2021-01-27T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)

insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-28T08:30:00.0000000', '2021-01-28T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 1)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-28T08:30:00.0000000', '2021-01-28T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-28T08:30:00.0000000', '2021-01-28T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-28T08:30:00.0000000', '2021-01-28T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)

insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-29T08:30:00.0000000', '2021-01-29T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 1)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-29T08:30:00.0000000', '2021-01-29T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-29T08:30:00.0000000', '2021-01-29T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[CalendarEvents] ("Start", "End", "Subject", Organizer, AttendeesAsString, EctUserId) values ('2021-01-29T08:30:00.0000000', '2021-01-29T09:00:00.0000000', 'After lunch break', 'Pavel Ivanov <ppivanov98@outlook.com>', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Alice AliceS <alice@ect.ie> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)

-- New licence email
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('New licence availabe', 'System <non-existent@ect.ie>', '2021-01-25T12:00:00.0000000', 1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('New licence availabe', 'System <non-existent@ect.ie>', '2021-01-25T12:00:00.0000000', 2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('New licence availabe', 'System <non-existent@ect.ie>', '2021-01-25T12:00:00.0000000', 3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('New licence availabe', 'System <non-existent@ect.ie>', '2021-01-25T12:00:00.0000000', 4)

-- new project - meeting notes
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('New project - meeting notes', 'Alice AliceS <alice@ect.ie>', '2021-01-26T11:25:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('New project - meeting notes', 'Alice AliceS <alice@ect.ie>', '2021-01-26T11:25:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('New project - meeting notes', 'Alice AliceS <alice@ect.ie>', '2021-01-26T11:25:00.0000000',4)

-- Secret birthday party
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Birthday Party for Alice', 'Pavel Ivanov <ppivanov98@outlook.com>', '2021-01-25T09:00:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Birthday Party for Alice', 'Pavel Ivanov <ppivanov98@outlook.com>', '2021-01-25T09:00:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Birthday Party for Alice', 'Pavel Ivanov <ppivanov98@outlook.com>', '2021-01-25T09:00:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Birthday Party for Alice', 'Pavel Ivanov <ppivanov98@outlook.com>', '2021-01-25T09:00:00.0000000',4)

-- phishing
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-02T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-02T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-02T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-02T09:30:00.0000000',4)

-- phishing 2
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 2', 'Attacker <attacker@email.com>', '2021-01-03T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 2', 'Attacker <attacker@email.com>', '2021-01-03T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 2', 'Attacker <attacker@email.com>', '2021-01-03T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 2', 'Attacker <attacker@email.com>', '2021-01-03T09:30:00.0000000',4)
-- phishing 3
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 3', 'Attacker <attacker@email.com>', '2021-01-03T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 3', 'Attacker <attacker@email.com>', '2021-01-03T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 3', 'Attacker <attacker@email.com>', '2021-01-03T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 3', 'Attacker <attacker@email.com>', '2021-01-03T09:30:00.0000000',4)
-- phishing 4
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 4', 'Attacker <attacker@email.com>', '2021-01-03T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 4', 'Attacker <attacker@email.com>', '2021-01-03T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 4', 'Attacker <attacker@email.com>', '2021-01-03T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 4', 'Attacker <attacker@email.com>', '2021-01-03T09:30:00.0000000',4)
-- phishing 5
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 5', 'Attacker <attacker@email.com>', '2021-01-03T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 5', 'Attacker <attacker@email.com>', '2021-01-03T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 5', 'Attacker <attacker@email.com>', '2021-01-03T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 5', 'Attacker <attacker@email.com>', '2021-01-03T09:30:00.0000000',4)

-- phishing 6
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 6', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 6', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 6', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 6', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',4)
-- phishing 7
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 7', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 7', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 7', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 7', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',4)
-- phishing 8
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 8', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 8', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 8', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 8', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',4)
-- phishing 9
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 9', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 9', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 9', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 9', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',4)
-- phishing 10
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 10', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 10', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 10', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 10', 'Attacker <attacker@email.com>', '2021-01-04T09:30:00.0000000',4)

-- phishing 11
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 11', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 11', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 11', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 11', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',4)
-- phishing 12
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 12', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 12', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 12', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 12', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',4)
-- phishing 13
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 13', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 13', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 13', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 13', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',4)
-- phishing 14
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 14', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 14', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 14', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 14', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',4)
-- phishing 15
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 15', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 15', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 15', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 15', 'Attacker <attacker@email.com>', '2021-01-05T09:30:00.0000000',4)

-- phishing 16
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 16', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 16', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 16', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 16', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',4)
-- phishing 17
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 17', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 17', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 17', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 17', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',4)
-- phishing 18
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 18', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 18', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 18', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 18', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',4)
-- phishing 19
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 19', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 19', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 19', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 19', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',4)
-- phishing 20
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 20', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 20', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 20', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 20', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',4)
-- phishing 21
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 21', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 21', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 21', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 21', 'Attacker <attacker@email.com>', '2021-01-06T09:30:00.0000000',4)

-- phishing 22
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 22', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 22', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 22', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 22', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',4)
-- phishing 23
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 23', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 23', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 23', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 23', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',4)
-- phishing 24
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 24', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 24', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 24', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 24', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',4)
-- phishing 25
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 25', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 25', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 25', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 25', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',4)
-- phishing 26
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 26', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 26', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 26', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 26', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',4)
-- phishing 27
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 27', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 27', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 27', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt 27', 'Attacker <attacker@email.com>', '2021-01-09T09:30:00.0000000',4)


-- more phishing
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',4)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',4)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',4)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',4)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',4)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-13T09:30:00.0000000',4)

-- more phishing
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',4)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',4)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',4)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',4)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',4)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-16T09:30:00.0000000',4)


-- more phishing
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',4)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',4)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',4)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',4)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',4)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Phishing attempt', 'Attacker <attacker@email.com>', '2021-01-23T09:30:00.0000000',4)

-- spam
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail', 'Spammer <spammer@email.com>', '2021-01-25T09:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail', 'Spammer <spammer@email.com>', '2021-01-25T09:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail', 'Spammer <spammer@email.com>', '2021-01-25T09:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail', 'Spammer <spammer@email.com>', '2021-01-25T09:30:00.0000000',4)

-- spam 2
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 2', 'Spammer <spammer@email.com>', '2021-01-25T10:00:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 2', 'Spammer <spammer@email.com>', '2021-01-25T10:00:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 2', 'Spammer <spammer@email.com>', '2021-01-25T10:00:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 2', 'Spammer <spammer@email.com>', '2021-01-25T10:00:00.0000000',4)

-- spam 3
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 3', 'Spammer <spammer@email.com>', '2021-01-26T10:00:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 3', 'Spammer <spammer@email.com>', '2021-01-26T10:00:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 3', 'Spammer <spammer@email.com>', '2021-01-26T10:00:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 3', 'Spammer <spammer@email.com>', '2021-01-26T10:00:00.0000000',4)

-- spam 4
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 4', 'Spammer <spammer@email.com>', '2021-01-26T11:00:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 4', 'Spammer <spammer@email.com>', '2021-01-26T11:00:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 4', 'Spammer <spammer@email.com>', '2021-01-26T11:00:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 4', 'Spammer <spammer@email.com>', '2021-01-26T11:00:00.0000000',4)

-- spam 5
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 5', 'Spammer <spammer@email.com>', '2021-01-26T12:00:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 5', 'Spammer <spammer@email.com>', '2021-01-26T12:00:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 5', 'Spammer <spammer@email.com>', '2021-01-26T12:00:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 5', 'Spammer <spammer@email.com>', '2021-01-26T12:00:00.0000000',4)

-- spam 6
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 6', 'Spammer <spammer@email.com>', '2021-01-27T08:00:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 6', 'Spammer <spammer@email.com>', '2021-01-27T08:00:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 6', 'Spammer <spammer@email.com>', '2021-01-27T08:00:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 6', 'Spammer <spammer@email.com>', '2021-01-27T08:00:00.0000000',4)

-- spam 7
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 7', 'Spammer <spammer@email.com>', '2021-01-27T08:20:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 7', 'Spammer <spammer@email.com>', '2021-01-27T08:20:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 7', 'Spammer <spammer@email.com>', '2021-01-27T08:20:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 7', 'Spammer <spammer@email.com>', '2021-01-27T08:20:00.0000000',4)

-- spam 8
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 8', 'Spammer <spammer@email.com>', '2021-01-27T08:22:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 8', 'Spammer <spammer@email.com>', '2021-01-27T08:22:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 8', 'Spammer <spammer@email.com>', '2021-01-27T08:22:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 8', 'Spammer <spammer@email.com>', '2021-01-27T08:22:00.0000000',4)

-- spam 9
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 9', 'Spammer <spammer@email.com>', '2021-01-27T08:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 9', 'Spammer <spammer@email.com>', '2021-01-27T08:30:00.0000000',2)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 9', 'Spammer <spammer@email.com>', '2021-01-27T08:30:00.0000000',3)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Spam mail 9', 'Spammer <spammer@email.com>', '2021-01-27T08:30:00.0000000',4)

-- Weight Test
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Test', 'Alice AliceS <alice@ect.ie>', '2021-02-12T11:25:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Test', 'Alice AliceS <alice@ect.ie>', '2021-02-12T11:30:00.0000000',1)
insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Test', 'Trudy TrudyS <trudy@ect.ie>', '2021-02-12T11:25:00.0000000',1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-02-12T11:25:00.0000000', 'Bob BobS <bob@ect.ie> | Alice AliceS <alice@ect.ie>', 1)

insert into [dbo].[ReceivedEmails] ("Subject", "From", ReceivedAt, EctUserId) values ('Test', 'Trudy TrudyS <trudy@ect.ie>', '2021-02-11T11:25:00.0000000',1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-02-11T11:25:00.0000000', 'Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 1)

-- Sent mail
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('New project - meeting notes', '2021-01-26T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)

-- this takes too long (Alice)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Afternoon break invitation', '2021-01-12T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Afternoon break invitation', '2021-01-13T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Afternoon break invitation', '2021-01-15T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Afternoon break invitation', '2021-01-26T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Afternoon break invitation', '2021-01-27T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Afternoon break invitation', '2021-01-28T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com> | X00149863@outlook.com <X00149863@outlook.com> | Bob BobS <bob@ect.ie> | Trudy TrudyS <trudy@ect.ie>', 4)

-- x00149863
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Project progress', '2021-01-01T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Project progress', '2021-01-04T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Project progress', '2021-01-05T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Project progress', '2021-01-06T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Project progress', '2021-01-07T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Project progress', '2021-01-08T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Project progress', '2021-01-11T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Project progress', '2021-01-12T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Project progress', '2021-01-13T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Project progress', '2021-01-14T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Project progress', '2021-01-15T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Project progress', '2021-01-18T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Project progress', '2021-01-19T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Project progress', '2021-01-20T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)

insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Customer stuff', '2021-01-04T11:25:00.0000000', 'customer <customer@email.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Scrum master updates', '2021-01-04T11:25:00.0000000', 'Scrum Master <s-master@email.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Investigation', '2021-01-04T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing', '2021-01-04T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing 2', '2021-01-04T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 1)

insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Customer stuff', '2021-01-06T11:25:00.0000000', 'customer <customer@email.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Scrum master updates', '2021-01-06T11:25:00.0000000', 'Scrum Master <s-master@email.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Investigation', '2021-01-06T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing', '2021-01-06T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing 2', '2021-01-06T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 1)

insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Customer stuff', '2021-01-08T11:25:00.0000000', 'customer <customer@email.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Scrum master updates', '2021-01-08T11:25:00.0000000', 'Scrum Master <s-master@email.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Investigation', '2021-01-08T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)

insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Customer stuff', '2021-01-11T11:25:00.0000000', 'customer <customer@email.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Scrum master updates', '2021-01-11T11:25:00.0000000', 'Scrum Master <s-master@email.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Investigation', '2021-01-11T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing', '2021-01-11T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing 2', '2021-01-11T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 1)


insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Customer stuff', '2021-01-18T11:25:00.0000000', 'customer <customer@email.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Scrum master updates', '2021-01-18T11:25:00.0000000', 'Scrum Master <s-master@email.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Investigation', '2021-01-18T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing', '2021-01-18T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing 2', '2021-01-18T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Customer stuff', '2021-01-18T11:25:00.0000000', 'customer <customer@email.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Scrum master updates', '2021-01-18T11:25:00.0000000', 'Scrum Master <s-master@email.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Investigation', '2021-01-18T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing', '2021-01-18T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing 2', '2021-01-18T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 1)

insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Customer stuff', '2021-01-25T11:25:00.0000000', 'customer <customer@email.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Scrum master updates', '2021-01-18T11:25:00.0000000', 'Scrum Master <s-master@email.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Investigation', '2021-01-25T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing', '2021-01-25T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing 2', '2021-01-25T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Customer stuff', '2021-01-25T11:25:00.0000000', 'customer <customer@email.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Scrum master updates', '2021-01-25T11:25:00.0000000', 'Scrum Master <s-master@email.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Investigation', '2021-01-25T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing', '2021-01-25T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 1)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing 2', '2021-01-25T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 1)

-- Trudy
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Customer stuff', '2021-01-05T11:25:00.0000000', 'customer <customer@email.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Scrum master updates', '2021-01-05T11:25:00.0000000', 'Scrum Master <s-master@email.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Investigation', '2021-01-05T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing', '2021-01-05T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing 2', '2021-01-05T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)

insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Customer stuff', '2021-01-07T11:25:00.0000000', 'customer <customer@email.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Scrum master updates', '2021-01-07T11:25:00.0000000', 'Scrum Master <s-master@email.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Investigation', '2021-01-07T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing', '2021-01-07T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing 2', '2021-01-07T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)

insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Customer stuff', '2021-01-08T11:25:00.0000000', 'customer <customer@email.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Scrum master updates', '2021-01-08T11:25:00.0000000', 'Scrum Master <s-master@email.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Investigation', '2021-01-08T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 4)

insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Customer stuff', '2021-01-12T11:25:00.0000000', 'customer <customer@email.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Scrum master updates', '2021-01-12T11:25:00.0000000', 'Scrum Master <s-master@email.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Investigation', '2021-01-12T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing', '2021-01-12T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing 2', '2021-01-12T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)


insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Customer stuff', '2021-01-19T11:25:00.0000000', 'customer <customer@email.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Scrum master updates', '2021-01-18T11:25:00.0000000', 'Scrum Master <s-master@email.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Investigation', '2021-01-19T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing', '2021-01-19T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing 2', '2021-01-19T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Customer stuff', '2021-01-19T11:25:00.0000000', 'customer <customer@email.com>', 4)

insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Customer stuff', '2021-01-27T11:25:00.0000000', 'customer <customer@email.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Scrum master updates', '2021-01-18T11:25:00.0000000', 'Scrum Master <s-master@email.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Investigation', '2021-01-27T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing', '2021-01-27T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing 2', '2021-01-27T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Customer stuff', '2021-01-27T11:25:00.0000000', 'customer <customer@email.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Scrum master updates', '2021-01-27T11:25:00.0000000', 'Scrum Master <s-master@email.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Investigation', '2021-01-27T11:25:00.0000000', 'ppivanov98@outlook.com <ppivanov98@outlook.com>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing', '2021-01-27T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Code reviewing 2', '2021-01-27T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)