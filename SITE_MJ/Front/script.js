// Remplace 'YOUR_MAPBOX_ACCESS_TOKEN' par ton propre token d'accès Mapbox
mapboxgl.accessToken = 'yohide';

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
