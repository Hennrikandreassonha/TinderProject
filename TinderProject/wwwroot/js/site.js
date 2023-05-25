let scrollPosition = document.getElementById("message-div");
if (scrollPosition != null) {
    scrollPosition.scrollTop = scrollPosition.scrollHeight;
}

let minAgeSlider = document.getElementById("minAge");
let maxAgeSlider = document.getElementById("maxAge");

let maxAgeNumber = document.getElementById("maxAgeInput");
let minAgeNumber = document.getElementById("minAgeInput");


if (minAgeSlider != null) {

    minAgeSlider.addEventListener("change", function () {

        if (minAgeSlider.value > maxAgeSlider.value) {
            maxAgeSlider.value = minAgeSlider.value;
        }
        maxAgeNumber.value = maxAgeSlider.value;
    })
}

if (maxAgeSlider != null) {

    maxAgeSlider.addEventListener("change", function () {

        if (minAgeSlider.value >= maxAgeSlider.value) {
            maxAgeSlider.value = minAgeSlider.value;
        }
        maxAgeNumber.value = maxAgeSlider.value;
    })
}

let ageFormula = document.getElementById("ageFormula");

if (ageFormula != null) {

    ageFormula.addEventListener("click", function () {
        if (ageFormula.checked) {
            minAgeSlider.disabled = true;
            maxAgeSlider.disabled = true;

            maxAgeNumber.disabled = true;
            minAgeNumber.disabled = true;
        } else {
            minAgeSlider.disabled = false;
            maxAgeSlider.disabled = false;

            maxAgeNumber.disabled = false;
            minAgeNumber.disabled = false;
        }
    });

    if (ageFormula.checked) {
        minAgeSlider.disabled = true;
        maxAgeSlider.disabled = true;

        maxAgeNumber.disabled = true;
        minAgeNumber.disabled = true;
    } else {
        minAgeSlider.disabled = false;
        maxAgeSlider.disabled = false;

        maxAgeNumber.disabled = false;
        minAgeNumber.disabled = false;
    }

}
