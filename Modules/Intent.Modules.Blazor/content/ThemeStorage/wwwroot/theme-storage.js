window.themeStorage = {
    usesCookieStorage: function () {
        return document.documentElement.dataset.themeStorage === 'cookie';
    },
    readCookie: function () {
        var m = document.cookie.match(/(?:^|;\s*)theme=([^;]+)/);
        return m ? decodeURIComponent(m[1]) : '';
    },
    writeCookie: function (theme) {
        document.cookie = 'theme=' + encodeURIComponent(theme) + '; path=/; max-age=31536000; samesite=lax';
    },
    set: function (theme) {
        document.documentElement.dataset.theme = theme;
        try { localStorage.setItem('theme', theme); } catch (e) { }
        if (this.usesCookieStorage()) { this.writeCookie(theme); }
    },
    get: function () {
        if (this.usesCookieStorage()) {
            var cookie = this.readCookie();
            if (cookie) { return cookie; }
        }
        try { return localStorage.getItem('theme') || ''; } catch (e) { return ''; }
    },
    init: function () {
        var saved = this.get();
        if (saved) { document.documentElement.dataset.theme = saved; }
    },
    toggle: function () {
        var current = document.documentElement.dataset.theme || this.get() || 'dark';
        this.set(current === 'light' ? 'dark' : 'light');
    }
};
window.themeHelper = window.themeStorage;
