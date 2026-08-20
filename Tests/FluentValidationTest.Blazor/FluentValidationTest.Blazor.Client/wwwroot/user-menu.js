// Circuit-free click-outside handling for the <details class="ux-user-menu"> dropdown,
// mirroring nav-drawer.js / theme-storage.js. A native <details> only closes on a click
// on its own <summary> (or when something removes the `open` attribute); a click anywhere
// else on the page does nothing by default, so we close any open instance on outside click.
document.addEventListener('click', function (e) {
    document.querySelectorAll('details.ux-user-menu[open]').forEach(function (menu) {
        if (!menu.contains(e.target)) {
            menu.removeAttribute('open');
        }
    });
});

// Close on Escape, matching common dropdown UX.
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        document.querySelectorAll('details.ux-user-menu[open]').forEach(function (menu) {
            menu.removeAttribute('open');
        });
    }
});
