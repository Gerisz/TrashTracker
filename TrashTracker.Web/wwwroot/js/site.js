// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));

var toggler = document.getElementById('theme-switch'),
    theme = window.localStorage.getItem('data-bs-theme');

if (theme)
    document.documentElement.setAttribute('data-bs-theme', theme);

toggler.checked = theme == 'dark' ? false : true;

toggler.addEventListener('change', function () {
    if (this.checked) {
        document.documentElement.setAttribute('data-bs-theme', 'light');
        window.localStorage.setItem('data-bs-theme', 'light');
    } else {
        document.documentElement.setAttribute('data-bs-theme', 'dark');
        window.localStorage.setItem('data-bs-theme', 'dark');
    }
});

function getLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(showPosition);
    } else {
        // TODO: navigatior not supported, display a message handling the error
    }
}