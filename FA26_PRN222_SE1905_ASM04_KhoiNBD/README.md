# ASM04 — Console Worker và Windows Service

Bài đọc danh sách `MembershipFeeKhoiNbd` từ database của KhoiNBD và ghi nối tiếp
JSON có timestamp vào `Logs/KhoiNBD_SE1905_WS.txt` theo chu kỳ cấu hình (mặc định 5
giây). `DeactivateExpiredFees` mặc định tắt để worker không sửa dữ liệu khi demo.

## Test bằng Visual Studio

1. Mở `FA26_PRN222_SE1905_ASM04_KhoiNBD.slnx`.
2. Chọn `zSubscription.WorkerService.KhoiNBD` làm Startup Project.
3. Ctrl+F5 và chờ ít nhất 10 giây. Console phải hiện log mỗi chu kỳ.
4. Kiểm tra file `bin/Debug/net8.0/Logs/KhoiNBD_SE1905_WS.txt`.

## Publish và cài Windows Service

Chạy trong thư mục ASM04:

```powershell
dotnet publish .\zSubscription.WorkerService.KhoiNBD\zSubscription.WorkerService.KhoiNBD.csproj -c Release -o .\publish
```

Mở PowerShell bằng **Run as administrator**, sau đó chạy:

```powershell
.\Install-Service.ps1
```

Mở `services.msc` và tìm `KhoiNBD_PRN222_ASM04`. Script đặt Startup Type là
Manual rồi khởi động service. Làm mới danh sách trong Services nếu chưa thấy tên.
File snapshot nằm ở `publish\Logs\KhoiNBD_SE1905_WS.txt`. Để gỡ đăng ký nhưng giữ
file output:

```powershell
.\Uninstall-Service.ps1
```

Nếu worker đã chạy bằng Ctrl+F5 thì dừng console trước khi cài service để tránh hai
tiến trình cùng ghi file. Service đọc `appsettings.json` nằm cạnh executable trong
thư mục `publish`; sau khi đổi cấu hình, publish lại trước khi cài/chạy service.
