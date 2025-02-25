function checkAnswers() {
    var phoneNumber = document.getElementById("phoneNumber").value;
    var afm = document.getElementById("afm").value;
    var restaurantName = document.getElementById("restaurantName").value;
    var email = document.getElementById("email").value;
    var priceRange = document.getElementById("priceRangeInput").value;


    var cuisine = document.getElementById("cuisine").value;
    var hours = document.getElementById("hours").value;
    var address = document.getElementById("address").value;
    var location = document.getElementById("location").value;
    var mapLink = document.getElementById("mapLink").value;
    var photosLink = document.getElementById("photosLink").value;
    var menuLink = document.getElementById("menuLink").value;
    var details = document.getElementById("details").value;


    var errorMessages = [];

    if (!validateRestaurantName(restaurantName)) {
        errorMessages.push("Restaurant Name has to start with a letter.");
    }
    if (!validateAfm(afm)) {
        errorMessages.push("Tax Identification Number has to be a number.");
    }

    if (!validatePhoneNumber(phoneNumber)) {
        errorMessages.push("Invalid Phone Number.");
    }

    if (!validateEmail2(email)) {
        errorMessages.push("Invalid Email format.");
    }


    if (!validatePriceRange(priceRange)) {
        errorMessages.push("Put a Price Range");
    }



    if (errorMessages.length === 0 && required(cuisine, hours, address, location, mapLink, photosLink, menuLink, details)) {
        document.getElementById("result").innerText = "" + "\n";
        return true; // Allow form submission
    } else {
        var errorMessage = "" + errorMessages.join("\n");
        document.getElementById("result").innerText = errorMessage;
        return false; // Prevent form submission
    }
}

function validateEmail2(email) {
    // Basic email format validation using regex
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function validateRestaurantName(restaurantName) {
    var restaurantNameRegex = /^[a-zA-Z]/;
    return restaurantNameRegex.test(restaurantName);
}

function validateAfm(afm) {
    var afmRegex = /^\d+$/;
    return afmRegex.test(afm);
}

function validatePhoneNumber(phoneNumber) {
    var phoneNumberRegex = /^\d+$/;
    return phoneNumberRegex.test(phoneNumber);
}


function validatePriceRange(priceRange) {
    if (priceRange == "Cheap Eats-€" || priceRange == "Mid Range-€€" || priceRange == "Fine Dining-€€€") {
        return true;
    } else {
        return false;
    }

}


function required(cuisine, hours, address, location, mapLink, photosLink, menuLink, details) {
    if (
        !cuisine.trim() ||
        !hours.trim() ||
        !address.trim() ||
        !location.trim() ||
        !mapLink.trim() ||
        !photosLink.trim() ||
        !menuLink.trim() ||
        !details.trim()
    ) {
        alert("Fill all the inputs to continue!");
        return false;
    } else {
        return true;
    }
}

