

namespace LIBCORE.DataRepository
{
    public partial class CalendarRepository : ICalendarRepository
    {
        string ICalendarRepository.ExampleRepositoryCalendar()
        {
            return "Implementation for the ExampleRepositoryCalendar() located in the IMemberRepository directly under the DataRepository folder";
        }
    }
}
