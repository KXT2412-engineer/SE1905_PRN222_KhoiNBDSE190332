using zSubscription.TCPServer.KhoiNBD.Models;

namespace zSubscription.TCPServer.KhoiNBD;

public static class DataStore
{
    // Bài TCP dùng dữ liệu mẫu trong bộ nhớ giống project tham chiếu.
    // Tên và trường dữ liệu được đổi theo database riêng của KhoiNBD.
    public static IReadOnlyList<MembershipFeeKhoiNbdRecord> MembershipFees { get; } =
        new List<MembershipFeeKhoiNbdRecord>
        {
            new()
            {
                MembershipFeeKhoiNbdid = 1,
                MemberName = "Nguyen Van An",
                Amount = 500_000m,
                PaymentDate = DateTime.Today.AddDays(-30),
                EndDate = DateTime.Today.AddMonths(1),
                Notes = "Monthly membership",
                Status = 1,
                PublishDate = DateTime.Today.AddDays(-30),
                FeePackageKhoiNbdid = 1,
                PackageName = "Standard",
                IsActive = true
            },
            new()
            {
                MembershipFeeKhoiNbdid = 2,
                MemberName = "Tran Minh Chau",
                Amount = 1_200_000m,
                PaymentDate = DateTime.Today.AddDays(-15),
                EndDate = DateTime.Today.AddMonths(3),
                Notes = "Quarterly membership",
                Status = 1,
                PublishDate = DateTime.Today.AddDays(-15),
                FeePackageKhoiNbdid = 2,
                PackageName = "Premium",
                IsActive = true
            },
            new()
            {
                MembershipFeeKhoiNbdid = 3,
                MemberName = "Le Thanh Binh",
                Amount = 2_400_000m,
                PaymentDate = DateTime.Today.AddMonths(-6),
                EndDate = DateTime.Today.AddDays(-1),
                Notes = "Expired membership",
                Status = 0,
                PublishDate = DateTime.Today.AddMonths(-6),
                FeePackageKhoiNbdid = 3,
                PackageName = "Annual",
                IsActive = false
            }
        };
}
