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

// Fonction pour vérifier si l'utilisateur est connecté
function checkAuth() {
    const userId = getCookie('userId');
    if (!userId) {
        window.location.href = 'connexion.html';
    }
    return userId;
}

// Fonction pour récupérer les informations de l'utilisateur
async function getUserInfo(userId) {
    const authToken = 'abcdef123456'; // Remplacez par le vrai token

    const response = await fetch(`http://localhost:8080/api/users/${userId}`, {
        headers: {
            'Authorization': `Bearer ${authToken}`
        }
    });
    const data = await response.json();
    return data;
}

// Fonction pour mettre à jour les informations de l'utilisateur
async function updateUserProfile(userId, user) {
    const authToken = 'abcdef123456'; // Remplacez par le vrai token

    const response = await fetch(`http://localhost:8080/api/users/${userId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${authToken}`
        },
        body: JSON.stringify(user)
    });

    if (!response.ok) {
        const errorData = await response.text();
        throw new Error(errorData || 'Erreur lors de la mise à jour du profil');
    }
    return await response.json();
}

// Fonction pour supprimer le compte de l'utilisateur
async function deleteUserAccount(userId) {
    const authToken = 'abcdef123456'; // Remplacez par le vrai token

    const response = await fetch(`http://localhost:8080/api/users/${userId}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${authToken}`
        }
    });

    if (!response.ok) {
        const errorData = await response.text();
        throw new Error(errorData || 'Erreur lors de la suppression du compte');
    }
}

// Fonction pour se déconnecter
function logout() {
    document.cookie = 'userId=; path=/; max-age=0';
    window.location.href = 'index.html';
}

// Appel lors du chargement de la page
window.onload = async function() {
    const userId = checkAuth();
    const userInfo = await getUserInfo(userId);

    document.getElementById('profile-firstname').value = userInfo.User_FirstName;
    document.getElementById('profile-lastname').value = userInfo.User_LastName;
    document.getElementById('profile-email').value = userInfo.User_Email;
    document.getElementById('profile-password').value = userInfo.User_Password;
    document.getElementById('profile-phone').value = userInfo.User_Phone || '';
};

// Gérer la soumission du formulaire de profil
document.getElementById('profile-form').addEventListener('submit', async function(event) {
    event.preventDefault();

    const userId = getCookie('userId');
    const user = {
        User_FirstName: document.getElementById('profile-firstname').value,
        User_LastName: document.getElementById('profile-lastname').value,
        User_Email: document.getElementById('profile-email').value,
        User_Password: document.getElementById('profile-password').value,
        User_Phone: document.getElementById('profile-phone').value
    };

    try {
        const updatedUser = await updateUserProfile(userId, user);
        alert('Profil mis à jour avec succès!');
        console.log('Profil mis à jour:', updatedUser);
    } catch (error) {
        console.error('Erreur:', error);
        alert('Une erreur est survenue lors de la mise à jour du profil. Veuillez réessayer.');
    }
});

// Gérer la suppression du compte
document.getElementById('delete-account-btn').addEventListener('click', async function() {
    if (confirm('Êtes-vous sûr de vouloir supprimer votre compte ? Cette action est irréversible.')) {
        const userId = getCookie('userId');
        try {
            await deleteUserAccount(userId);
            alert('Compte supprimé avec succès!');
            logout();
        } catch (error) {
            console.error('Erreur:', error);
            alert('Une erreur est survenue lors de la suppression du compte. Veuillez réessayer.');
        }
    }
});

document.getElementById('toggle-password').addEventListener('click', function() {
    const passwordField = document.getElementById('profile-password');
    const type = passwordField.getAttribute('type') === 'password' ? 'text' : 'password';
    passwordField.setAttribute('type', type);
    this.classList.toggle('fa-eye-slash');
});



// Gérer la déconnexion
document.getElementById('logout-btn').addEventListener('click', logout);
