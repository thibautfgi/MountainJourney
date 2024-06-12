import { MAPBOX_ACCESS_TOKEN } from './config.js';

document.addEventListener('DOMContentLoaded', () => {
    mapboxgl.accessToken = MAPBOX_ACCESS_TOKEN;
    var map = new mapboxgl.Map({
        container: 'map',
        style: 'mapbox://styles/mapbox/streets-v11',
        center: [2.3522, 48.8566], // Starting position [lng, lat]
        zoom: 10 // Starting zoom level
    });

    document.getElementById('toggleButton').addEventListener('click', function () {
        const sidebar = document.getElementById('sidebar');
        const mainContent = document.getElementById('mainContent');
        if (sidebar.classList.contains('show')) {
            sidebar.classList.remove('show');
            mainContent.classList.remove('col-8');
            mainContent.classList.add('col-12');
            this.innerHTML = '&#10095;';
        } else {
            sidebar.classList.add('show');
            mainContent.classList.remove('col-12');
            mainContent.classList.add('col-8');
            this.innerHTML = '&#10094;';
        }
        // Resize the map after the layout change
        setTimeout(() => {
            map.resize();
        }, 20);
    });

    // Fetch and display user maps
    fetchUserMaps(1); // Fetch maps for User_Id = 1
});

function fetchUserMaps(userId) {
    fetch(`/api/users/${userId}/maps`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(maps => {
            const cartesContainer = document.getElementById('cartes');
            cartesContainer.innerHTML = ''; // Clear any existing content
            maps.forEach(map => {
                const mapCard = document.createElement('div');
                mapCard.className = 'card mb-3';
                mapCard.innerHTML = `
                    <img src="${map.Map_Image}" class="card-img-top" alt="${map.Map_Name}">
                    <div class="card-body">
                        <h5 class="card-title">${map.Map_Name}</h5>
                        <p class="card-text">${map.Map_TotalDistance}km - ${map.Map_TravelTime} hours</p>
                        <p class="card-text">${map.Map_Description}</p>
                        <p class="card-text"><small class="text-muted">Likes: ${map.Map_LikeNumber} Comments: ${map.Map_NumberCommentary}</small></p>
                    </div>
                `;
                cartesContainer.appendChild(mapCard);
            });
        })
        .catch(error => console.error('Error fetching maps:', error));
}
