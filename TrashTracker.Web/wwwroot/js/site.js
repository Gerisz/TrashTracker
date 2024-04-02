// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));

(function () {
    var toggler = document.querySelector('#theme-switch'),
        root = document.getElementById('html'),
        currentTheme = document.getElementById('html').getAttribute('data-bs-theme');

    if (currentTheme == 'light')
        toggler.checked = 'true';
    else
        toggler.removeAttribute('checked');

    root.setAttribute('data-bs-theme', currentTheme);

    toggler.addEventListener('change', toggleTheme);

    function toggleTheme() {
        if (this.checked)
            root.setAttribute('data-bs-theme', 'light');
        else
            root.setAttribute('data-bs-theme', 'dark');
    }
})();
