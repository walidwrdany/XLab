using XLab.Common.Interfaces;

namespace XLab.Web.Data;

public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}