function checkAnswers() {
    var username = document.getElementById("username").value;
    var password = document.getElementById("newPassword").value;
    var conf_password = document.getElementById("conf_password").value;
    var email = document.getElementById("email").value;

    var errorMessages = [];

    if (!validateUsername(username)) {
        errorMessages.push("Invalid username. It has to start with a letter.");
    }

    if (!validatePassword(password, conf_password)) {
        errorMessages.push("You gave two different passwords");
    }

    if (!validateEmail(email)) {
        errorMessages.push("Invalid email format");
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
    var usernameRegex = /^[a-zA-Z]/;
    return usernameRegex.test(username);
}

function validatePassword(password, conf_password) {
    if (password === conf_password) {
        return true;
    } else {
        return false;
    }
}