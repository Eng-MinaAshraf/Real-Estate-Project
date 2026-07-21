// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

(function () {
    "use strict";

    // ============================================================
    // Theme toggle (light / dark)
    // ============================================================
    function applyTheme(theme) {
        var root = document.documentElement;
        root.classList.remove("light", "dark");
        root.classList.add(theme);
        root.setAttribute("data-theme", theme);
        root.style.colorScheme = theme;

        document.querySelectorAll("[data-theme-toggle]").forEach(function (btn) {
            btn.setAttribute("aria-pressed", theme === "light" ? "true" : "false");
        });
    }

    function currentTheme() {
        return document.documentElement.getAttribute("data-theme") === "light" ? "light" : "dark";
    }

    document.addEventListener("click", function (e) {
        var toggle = e.target.closest("[data-theme-toggle]");
        if (!toggle) return;

        var next = currentTheme() === "light" ? "dark" : "light";
        applyTheme(next);
        try {
            localStorage.setItem("aqx-theme", next);
        } catch (err) { /* تجاهل لو localStorage مش متاح */ }
    });

    // نظبط aria-pressed مبدئياً حسب الثيم المطبّق من سكربت الـ head
    applyTheme(currentTheme());

    // ============================================================
    // Notifications dropdown
    // ============================================================
    function closeAllPanels(except) {
        document.querySelectorAll("[data-notif-root]").forEach(function (root) {
            if (root === except) return;
            var panel = root.querySelector("[data-notif-panel]");
            var trigger = root.querySelector("[data-notif-toggle]");
            if (panel) panel.classList.add("aqx-hidden");
            if (trigger) trigger.setAttribute("aria-expanded", "false");
        });
    }

    document.addEventListener("click", function (e) {
        var trigger = e.target.closest("[data-notif-toggle]");
        if (trigger) {
            var root = trigger.closest("[data-notif-root]");
            var panel = root.querySelector("[data-notif-panel]");
            var isOpen = !panel.classList.contains("aqx-hidden");
            closeAllPanels(root);
            if (isOpen) {
                panel.classList.add("aqx-hidden");
                trigger.setAttribute("aria-expanded", "false");
            } else {
                panel.classList.remove("aqx-hidden");
                trigger.setAttribute("aria-expanded", "true");
            }
            return;
        }

        // كليك جوّا الدروب داون مايقفلوش (إلا الروابط)، وبرّه يقفل الكل
        if (!e.target.closest("[data-notif-panel]")) {
            closeAllPanels(null);
        }
    });

    document.addEventListener("keydown", function (e) {
        if (e.key === "Escape") closeAllPanels(null);
    });

    // ============================================================
    // Mark all as read (AJAX) — يحدّث الـ badge لحظياً
    // ============================================================
    document.addEventListener("submit", function (e) {
        var form = e.target.closest("[data-notif-markall-form]");
        if (!form) return;

        e.preventDefault();
        var tokenInput = form.querySelector('input[name="__RequestVerificationToken"]');
        var body = new FormData();
        if (tokenInput) body.append("__RequestVerificationToken", tokenInput.value);

        fetch(form.getAttribute("action"), {
            method: "POST",
            headers: { "X-Requested-With": "XMLHttpRequest" },
            body: body
        }).then(function (res) {
            if (!res.ok) return;
            // نخفي كل الـ badges ونشيل حالة unread من العناصر المعروضة
            document.querySelectorAll("[data-notif-badge]").forEach(function (b) {
                b.classList.add("aqx-hidden");
            });
            document.querySelectorAll(".aqx-notif-unread").forEach(function (item) {
                item.classList.remove("aqx-notif-unread");
            });
            document.querySelectorAll(".aqx-notif-dot").forEach(function (dot) {
                dot.remove();
            });
        }).catch(function () { /* تجاهل */ });
    });
})();
