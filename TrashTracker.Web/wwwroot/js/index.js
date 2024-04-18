const currentPageSize = new URLSearchParams(window.location.search).get('pageSize');
document.getElementById('pageSize').value = currentPageSize;


function filter() {
    const showCleaned = document.getElementById('showCleaned').checked;
    const searchString = document.getElementById('searchString').value;
    const pageSize = document.getElementById('pageSize').value;
    window.location.replace(`
        ${window.location.origin}${window.location.pathname}?${new URLSearchParams({
        showCleaned: showCleaned,
        searchString: searchString,
        pageSize: pageSize
    })}`);
    return false;
}