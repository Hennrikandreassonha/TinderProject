export default function fakeFetch(urlString, options) {
    const url = new URL(urlString);

    if (url.hostname === 'tinderapp.azurewebsites.net' && url.pathname === '/api/') {

        if (url.searchParams.get('q') === 'Mexican') {
            return {
                "id": 3,
                "accountOwner": "3333333333",
                "name": "Tacos al Pastor",
                "description": "Tacos al Pastor is a popular Mexican street food made with marinated pork cooked on a vertical spit and served in corn tortillas.",
                "primaryIngredient": "Pork",
                "country": "Mexico",
                "category": "Mexican",
                "ingredient": "Pork, pineapple, onions, cilantro, corn tortillas"
            }
        }
        else if (url.searchParams.get('q') === 'Italian') {
            return {

                "id": 1,
                "accountOwner": "3333333333",
                "name": "Pizza Margherita",
                "description": "Pizza Margherita is a traditional Italian pizza topped with fresh mozzarella cheese, tomatoes, and basil leaves.",
                "primaryIngredient": "Tomatoes",
                "country": "Italy",
                "category": "Italian",
                "ingredient": "Pizza dough, mozzarella cheese, tomatoes, basil"
            }
        }
        else if (url.searchParams.get('q') === 'Chinese') {
            return {
                "id": 2,
                "accountOwner": "3333333333",
                "name": "Kung Pao Chicken",
                "description": "Kung Pao Chicken is a classic Chinese dish made with diced chicken, peanuts, and vegetables in a flavorful sauce.",
                "primaryIngredient": "Chicken",
                "country": "China",
                "category": "Chinese",
                "ingredient": "Chicken, peanuts, vegetables, soy sauce, Sichuan peppercorns"
            }
        }
        else if (url.searchParams.get('q') === 'Indian') {
            return {
                "id": 4,
                "accountOwner": "3333333333",
                "name": "Butter Chicken",
                "description": "Butter Chicken, also known as Murgh Makhani, is a creamy and flavorful Indian dish made with marinated chicken cooked in a rich tomato-based sauce.",
                "primaryIngredient": "Chicken",
                "country": "India",
                "category": "Indian",
                "ingredient": "Chicken, tomatoes, butter, cream, spices"
            }
        }
        else if (url.searchParams.get('q') === 'French') {
            return {
                "id": 5,
                "accountOwner": "3333333333",
                "name": "Coq au Vin",
                "description": "Coq au Vin is a classic French dish made with chicken braised in red wine with mushrooms, onions, and bacon.",
                "primaryIngredient": "Chicken",
                "country": "France",
                "category": "French",
                "ingredient": "Chicken, red wine, mushrooms, onions, bacon"
            }
        }
        else if (url.searchParams.get('q') === 'Japanese') {
            return {
                "id": 6,
                "accountOwner": "3333333333",
                "name": "Sushi",
                "description": "Sushi is a traditional Japanese dish made with vinegared rice, fresh fish or seafood, and vegetables.",
                "primaryIngredient": "Fish",
                "country": "Japan",
                "category": "Japanese",
                "ingredient": "Rice, fish/seafood, seaweed, vegetables"
            }
        }
        else if (url.searchParams.get('q') === 'Thai') {
            return {
                "id": 7,
                "accountOwner": "3333333333",
                "name": "Pad Thai",
                "description": "Pad Thai is a popular Thai stir-fried noodle dish made with rice noodles, shrimp or chicken, eggs, tofu, and peanuts.",
                "primaryIngredient": "Shrimp/Chicken",
                "country": "Thailand",
                "category": "Thai",
                "ingredient": "Rice noodles, shrimp/chicken, eggs"
            }
        }
        else if (url.searchParams.get('q') === 'Spanish') {
            return {
                "id": 8,
                "accountOwner": "3333333333",
                "name": "Paella",
                "description": "Paella is a traditional Spanish rice dish made with saffron-infused rice, various meats or seafood, and vegetables.",
                "primaryIngredient": "Rice",
                "country": "Spain",
                "category": "Spanish",
                "ingredient": "Rice, meats/seafood, saffron, vegetables"
            }
        }
        else if (url.searchParams.get('q') === 'Greek') {
            return {
                "id": 9,
                "accountOwner": "3333333333",
                "name": "Moussaka",
                "description": "Moussaka is a classic Greek dish made with layers of eggplant, ground meat, and béchamel sauce, baked to perfection.",
                "primaryIngredient": "Eggplant",
                "country": "Greece",
                "category": "Greek",
                "ingredient": "Eggplant, ground meat, potatoes, tomatoes, béchamel sauce"
            }
        }
        else if (url.searchParams.get('q') === 'Swedish') {
            return {
                "id": 10,
                "accountOwner": "3333333333",
                "name": "Swedish Meatballs",
                "description": "Swedish Meatballs are a traditional Swedish dish made with seasoned ground meatballs served in a creamy gravy, often accompanied by lingonberry sauce.",
                "primaryIngredient": "Ground meat",
                "country": "Sweden",
                "category": "Swedish",
                "ingredient": "Ground meat, breadcrumbs, onions, spices, cream, lingonberry sauce"
            }
        }
        else {
            return {
                total: 0,
                totalHits: 0,
                hits: []
            }
        }
    }
    else {
        throw new Error('This URL has not been implemented in the fake API: ' + urlString);
    }
}