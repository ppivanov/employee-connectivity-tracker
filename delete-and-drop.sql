drop table [dbo].[CommunicationPoints];
drop table [dbo].[CalendarEvents];
drop table [dbo].[ReceivedEmails];
drop table  [dbo].[SentEmails];
drop table  [dbo].[Administrators];
alter table [dbo].[Teams] drop constraint FK_Teams_Users_LeaderId
alter table [dbo].[Users] drop constraint FK_Users_Teams_MemberOfId
drop table  [dbo].[Teams];
drop table  [dbo].[Users];
