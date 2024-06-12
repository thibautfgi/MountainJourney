import { MAPBOX_ACCESS_TOKEN } from './config.js';

mapboxgl.accessToken = MAPBOX_ACCESS_TOKEN;

const parisCoordinates = [2.3522, 48.8566]; // Coordonnées de Paris

// Initialisation de la carte
const map = new mapboxgl.Map({
    container: 'map', // ID de l'élément HTML où la carte sera rendue
    style: 'mapbox://styles/mapbox/streets-v11', // Style de la carte
    center: parisCoordinates, // Centre initial de la carte
    zoom: 12 // Niveau de zoom initial
});

// Ajout des contrôles de navigation (zoom et rotation)
map.addControl(new mapboxgl.NavigationControl());

// Fonction pour recentrer la carte sur Paris
function centerMapOnParis() {
    map.flyTo({
        center: parisCoordinates,
        essential: true // Ce paramètre garantit l'utilisation de l'animation essentielle pour une meilleure expérience utilisateur
    });
}

// Ajout d'un écouteur d'événement pour le bouton
document.getElementById('parcours-Paris').addEventListener('click', centerMapOnParis);

document.getElementById('login-form').addEventListener('submit', async function(event) {
    event.preventDefault();
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    // Simuler une vérification des informations d'identification et utiliser le token existant
    if (email === 'user@example.com' && password === 'password123') {
        alert('Connexion réussie !');
        // Stocker le token pour une utilisation ultérieure
        localStorage.setItem('token', 'abcdef123456');
        
        // Récupérer les informations des utilisateurs
        try {
            const response = await fetch('http://localhost:8080/api/users', {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer abcdef123456'
                }
            });

            if (response.ok) {
                const users = await response.json();
                console.log('Utilisateurs :', users);
            } else {
                console.error('Erreur lors de la récupération des utilisateurs');
            }
        } catch (error) {
            console.error('Une erreur est survenue :', error);
        }
    } else {
        alert('E-mail ou mot de passe incorrect');
    }
});



