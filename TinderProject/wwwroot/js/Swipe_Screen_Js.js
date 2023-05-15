//JavaScript for the Swipe Screen aka HomePage.
var swipeCard = document.getElementById("swipe-div");
var likeButton = document.getElementById("like-button");

if (likeButton != null) {
    likeButton.addEventListener("mouseleave", function () {
        var imgs = document.getElementsByClassName("heart-pic");

        Array.from(imgs).forEach(function (img) {
            img.parentNode.removeChild(img);
        });

    })
}

if (likeButton != null) {
    likeButton.addEventListener("mouseenter", function () {
        //Adding a random amount of hearths when user is hovering the like button
        //Indicating that the person in picture might be the one.

        const amountHearts = Math.floor(Math.random() * 15) + 8;

        for (let index = 0; index < amountHearts; index++) {
            var img = document.createElement("img");

            //Sätter hur mkt margin till sidan.
            const marginLeft = Math.floor(Math.random() * 100);
            img.style.left = `${marginLeft}%`;

            //Margin till toppen
            const marginTop = Math.floor(Math.random() * 85);
            img.style.top = `${marginTop}%`;

            swipeCard.appendChild(img);

            img.setAttribute("src", "../Pictures/heart.png");
            img.classList.add('heart-pic');
        }

        Showhearts(amountHearts);
    })
}

function Showhearts(amountNewHearts) {
    var imgs = document.getElementsByClassName("heart-pic");

    //Removing those hearts who already has the class.
    const filteredImgs = Array.from(imgs).filter(img => !img.classList.contains('heart-pic-show'));

    for (let i = 0; i < amountNewHearts; i++) {
        //Putting a timeout making the hearts apear with 0.5s delay.
        setTimeout(function () {

            //Random storlek
            let heartWidth = Math.floor(Math.random() * 31) + 10;

            filteredImgs[i].style.width = `${heartWidth}px`;

            //Random rotation.
            const rotation = Math.floor(Math.random() * 21) - 10;
            filteredImgs[i].style.transform = `rotate(${rotation}deg)`;


            filteredImgs[i].classList.add('heart-pic-show');
        }, i * 500);
    }

}

var dislikeButton = document.getElementById("dislike-button");

if (dislikeButton != null) {
    dislikeButton.addEventListener("mouseenter", function () {
        swipeCard.classList.add("grey-scale");
    })
    dislikeButton.addEventListener("mouseleave", function () {
        swipeCard.classList.remove("grey-scale");
    })
}

//CSS for the heart div that appears when there is a match.
//And super like popup.
var popup = document.getElementById("popup");
if (popup != null) {
    setTimeout(function () {
        popup.classList.add("show-popup");
    }, 250);

    //The button for sending message.
    var superLikeBtn = document.getElementById('send-super-message');
    superLikeBtn.addEventListener("click", function () {
        if (!validateSuperMsg()) {
            return;
        }

        document.getElementById("send-super-message").classList.add("display-none");
        document.getElementById("msg-sent-popup").classList.add("display-block");
        document.getElementById("msg-sent-popup").classList.add("show-popup-full");
    })
}


//Changing smart matching values.
function submitForm() {
    const form = document.getElementById("radio-form");
    const radios = document.getElementsByName("smartMatching");

    radios.forEach(element => {
        if (element.checked) {
            value = element.value;
        }
    });

    form.submit();
}

//Putting greyscale if user isnt premium.
var superLikeBtn = document.getElementById('superlike-btn');
if (superLikeBtn != null) {
    if (superLikeBtn.disabled) {
        document.getElementById('super-like-pic').classList.add('disabled-btn')
    }
}



function validateSuperMsg() {
    var messageInput = document.getElementById("message-input").value;
    var errorMessage = document.getElementById("error-message");

    if (messageInput === "" || messageInput == null) {
        errorMessage.classList.add("display-block")
        return false;
    } else {
        return true;
    }
}