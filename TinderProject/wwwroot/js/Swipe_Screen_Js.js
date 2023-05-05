//JavaScript for the Swipe Screen aka HomePage.
var swipeCard = document.getElementById("swipe-div");
var likeButton = document.getElementById("like-button");

likeButton.addEventListener("mouseleave", function () {
    var imgs = document.getElementsByClassName("heart-pic");

    Array.from(imgs).forEach(function (img) {
        img.parentNode.removeChild(img);
    });

})

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

dislikeButton.addEventListener("mouseenter", function () {
    swipeCard.classList.add("grey-scale");
})
dislikeButton.addEventListener("mouseleave", function () {
    swipeCard.classList.remove("grey-scale");
})

//CSS for the heart div that appears when there is a match.
var matchDiv = document.getElementById("match-div");

var matchDiv = document.getElementById("match-div");
if (matchDiv != null) {
  setTimeout(function() {
    matchDiv.classList.add("show-match-div");
  }, 500);
}