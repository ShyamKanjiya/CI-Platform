function validation() {
    var password = document.getElementById("InputPassword").value;
    var confirmPassword = document.getElementById("InputCheckPassword").value;
    if (password != confirmPassword) {
        alert("Passwords do not match.");
        return false;
    }
    return true;
}