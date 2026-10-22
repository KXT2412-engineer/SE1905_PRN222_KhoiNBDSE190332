using Microsoft.AspNetCore.SignalR;
using zSubscription.Entities.KhoiNBD.Models;
using zSubscription.Repositories.KhoiNBD.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace zSubscription.MVCWebApp.KhoiNBD.Hubs
{
    public class zPaymentHub : Hub
    {
        private readonly PRN222Context _context;

        public zPaymentHub(PRN222Context context)
        {
            _context = context;
        }

        public async Task HubDelete_TransactionCashDepositSlip(string id)
        {
            if (int.TryParse(id, out int intId))
            {
                var membershipFee = await _context.MembershipFeeKhoiNbds.FindAsync(intId);
                if (membershipFee != null)
                {
                    _context.MembershipFeeKhoiNbds.Remove(membershipFee);
                    await _context.SaveChangesAsync();
                }

                var feePackage = await _context.FeePackageKhoiNbds.FindAsync(intId);
                if (feePackage != null)
                {
                    _context.FeePackageKhoiNbds.Remove(feePackage);
                    await _context.SaveChangesAsync();
                }
            }

            // Gửi thông báo real-time tới tất cả client đang mở trang Index
            await Clients.All.SendAsync("Receiver_DeleteTransactionCashDepositSlip", id);
        }

        public async Task HubCreate_TransactionCashDepositSlip(string dataObj)
        {
            try
            {
                using var doc = JsonDocument.Parse(dataObj);
                var root = doc.RootElement;

                string code = root.TryGetProperty("Code", out var codeElem) ? codeElem.GetString() ?? "" : "";
                string memberName = root.TryGetProperty("MemberName", out var nameElem) ? nameElem.GetString() ?? "" : "";
                if (string.IsNullOrWhiteSpace(memberName))
                {
                    memberName = !string.IsNullOrWhiteSpace(code) ? "Member " + code : "New Member";
                }

                decimal amount = 0;
                if (root.TryGetProperty("Amount", out var amtElem))
                {
                    if (amtElem.ValueKind == JsonValueKind.Number) amount = amtElem.GetDecimal();
                    else if (amtElem.ValueKind == JsonValueKind.String && decimal.TryParse(amtElem.GetString(), out decimal parsedAmt)) amount = parsedAmt;
                }

                int? packageId = null;
                if (root.TryGetProperty("FeePackageKhoiNbdid", out var pkgElem))
                {
                    if (pkgElem.ValueKind == JsonValueKind.Number) packageId = pkgElem.GetInt32();
                    else if (pkgElem.ValueKind == JsonValueKind.String && int.TryParse(pkgElem.GetString(), out int parsedPkg)) packageId = parsedPkg;
                }

                var newFee = new MembershipFeeKhoiNbd
                {
                    MemberName = memberName,
                    Amount = amount,
                    PaymentDate = DateTime.Now,
                    TransactionCode = code,
                    FeePackageKhoiNbdid = packageId,
                    IsActive = true,
                    Status = 1
                };

                _context.MembershipFeeKhoiNbds.Add(newFee);
                await _context.SaveChangesAsync();

                var item = new
                {
                    code = !string.IsNullOrEmpty(code) ? code : "TXN" + newFee.MembershipFeeKhoiNbdid,
                    cashDepositDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    depositorName = memberName,
                    accountNo = "ACC" + newFee.MembershipFeeKhoiNbdid,
                    withBank = "Vietcombank",
                    amountInFigures = amount > 0 ? amount.ToString("N0") : "100,000",
                    currencyCode = "VND",
                    tellerCode = "TL01",
                    supervisor = "KhoiNBD",
                    id = newFee.MembershipFeeKhoiNbdid
                };

                await Clients.All.SendAsync("Receiver_CreateTransactionCashDepositSlip", item);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in HubCreate_TransactionCashDepositSlip: " + ex.Message);
            }
        }

        public async Task HubUpdate_TransactionCashDepositSlip(string dataObj)
        {
            try
            {
                using var doc = JsonDocument.Parse(dataObj);
                var root = doc.RootElement;

                string idStr = root.TryGetProperty("CashDepositSlipId", out var idElem) ? idElem.GetString() ?? "" : "";
                string code = root.TryGetProperty("Code", out var codeElem) ? codeElem.GetString() ?? "" : "";
                string memberName = root.TryGetProperty("MemberName", out var nameElem) ? nameElem.GetString() ?? "" : "";

                decimal amount = 0;
                if (root.TryGetProperty("Amount", out var amtElem))
                {
                    if (amtElem.ValueKind == JsonValueKind.Number) amount = amtElem.GetDecimal();
                    else if (amtElem.ValueKind == JsonValueKind.String && decimal.TryParse(amtElem.GetString(), out decimal a)) amount = a;
                }

                if (int.TryParse(idStr, out int id))
                {
                    var fee = await _context.MembershipFeeKhoiNbds.FindAsync(id);
                    if (fee != null)
                    {
                        if (!string.IsNullOrEmpty(code))
                            fee.TransactionCode = code;

                        if (!string.IsNullOrEmpty(memberName))
                            fee.MemberName = memberName;

                        if (amount > 0)
                            fee.Amount = amount;

                        if (root.TryGetProperty("FeePackageKhoiNbdid", out var pkgElem))
                        {
                            if (pkgElem.ValueKind == JsonValueKind.Number) fee.FeePackageKhoiNbdid = pkgElem.GetInt32();
                            else if (pkgElem.ValueKind == JsonValueKind.String && int.TryParse(pkgElem.GetString(), out int p)) fee.FeePackageKhoiNbdid = p;
                        }

                        fee.UpdateAt = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }

                    var pkg = await _context.FeePackageKhoiNbds.FindAsync(id);
                    if (pkg != null)
                    {
                        if (!string.IsNullOrEmpty(code))
                            pkg.PackageName = code;

                        await _context.SaveChangesAsync();
                    }
                }

                // Gửi đối tượng depositSlip với thuộc tính code, depositorName...
                var depositSlip = new
                {
                    code = code,
                    depositorName = memberName,
                    memberName = memberName,
                    amount = amount,
                    cashDepositSlipId = idStr
                };

                await Clients.All.SendAsync("Receiver_UpdateTransactionCashDepositSlip", depositSlip);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in HubUpdate_TransactionCashDepositSlip: " + ex.Message);
            }
        }
    }
}
