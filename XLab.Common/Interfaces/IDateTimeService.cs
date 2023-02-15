namespace XLab.Common.Interfaces;

public interface IDateTimeService
{
    public DateTime Now { get; }
    public DateTime UtcNow { get; }
}