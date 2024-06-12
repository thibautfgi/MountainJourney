//  Pour la page de connexion

document.getElementById('login-form').addEventListener('submit', function(event) {
    event.preventDefault();
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    // Envoye une requête à l'API pour vérifier les informations d'identification
    fetch('http://localhost:8080/api/users')
    .then(response => response.json())
    .then(users => {
        const user = users.find(user => user.User_Email === email && user.User_Password === password);
        if (user) {
            // Connexion réussie
            alert('Connexion réussie !');
            // Stock les infos de l'utilisateur pour une utilisation ultérieure
            document.cookie = `userId=${user.User_Id}; path=/; max-age=${60*60*24*7}`;
            // Redirige vers la page d'accueil
            window.location.href = 'index.html';
        } else {
            // Affiche un message en cas d'erreur
            alert('E-mail ou mot de passe incorrect');
        }
    })
    .catch(error => {
        console.error('Erreur:', error);
        alert('Une erreur est survenue lors de la connexion. Veuillez réessayer.');
    });
});

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
        // L'utilisateur est connecté
        console.log('Utilisateur connecté');
    } else {
        // Si l'utilisateur n'est pas connecté, redirigez-le vers la page de connexion
        window.location.href = 'connexion.html';
    }
}

// Appeler checkAuth lorsque la page se charge pour vérifier si l'utilisateur est connecté
window.onload = checkAuth;


//  -----------------