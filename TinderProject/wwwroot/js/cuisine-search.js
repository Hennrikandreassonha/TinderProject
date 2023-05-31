import fakeFetch from './fake-fetch.js';

const apiKey = '9143904-58a05ad2013e7353b89d19cb0';
const useRealAPI = false;


//Anger om vi ska använda riktigt eller fake-api.
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

// const resultList = document.querySelector('#results');
// const message = document.querySelector('#message');

commonCuisine.addEventListener("change", function () {
    MakeCall(commonCuisine.value);
})

cuisine.addEventListener("change", function () {
    MakeCall(cuisine.value);
})


function MakeCall(cuisine){

    form.onsubmit = async event => {
        event.preventDefault();
    
        const result = await fetchJSON(
            'https://tinderapp.azurewebsites.net/api/?' +
            new URLSearchParams({
                //Här ska vi ha cuisine
                q: cuisine,
                key: apiKey,
            }).toString()
        );
    
        cuisine = '';
    
        if (result.hits.length === 0) {
            message.hidden = false;
            resultList.hidden = true;
        }
        else {
            message.hidden = true;
            resultList.hidden = false;
    
            resultList.replaceChildren();
    
            for (const hit of result.hits) {
                const img = document.createElement('img');
                img.src = hit.webformatURL;
                img.style.width = '250px';
    
                const li = document.createElement('li');
                li.append(img);
    
                resultList.append(li);
            }
        }
    };
} 
