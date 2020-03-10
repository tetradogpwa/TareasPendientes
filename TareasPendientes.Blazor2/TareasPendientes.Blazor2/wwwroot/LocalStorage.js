function SaveLocalStorage(id, data) {
    localStorage.setItem(id, data);
}
function LoadLocalStorage(id) {
    return localStorage.getItem(id);

}