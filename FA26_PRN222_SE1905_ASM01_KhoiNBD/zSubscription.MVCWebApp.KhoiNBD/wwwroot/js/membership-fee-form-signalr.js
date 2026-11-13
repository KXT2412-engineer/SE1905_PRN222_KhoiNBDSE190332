"use strict";

document.addEventListener("DOMContentLoaded", () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/membershipFeeHub", { transport: signalR.HttpTransportType.WebSockets, skipNegotiation: true })
        .withAutomaticReconnect()
        .build();

    // The shared layout has a logout form before the page form. Select the
    // membership fee form explicitly so the SignalR payload reads its fields.
    const form = document.querySelector("form[data-membership-fee-form]");
    const indexUrl = "/MembershipFees";
    const field = name => form?.elements.namedItem(name)?.value ?? "";

    document.querySelectorAll("[data-sr-form-action]").forEach(button => {
        button.addEventListener("click", async () => {
            if (!form || !form.reportValidity()) return;
            button.disabled = true;
            const action = button.dataset.srFormAction;
            const dto = {
                membershipFeeKhoiNbdid: Number(field("MembershipFeeKhoiNbdid")) || 0,
                memberName: field("MemberName").trim(),
                amount: Number(field("Amount")),
                paymentDate: field("PaymentDate") || null,
                endDate: field("EndDate") || null,
                notes: field("Notes") || null,
                status: field("Status") === "" ? null : Number(field("Status")),
                publishDate: field("PublishDate") || null,
                feePackageKhoiNbdid: field("FeePackageKhoiNbdid") === "" ? null : Number(field("FeePackageKhoiNbdid")),
                isActive: Boolean(form.querySelector('[name="IsActive"][type="checkbox"]')?.checked)
            };

            try {
                if (connection.state !== signalR.HubConnectionState.Connected)
                    throw new Error("SignalR chưa kết nối. Tải lại trang rồi thử lại.");
                await connection.invoke(action === "create" ? "Create" : "Update", dto);
                window.location.assign(indexUrl);
            } catch (error) {
                alert(`Không thể lưu bằng SignalR: ${error.message}`);
                button.disabled = false;
            }
        });
    });

    document.querySelectorAll("[data-sr-delete-id]").forEach(button => {
        button.addEventListener("click", async () => {
            const id = Number(button.dataset.srDeleteId);
            if (!confirm(`Xoá khoản phí #${id} bằng SignalR?`)) return;
            button.disabled = true;
            try {
                if (connection.state !== signalR.HubConnectionState.Connected)
                    throw new Error("SignalR chưa kết nối. Tải lại trang rồi thử lại.");
                await connection.invoke("Delete", id);
                window.location.assign(indexUrl);
            } catch (error) {
                alert(`Không thể xoá bằng SignalR: ${error.message}`);
                button.disabled = false;
            }
        });
    });

    connection.start().catch(error => console.error("Membership fee SignalR connection failed:", error));
});
