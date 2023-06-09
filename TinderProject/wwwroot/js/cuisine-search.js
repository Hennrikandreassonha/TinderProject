import fakeFetch from './fake-fetch.js';

const useRealAPI = false;

//Anger om vi ska anv�nda riktigt eller fake-api.
async function fetchJSON(url, options) {
    if (useRealAPI) {
        const response = await fetch(url, options);
        const json = await response.json();
        return json;
    }
    else {
        // Sleep for one second to simulate a network delay.
        await new Promise(r => setTimeout(r, 1000));
        const json = fakeFetch(url, options);
        return json;
    }
}

var commonCuisine = document.querySelector('#common-cuisine-data');
var cuisine = document.querySelector('#cuisine-data');

if (commonCuisine != null && commonCuisine.value != "") {
    MakeCall(commonCuisine.value)
    commonCuisine.value;
}

if (cuisine != null && cuisine.value != "") {
    MakeCall(cuisine.value);
    cuisine.value;
}


async function MakeCall(cuisine) {

    const result = await fetchJSON(
        'https://tinderapp.azurewebsites.net/api/?' +
        new URLSearchParams({
            //Här ska vi ha cuisine
            q: cuisine,
        }).toString()
    );

    cuisine = '';

    if (result == null) {
        message.hidden = false;
        resultList.hidden = true;
    }
    else {

        //Skicka tillbaks resultat till backend.
        //Skicka meddelande spara i databasen.

        const jsonDataString = JSON.stringify(result);

        var cuisineForm = document.querySelector('#cuisine-data-form');
        var dataToSend = document.querySelector('#data-to-send');

        dataToSend.value = jsonDataString;
        cuisineForm.submit();
    }
} 
