# ASM04 — Console Worker và Windows Service

Bài đọc danh sách `MembershipFeeKhoiNbd` từ database của KhoiNBD và ghi nối tiếp
JSON có timestamp vào `Logs/KhoiNBD_SE1905_WS.txt` mỗi 5 giây. Bài chỉ đọc database,
không thay đổi dữ liệu.

## Test bằng Visual Studio

1. Mở `FA26_PRN222_SE1905_ASM04_KhoiNBD.slnx`.
2. Chọn `zSubscription.WorkerService.KhoiNBD` làm Startup Project.
3. Ctrl+F5 và chờ 10 giây. Console phải hiện `Appended ... Membership Fees...`
   khoảng hai lần.
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
Manual. Để gỡ đăng ký nhưng giữ file output:

```powershell
.\Uninstall-Service.ps1
```

Connection string và các mapping database dùng nguyên cấu hình hiện có của project.
