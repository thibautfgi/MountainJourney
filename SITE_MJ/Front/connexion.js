//  Pour la page de connexion

document.getElementById('login-form').addEventListener('submit', function(event) {
    event.preventDefault();
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    // Envoie une requête API pour vérifier les informations d'identification
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
        console.log('Utilisateur non connecté, redirection vers la page de connexion');
    }
}

// // Appel checkAuth lorsque la page se charge pour vérifier si l'utilisateur est connecté
// window.onload = checkAuth;


//  -----------------

//  Création de compte

document.getElementById('signup-form').addEventListener('submit', async function(event) {
    event.preventDefault();

    const firstName = document.getElementById('signup-firstname').value;
    const lastName = document.getElementById('signup-lastname').value;
    const email = document.getElementById('signup-email').value;
    const password = document.getElementById('signup-password').value;
    const confirmPassword = document.getElementById('signup-confirm-password').value;
    const phone = document.getElementById('signup-phone').value;

    if (password !== confirmPassword) {
        alert('Les mots de passe ne correspondent pas.');
        return;
    }

    const authToken = 'abcdef123456'; // Token

    try {
        const response = await fetch('http://localhost:8080/api/users', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify({
                User_FirstName: firstName,
                User_LastName: lastName,
                User_Email: email,
                User_Password: password,
                User_Phone: phone
            })
        });

        // Vérif le statut de la réponse
        if (response.ok) {
            alert('Compte créé avec succès !');
            $('#signupModal').modal('hide');
        } else {
            // Si la réponse n'est pas au format JSON valide, affiche un message d'erreur
            const responseText = await response.text();
            console.error('Réponse brute:', responseText);
            alert('Erreur lors de la création du compte: ' + responseText);
        }
    } catch (error) {
        console.error('Erreur:', error);
        alert('Une erreur est survenue lors de la création du compte. Veuillez réessayer.');
    }
});





