function checkAnswers() {

    var email = document.getElementById("email").value;
    var username = document.getElementById("username").value;
    var password = document.getElementById("password").value;
    var conf_password = document.getElementById("conf_password").value;

    var errorMessages = [];

    if (!validateEmail(email)) {
        errorMessages.push("Invalid email format");
    }

    if (!validateUsername(username)) {
        errorMessages.push("Invalid username. It has to start with a letter.");
    }

    if (!validatePassword(password, conf_password)) {
        errorMessages.push("You gave two different passwords");
    }

    if (errorMessages.length === 0) {
        document.getElementById("result").innerText = "\n" + "";
        return true; // Allow form submission
    } else {
        var errorMessage = "\n" + errorMessages.join("\n");
        document.getElementById("result").innerText = errorMessage;
        return false; // Prevent form submission
    }
}

function validateEmail(email) {
    // Basic email format validation using regex
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function validateUsername(username) {
    // Username must end with ".wnc"
    var usernameRegex = /^[a-zA-Z]/;
    return usernameRegex.test(username);
}

function validatePassword(password, conf_password) {
    //passwords have to be identical
    if (password === conf_password)
        return true;
}

