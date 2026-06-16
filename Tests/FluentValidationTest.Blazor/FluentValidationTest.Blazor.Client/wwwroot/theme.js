window.themeHelper = {
    init: function () {
        var saved = localStorage.getItem('theme');
        if (saved) {
            document.documentElement.dataset.theme = saved;
        }
    },
    set: function (theme) {
        document.documentElement.dataset.theme = theme;
        localStorage.setItem('theme', theme);
    },
    get: function () {
        return localStorage.getItem('theme') || '';
    }
};
