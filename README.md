# PRN222 - Assignment 01 → 04 (KhoiNBD - SE190332)

Bộ bài tập dùng database riêng của KhoiNBD với hai bảng chính:

- `MemberKhoiNBD` (được map bởi entity `MembershipFeeKhoiNbd`)
- `MembershipPackageKhoiNBD` (được map bởi entity `FeePackageKhoiNbd`)

Connection string và toàn bộ mapping database gốc được giữ nguyên.

| Bài | Công nghệ | Chức năng chính |
| --- | --- | --- |
| ASM01 | ASP.NET Core MVC | CRUD, tìm kiếm 3 tiêu chí, phân trang jQuery |
| ASM02 | Razor Pages + SignalR | Đăng nhập cookie, CRUD Razor Pages, CRUD SignalR |
| ASM03 | Blazor Server | CRUD tương tác, tìm kiếm, phân trang jQuery |
| ASM04 | Worker Service | Tự động vô hiệu hoá membership hết hạn và ghi log thống kê |

## Chạy từng assignment

```powershell
dotnet run --project "FA26_PRN222_SE1905_ASM01_KhoiNBD\zSubscription.MVCWebApp.KhoiNBD"
dotnet run --project "FA26_PRN222_SE1905_ASM02_KhoiNBD\zCoach.RazorWebApp.KhoiNBD"
dotnet run --project "FA26_PRN222_SE1905_ASM03_KhoiNBD\zSubscription.BlazorWebApp.KhoiNBD"
dotnet run --project "FA26_PRN222_SE1905_ASM04_KhoiNBD\zSubscription.WorkerService.KhoiNBD"
```

ASM01–04 đều target .NET 8. ASM02 dùng lại ba project Entities, Repositories và
Services của ASM01; ASM03 và ASM04 có bản copy riêng để có thể mở/chạy độc lập.

## Lưu ý về database riêng

Database của KhoiNBD không có các cột `PaymentMethod`, `TransactionCode` và
`StartDate` như database tham chiếu. Vì vậy UI mới chỉ dùng các trường thực sự được
map: tên hội viên, phí đăng ký, ngày tham gia, ngày hết hạn, gói membership, trạng
thái, ngày publish và ghi chú. Ba tiêu chí tìm kiếm là tên hội viên, trạng thái
Active/Inactive và tên gói.

