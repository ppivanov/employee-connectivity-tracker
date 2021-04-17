delete from [dbo].[CalendarEvents];
delete from [dbo].[ReceivedEmails];
delete from [dbo].[SentEmails];

insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-05T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-05T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-06T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-06T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-07T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-07T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-08T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-08T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-09T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-09T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)

insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-12T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-12T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-13T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-13T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-14T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-14T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-14T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-14T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-15T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 2)

-- BOB -- PAST (4 emails) -- CURR (4 emails)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-05T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-06T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-07T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-09T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 3)

insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-13T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-14T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-14T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 3)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-15T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 3)

-- TRUDY -- PAST (10 emails) -- CURR (7 emails)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-05T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-05T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-06T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-06T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-07T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-07T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-08T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-08T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-09T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-09T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)

insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-14T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-14T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-15T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-16T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-16T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-16T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-17T11:25:00.0000000', 'Alice AliceS <alice@ect.ie>', 4)

-- PAVEL -- PAST (6 emails) -- CURR (3 emails)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-05T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 5)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-06T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 5)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-06T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 5)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-06T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 5)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-07T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 5)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-09T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 5)

insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-13T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 5)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-14T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 5)
insert into [dbo].[SentEmails] ("Subject", SentAt, RecipientsAsString, EctUserId) values ('Test', '2021-04-14T11:25:00.0000000', 'Trudy TrudyS <trudy@ect.ie>', 5)