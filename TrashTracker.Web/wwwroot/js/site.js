// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function () {
    var toggler = document.querySelector('#theme-switch'),
        root = document.getElementById('html'),
        currentTheme = document.getElementById('html').getAttribute('data-bs-theme');

    if (currentTheme == 'light')
        toggler.removeAttribute('checked');
    else
        toggler.checked = 'true';

    root.setAttribute('data-bs-theme', currentTheme);

    toggler.addEventListener('change', toggleTheme);

    function toggleTheme() {
        if (this.checked) {
            root.setAttribute('data-bs-theme', 'dark');
            localStorage.setItem('theme', 'dark');
        }

        else {
            root.setAttribute('data-bs-theme', 'light');
            localStorage.setItem('theme', 'light');

        }
    }
})();