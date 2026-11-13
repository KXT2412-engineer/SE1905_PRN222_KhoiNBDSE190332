"use strict";

document.addEventListener("DOMContentLoaded", () => {
    const details = document.getElementById("membershipFeeDetails");
    if (!details || typeof signalR === "undefined") return;

    const id = Number(details.dataset.membershipFeeId);
    const field = name => details.querySelector(`[data-detail-field="${name}"]`);
    const formatDate = value => {
        if (!value) return "-";
        const [year, month, day] = value.slice(0, 10).split("-");
        return `${day}/${month}/${year}`;
    };
    const formatDateTime = value => {
        if (!value) return "-";
        const dateText = formatDate(value);
        const timeText = value.length >= 16 ? value.slice(11, 16) : "";
        return timeText ? `${dateText} ${timeText}` : dateText;
    };

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/membershipFeeHub", {
            transport: signalR.HttpTransportType.WebSockets,
            skipNegotiation: true
        })
        .withAutomaticReconnect()
        .build();

    connection.on("MembershipFeeChanged", change => {
        if (!change || Number(change.membershipFeeKhoiNbdid) !== id) return;

        if (change.action === "deleted") {
            details.querySelector("dl").hidden = true;
            document.getElementById("membershipFeeDeletedNotice").hidden = false;
            document.getElementById("membershipFeeDetailsActions").hidden = true;
            return;
        }

        if (change.action !== "updated" || !change.item) return;

        const item = change.item;
        field("memberName").textContent = item.memberName || "-";
        field("amount").textContent = Number(item.amount ?? 0).toLocaleString("vi-VN");
        field("paymentDate").textContent = formatDate(item.paymentDate);
        field("packageName").textContent = item.packageName || "-";
        field("endDate").textContent = formatDate(item.endDate);
        field("status").textContent = item.status ?? "-";
        const activeBadge = field("activeBadge");
        activeBadge.textContent = item.isActive ? "Active" : "Inactive";
        activeBadge.classList.toggle("bg-success", Boolean(item.isActive));
        activeBadge.classList.toggle("bg-secondary", !item.isActive);
        field("publishDate").textContent = formatDate(item.publishDate);
        field("updateAt").textContent = formatDateTime(item.updateAt);
        field("notes").textContent = item.notes?.trim() || "-";
    });

    connection.start().catch(error => console.error("Details SignalR connection failed:", error));
});
