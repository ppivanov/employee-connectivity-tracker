using System;
using System.Collections.Generic;
using static EctWebApp.CommonMethods.CommonStaticMethods;

namespace EctWebApp.Models
{
    public class DashboardViewModel
    {
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;
        private readonly List<CalendarEvent> _calendarEvents;
        private readonly List<ReceivedMail> _receivedMail;
        private readonly List<SentMail> _sentMail;

        public List<CalendarEvent> CalendarEvents { get { return _calendarEvents; } }
        public List<ReceivedMail> ReceivedMail { get { return _receivedMail; } }
        public List<SentMail> SentMail { get { return _sentMail; } }

        public DashboardViewModel()
        {
            _startDate = GetDefaultDateTime();
            _endDate = GetEndOfDayForParm(GetDefaultDateTime());
            _calendarEvents = new List<CalendarEvent>();
            _receivedMail = new List<ReceivedMail>();
            _sentMail = new List<SentMail>();
        }

        public DashboardViewModel(DateTime startDate,
                                  DateTime endDate,
                                  List<CalendarEvent> calendarEvents,
                                  List<ReceivedMail> receivedMail,
                                  List<SentMail> sentMail)
        {
            _startDate = startDate;
            _endDate = endDate;
            _calendarEvents = calendarEvents;
            _receivedMail = receivedMail;
            _sentMail = sentMail;
        }

        public string GetDateRange()
        {
            return $"{FormatDateTimeToMinutePrecision(_startDate)} - {FormatDateTimeToMinutePrecision(_endDate)}";
        }
    }
}
