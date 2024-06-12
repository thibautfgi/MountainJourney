// auth.js

// Fonction pour obtenir la valeur d'un cookie par son nom
function getCookie(name) {
    let cookies = document.cookie.split(';');
    for (let i = 0; i < cookies.length; i++) {
        let cookie = cookies[i].trim();
        if (cookie.startsWith(name + '=')) {
            return cookie.substring((name + '=').length, cookie.length);
        }
    }
    return '';
}

// Vérifie si l'utilisateur est connecté
function checkAuth() {
    const userId = getCookie('userId');
    if (userId) {
        // L'utilisateur est connecté et la navbar affiche les liens pour un utilisateur connecté
        console.log('Utilisateur connecté');
        updateNavbarForLoggedInUser();
    } else {
        // Affiche une navbar différente pour un utilisateur déconnecté
        updateNavbarForLoggedOutUser();
    }
}

// Met à jour la barre de navigation pour un utilisateur connecté
function updateNavbarForLoggedInUser() {
    const navBar = document.querySelector('.navbar-nav.ml-auto');
    navBar.innerHTML = `
        <li class="nav-item">
            <a class="nav-link" href="carte.html">Carte</a>
        </li>
        <li class="nav-item">
            <a class="nav-link" href="profile.html">Profil</a>
        </li>
    `;
}

// Met à jour la barre de navigation pour un utilisateur déconnecté
function updateNavbarForLoggedOutUser() {
    const navBar = document.querySelector('.navbar-nav.ml-auto');
    navBar.innerHTML = `
        <li class="nav-item">
            <a class="nav-link" href="carte.html">Carte</a>
        </li>
        <li class="nav-item">
            <a class="btn btn-primary" href="connexion.html">Se connecter</a>
        </li>
    `;
}

// Appel checkAuth lorsque la page se charge pour vérifier si l'utilisateur est connecté
window.onload = checkAuth;
