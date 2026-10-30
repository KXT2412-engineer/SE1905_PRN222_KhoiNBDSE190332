"use strict";

document.addEventListener("DOMContentLoaded", () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/membershipFeeHub")
        .withAutomaticReconnect()
        .build();
    const modalElement = document.getElementById("signalRModal");
    const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
    const status = document.getElementById("hubStatus");
    const form = document.getElementById("signalRFeeForm");
    const errorBox = document.getElementById("signalRFormError");

    const value = id => document.getElementById(id).value;
    const setValue = (id, value) => document.getElementById(id).value = value ?? "";
    const setReady = ready => {
        document.querySelectorAll("#btnSignalRCreate,.btn-sr-edit,.btn-sr-delete")
            .forEach(button => button.disabled = !ready);
    };
    const showStatus = (message, style) => {
        status.textContent = message;
        status.className = `alert alert-${style} py-2 small mb-3`;
    };
    const showError = message => {
        errorBox.textContent = message;
        errorBox.classList.remove("d-none");
    };
    const clearError = () => {
        errorBox.textContent = "";
        errorBox.classList.add("d-none");
    };
    const today = () => new Date().toISOString().slice(0, 10);

    async function loadPackages(selectedId) {
        const packages = await connection.invoke("GetPackages");
        const select = document.getElementById("srPackage");
        select.innerHTML = '<option value="">-- Chọn gói --</option>';
        packages.forEach(item => {
            const option = document.createElement("option");
            option.value = item.feePackageKhoiNbdid;
            option.textContent = item.packageName;
            option.selected = String(item.feePackageKhoiNbdid) === String(selectedId ?? "");
            select.appendChild(option);
        });
    }

    async function openCreate() {
        clearError();
        form.reset();
        setValue("srId", 0);
        setValue("srPaymentDate", today());
        setValue("srStatus", 1);
        document.getElementById("srIsActive").checked = true;
        document.getElementById("signalRModalLabel").textContent = "SignalR Create";
        document.getElementById("btnSignalRSave").textContent = "Create via SignalR";
        await loadPackages(null);
    }

    async function openEdit(id) {
        clearError();
        const item = await connection.invoke("GetById", Number(id));
        if (!item) throw new Error("Không tìm thấy khoản phí.");
        setValue("srId", item.membershipFeeKhoiNbdid);
        setValue("srMemberName", item.memberName);
        setValue("srAmount", item.amount);
        setValue("srPaymentDate", item.paymentDate);
        setValue("srEndDate", item.endDate);
        setValue("srStatus", item.status);
        setValue("srNotes", item.notes);
        document.getElementById("srIsActive").checked = item.isActive;
        await loadPackages(item.feePackageKhoiNbdid);
        document.getElementById("signalRModalLabel").textContent = `SignalR Edit #${id}`;
        document.getElementById("btnSignalRSave").textContent = "Update via SignalR";
        modal.show();
    }

    document.getElementById("btnSignalRCreate").addEventListener("click", () => {
        openCreate().catch(error => showError(error.message));
    });
    document.querySelectorAll(".btn-sr-edit").forEach(button => button.addEventListener("click", () => {
        openEdit(button.dataset.id).catch(error => showStatus(error.message, "danger"));
    }));
    document.querySelectorAll(".btn-sr-delete").forEach(button => button.addEventListener("click", async () => {
        if (!confirm(`Xoá khoản phí #${button.dataset.id} bằng SignalR?`)) return;
        try { await connection.invoke("Delete", Number(button.dataset.id)); }
        catch (error) { showStatus(error.message, "danger"); }
    }));

    form.addEventListener("submit", async event => {
        event.preventDefault();
        clearError();
        const id = Number(value("srId"));
        const dto = {
            membershipFeeKhoiNbdid: id,
            memberName: value("srMemberName").trim(),
            amount: Number(value("srAmount")),
            paymentDate: value("srPaymentDate") || null,
            endDate: value("srEndDate") || null,
            notes: value("srNotes") || null,
            status: value("srStatus") ? Number(value("srStatus")) : null,
            publishDate: today(),
            feePackageKhoiNbdid: value("srPackage") ? Number(value("srPackage")) : null,
            isActive: document.getElementById("srIsActive").checked
        };
        if (!dto.memberName || dto.amount <= 0) {
            showError("Tên hội viên và số tiền lớn hơn 0 là bắt buộc.");
            return;
        }
        try {
            await connection.invoke(id ? "Update" : "Create", dto);
            modal.hide();
        } catch (error) {
            showError(error.message);
        }
    });

    connection.on("MembershipFeeChanged", payload => {
        showStatus(`SignalR: ${payload.action} ${payload.memberName ?? ""}. Đang tải lại...`, "success");
        window.setTimeout(() => window.location.reload(), 700);
    });
    connection.onreconnecting(() => { setReady(false); showStatus("SignalR đang kết nối lại...", "warning"); });
    connection.onreconnected(() => { setReady(true); showStatus("SignalR đã kết nối lại.", "success"); });
    connection.onclose(() => { setReady(false); showStatus("SignalR đã ngắt kết nối.", "danger"); });

    connection.start()
        .then(() => { setReady(true); showStatus("SignalR đã kết nối. CRUD realtime sẵn sàng.", "success"); })
        .catch(error => showStatus(`Kết nối SignalR thất bại: ${error.message}`, "danger"));
});
