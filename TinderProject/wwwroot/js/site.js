let scrollPosition = document.getElementById("message-div");
if (scrollPosition != null) {
    scrollPosition.scrollTop = scrollPosition.scrollHeight;
}

let minAgeInput = document.getElementById("minAge");
let maxAgeInput = document.getElementById("maxAge");

let maxAgeNumber = document.getElementById("maxAgeInput");
let minAgeNumber = document.getElementById("minAgeInput");


if (minAgeInput != null) {

    minAgeInput.addEventListener("change", function () {

        if (minAgeInput.value > maxAgeInput.value) {
            maxAgeInput.value = minAgeInput.value;
        }
        maxAgeNumber.value = maxAgeInput.value;
    })
}

if (maxAgeInput != null) {

    maxAgeInput.addEventListener("change", function () {

        if (maxAgeInput.value < minAgeInput.value) {
            maxAgeInput.value = minAgeInput.value;
        }
        maxAgeNumber.value = maxAgeInput.value;
    })
}

let ageFormula = document.getElementById("ageFormula");



if (ageFormula != null) {

    ageFormula.addEventListener("click", function () {
        if (ageFormula.checked) {
            minAgeInput.disabled = true;
            maxAgeInput.disabled = true;

            maxAgeNumber.disabled = true;
            minAgeNumber.disabled = true;
        } else {
            minAgeInput.disabled = false;
            maxAgeInput.disabled = false;

            maxAgeNumber.disabled = false;
            minAgeNumber.disabled = false;
        }
    });

    if (ageFormula.checked) {
        minAgeInput.disabled = true;
        maxAgeInput.disabled = true;

        maxAgeNumber.disabled = true;
        minAgeNumber.disabled = true;
    } else {
        minAgeInput.disabled = false;
        maxAgeInput.disabled = false;

        maxAgeNumber.disabled = false;
        minAgeNumber.disabled = false;
    }

}
