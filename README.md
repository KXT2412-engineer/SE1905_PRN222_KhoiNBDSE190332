# PRN222 - Assignment 01 → 05 (KhoiNBD - SE190332)

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
| ASM05 | TCP Client/Server | Tìm khoản phí theo ID, tên hội viên, tên gói hoặc lấy tất cả qua TCP |

## Chạy từng assignment

```powershell
dotnet run --project "FA26_PRN222_SE1905_ASM01_KhoiNBD\zSubscription.MVCWebApp.KhoiNBD"
dotnet run --project "FA26_PRN222_SE1905_ASM02_KhoiNBD\zCoach.RazorWebApp.KhoiNBD"
dotnet run --project "FA26_PRN222_SE1905_ASM03_KhoiNBD\zSubscription.BlazorWebApp.KhoiNBD"
dotnet run --project "FA26_PRN222_SE1905_ASM04_KhoiNBD\zSubscription.WorkerService.KhoiNBD"
dotnet run --project "FA26_PRN222_SE1905_ASM05_KhoiNBD\zSubscription.TCPServer.KhoiNBD"
dotnet run --project "FA26_PRN222_SE1905_ASM05_KhoiNBD\zSubscription.TCPClient.KhoiNBD"
```

ASM01–05 đều target .NET 8. ASM02 dùng lại ba project Entities, Repositories và
Services của ASM01; ASM03 và ASM04 có bản copy riêng để có thể mở/chạy độc lập.

## Lưu ý về database riêng

Database của KhoiNBD không có các cột `PaymentMethod`, `TransactionCode` và
`StartDate` như database tham chiếu. Vì vậy UI mới chỉ dùng các trường thực sự được
map: tên hội viên, phí đăng ký, ngày tham gia, ngày hết hạn, gói membership, trạng
thái, ngày publish và ghi chú. Ba tiêu chí tìm kiếm là tên hội viên, trạng thái
Active/Inactive và tên gói.

## ASM04 - chạy dưới dạng Windows Service

ASM04 có thể chạy trực tiếp để debug hoặc cài thành Windows Service. Khi chạy như
service, log hệ thống được gửi vào Windows Event Log; thống kê mỗi chu kỳ cũng được
ghi dạng JSON Lines tại `Logs/membership-worker.jsonl`.

Publish ứng dụng:

```powershell
dotnet publish "FA26_PRN222_SE1905_ASM04_KhoiNBD\zSubscription.WorkerService.KhoiNBD" -c Release -r win-x64 --self-contained false -o "C:\Services\KhoiNBD_PRN222_ASM04"
```

Mở PowerShell bằng quyền Administrator rồi cài và khởi động service:

```powershell
sc.exe create KhoiNBD_PRN222_ASM04 binPath= "C:\Services\KhoiNBD_PRN222_ASM04\zSubscription.WorkerService.KhoiNBD.exe" start= auto
sc.exe start KhoiNBD_PRN222_ASM04
```

Dừng và gỡ service khi không còn sử dụng:

```powershell
sc.exe stop KhoiNBD_PRN222_ASM04
sc.exe delete KhoiNBD_PRN222_ASM04
```

Các thiết lập `IntervalSeconds`, `DeactivateExpiredFees`, `WriteFileLog` và
`LogFilePath` nằm trong `appsettings.json`. Khi chạy bằng profile Development,
`DeactivateExpiredFees` được tắt để việc kiểm thử không sửa dữ liệu.

