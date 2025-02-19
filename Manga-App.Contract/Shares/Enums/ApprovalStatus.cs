using System.ComponentModel;

namespace MangaApp.Contract.Shares.Enums;
public enum ApprovalStatus
{
    [Description("chờ duyệt")]
    Pending,    // Chờ duyệt
    [Description("đã duyệt")]
    Approved,   // Đã duyệt
    [Description("bị từ chối")]
    Rejected    // Bị từ chối
}
