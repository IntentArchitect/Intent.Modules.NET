// Circuit-free nav-drawer toggle for static-SSR pages (Identity/manage), mirroring
// theme-storage.js. The drawer's open state is a class on <html>; CSS does the slide +
// scrim. No Blazor circuit, no @onclick, no full-page reload.
window.navDrawer = {
    open: function () { document.documentElement.classList.add('ux-drawer-open'); },
    close: function () { document.documentElement.classList.remove('ux-drawer-open'); },
    toggle: function () { document.documentElement.classList.toggle('ux-drawer-open'); }
};

// Close on Escape, matching common drawer UX.
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') { window.navDrawer.close(); }
});
