"use strict";

document.addEventListener("DOMContentLoaded", () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/membershipFeeHub", { transport: signalR.HttpTransportType.WebSockets, skipNegotiation: true })
        .withAutomaticReconnect()
        .build();
    const table = document.getElementById("membershipFeeTable");
    const body = table.tBodies[0];
    const status = document.getElementById("hubStatus");
    let busy = false;
    let pending = false;

    const esc = value => {
        const node = document.createElement("span");
        node.textContent = value ?? "";
        return node.innerHTML;
    };
    const date = value => value ? new Date(`${value}T00:00:00`).toLocaleDateString("vi-VN") : "-";

    function row(item) {
        const id = item.membershipFeeKhoiNbdid;
        const tr = document.createElement("tr");
        const columns = [
            `<div class="fw-semibold">${esc(item.memberName)}</div><div class="text-muted small">ID: ${id}</div>`,
            `<div class="fw-semibold">${Number(item.amount).toLocaleString("vi-VN")} đ</div><div class="text-muted small">Ngày tham gia: ${date(item.paymentDate)}</div>`,
            `<div>Hết hạn: ${date(item.endDate)}</div><div class="text-muted small">Publish: ${date(item.publishDate)}</div>`,
            esc(item.packageName || "-"),
            `<div class="small">Status: ${item.status ?? "-"}</div><span class="badge ${item.isActive ? "bg-success" : "bg-secondary"}">${item.isActive ? "Active" : "Inactive"}</span>`,
            `<div class="d-flex flex-wrap gap-1 justify-content-end"><a class="btn btn-sm btn-outline-secondary" href="/MembershipFees/Details/${id}">Details</a><a class="btn btn-sm btn-outline-primary" href="/MembershipFees/Edit/${id}">Edit</a><a class="btn btn-sm btn-outline-danger" href="/MembershipFees/Delete/${id}">Delete</a></div>`
        ];
        columns.forEach((content, index) => {
            const cell = document.createElement("td");
            if (index === 5) cell.className = "text-end";
            cell.innerHTML = content;
            tr.appendChild(cell);
        });
        return tr;
    }

    async function refresh() {
        pending = true;
        if (busy) return;
        busy = true;
        try {
            while (pending) {
                pending = false;
                const items = await connection.invoke(
                    "GetAll",
                    document.getElementById("memberName").value || null,
                    document.getElementById("activeStatus").value || null,
                    document.getElementById("packageName").value || null);
                const rows = items.map(row);
                if (!rows.length) {
                    const empty = document.createElement("tr");
                    empty.className = "js-empty-row";
                    empty.innerHTML = '<td colspan="6" class="text-center text-muted py-4">Không có dữ liệu phù hợp.</td>';
                    rows.push(empty);
                }
                body.replaceChildren(...rows);
                tablePagination.init({
                    table: "#membershipFeeTable",
                    pager: "#feePager",
                    info: "#feePageInfo",
                    pageSizeSelector: "#feePageSize",
                    pageSize: Number(document.getElementById("feePageSize").value) || 10
                });
            }
        } catch (error) {
            status.textContent = `Không thể cập nhật danh sách qua SignalR: ${error.message}`;
            status.className = "alert alert-danger py-2 small mb-3";
        } finally {
            busy = false;
        }
    }

    connection.on("MembershipFeeChanged", refresh);
    connection.onreconnecting(() => {
        status.textContent = "SignalR đang kết nối lại...";
        status.className = "alert alert-warning py-2 small mb-3";
    });
    connection.onreconnected(() => {
        status.textContent = "SignalR đã kết nối. Đang đồng bộ danh sách...";
        status.className = "alert alert-success py-2 small mb-3";
        refresh();
    });
    connection.onclose(() => {
        status.textContent = "SignalR đã ngắt kết nối.";
        status.className = "alert alert-danger py-2 small mb-3";
    });
    connection.start()
        .then(() => {
            status.textContent = "SignalR đã kết nối. Danh sách tự cập nhật khi có thay đổi.";
            status.className = "alert alert-success py-2 small mb-3";
        })
        .catch(error => {
            status.textContent = `Kết nối SignalR thất bại: ${error.message}`;
            status.className = "alert alert-danger py-2 small mb-3";
        });
});
