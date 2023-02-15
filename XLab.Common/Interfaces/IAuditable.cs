namespace XLab.Common.Interfaces;

public interface IAuditable
{
    bool IsActive { get; set; }
    bool IsDeleted { get; set; }
    string CreatedBy { get; set; }
    DateTime? CreatedAt { get; set; }
    string ModifiedBy { get; set; }
    DateTime? ModifiedAt { get; set; }
}