# Assignment 05 TCP Client Server KhoiNBD

Assignment 05 gồm hai Console App .NET 8 giao tiếp qua TCP tại
`127.0.0.1:5000`:

- `zSubscription.TCPServer.KhoiNBD`: nhận yêu cầu và trả dữ liệu JSON.
- `zSubscription.TCPClient.KhoiNBD`: nhập lệnh, gửi đến server và trình bày JSON.

## Chạy ứng dụng

Mở hai terminal. Chạy server trước:

```powershell
dotnet run --project "zSubscription.TCPServer.KhoiNBD"
```

Sau đó chạy client:

```powershell
dotnet run --project "zSubscription.TCPClient.KhoiNBD"
```

Client hỗ trợ:

- `ALL`: lấy toàn bộ khoản phí mẫu.
- ID, ví dụ `1`: tìm theo `MembershipFeeKhoiNbdid`.
- Tên hội viên hoặc tên gói, ví dụ `An` hoặc `Premium`.
- `EXIT`: thoát client.

ASM05 sử dụng dữ liệu mẫu trong bộ nhớ theo đúng bài TCP tham chiếu. Model truyền
qua mạng chỉ gồm các trường có thật trong database riêng của KhoiNBD; ứng dụng
không kết nối, không ghi và không thay đổi database `PRN222`.
