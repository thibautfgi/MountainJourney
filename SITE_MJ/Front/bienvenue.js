// bienvenue.js

document.addEventListener("DOMContentLoaded", function() {
    const messages = [
        "Bienvenue sur Mountain Journey",
        "Explorez les plus beaux sentiers",
        "Créez votre propre voyage",
        "Partagez vos aventures"
    ];

    let currentMessageIndex = 0;
    const rotatingText = document.getElementById("rotating-text");

    function rotateText() {
        rotatingText.textContent = messages[currentMessageIndex];
        rotatingText.classList.remove("fade-in");
        void rotatingText.offsetWidth; // trigger reflow
        rotatingText.classList.add("fade-in");

        currentMessageIndex = (currentMessageIndex + 1) % messages.length;
    }

    rotateText();
    setInterval(rotateText, 3000); // Change message every 3 seconds
});
