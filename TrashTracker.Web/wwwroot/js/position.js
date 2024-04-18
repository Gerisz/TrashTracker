function getPosition() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(position => success(position),
            error => error(error), { enableHighAccuracy: true });
    } else {
        alert("Geolocation is not supported by this browser.");
    }
}

function success(position) {
    document.getElementById("Lat").value =
        Math.round(position.coords.latitude * 10 ** 6) / 10 ** 6;
    document.getElementById("Long").value =
        Math.round(position.coords.longitude * 10 ** 6) / 10 ** 6;
}

function error(error) {
    alert(`ERROR(${error.code}): ${error.message}`);
}
