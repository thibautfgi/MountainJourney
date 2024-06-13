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
    fetchUserLikes(1); // Fetch liked maps for User_Id = 1

    // Handle form submission for creating a new map
    document.getElementById('create-map-form').addEventListener('submit', createMap);

    // Handle placing a marker on the map
    document.getElementById('place-marker-button').addEventListener('click', () => {
        map.once('click', (e) => {
            const marker = new mapboxgl.Marker()
                .setLngLat([e.lngLat.lng, e.lngLat.lat])
                .addTo(map);
            document.getElementById('marker-lng').value = e.lngLat.lng;
            document.getElementById('marker-lat').value = e.lngLat.lat;
        });
    });

    // Handle form submission for adding a new mark
    document.getElementById('add-marker-form').addEventListener('submit', createMark);
});

function fetchUserMaps(userId) {
    fetch(`http://localhost:8080/api/users/${userId}/maps`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(maps => {
            const cartesContainer = document.getElementById('data-container'); // Ensure this container exists
            cartesContainer.innerHTML = ''; // Clear any existing content
            maps.forEach(map => {
                const mapCard = createMapCard(map);
                cartesContainer.appendChild(mapCard);
            });
        })
        .catch(error => console.error('Error fetching maps:', error));
}

function fetchUserLikes(userId) {
    fetch(`http://localhost:8080/api/users/${userId}/likes`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(likes => {
            const likedMapIds = likes.map(like => like.Map_Id);
            return Promise.all(likedMapIds.map(mapId => fetchMapDetails(mapId)));
        })
        .then(likedMaps => {
            const favorisContainer = document.getElementById('favoris-container');
            favorisContainer.innerHTML = ''; // Clear any existing content
            likedMaps.forEach(map => {
                const mapCard = createMapCard(map);
                favorisContainer.appendChild(mapCard);
            });
        })
        .catch(error => console.error('Error fetching likes:', error));
}

function fetchMapDetails(mapId) {
    return fetch(`http://localhost:8080/api/maps/${mapId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        });
}

function createMap(event) {
    event.preventDefault();

    const mapName = document.getElementById('map-name').value;
    const mapDescription = document.getElementById('map-description').value;
    const mapImage = document.getElementById('map-image').value;

    const mapData = {
        User_Id: 1,
        Map_Name: mapName,
        Map_Description: mapDescription,
        Map_LikeNumber: 0,
        Map_NumberCommentary: 0,
        Map_TravelTime: 0,
        Map_TotalDistance: 0,
        Map_Image: mapImage,
        Map_Rating: 0
    };

    fetch('http://localhost:8080/api/maps', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer abcdef123456'
        },
        body: JSON.stringify(mapData)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    })
    .then(() => {
        location.reload(); // Reload the page on successful map creation
    })
    .catch(error => console.error('Error creating map:', error));
    location.reload();
}

function createMark(event) {
    event.preventDefault();

    const markerName = document.getElementById('marker-name').value;
    const markerDescription = document.getElementById('marker-description').value;
    const markerLat = parseFloat(document.getElementById('marker-lat').value);
    const markerLng = parseFloat(document.getElementById('marker-lng').value);

    const markData = {
        Map_Id: 1, // Assuming you have a Map_Id to associate this mark with
        Mark_Name: markerName,
        Mark_Description: markerDescription,
        Mark_Latitude: markerLat,
        Mark_Longitude: markerLng
    };

    fetch('http://localhost:8080/api/marks', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer abcdef123456'
        },
        body: JSON.stringify(markData)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    })
    .then(() => {
        location.reload(); // Reload the page on successful mark creation
    })
    .catch(error => console.error('Error creating mark:', error));
    location.reload();
}

function createMapCard(map) {
    const mapCard = document.createElement('div');
    mapCard.className = 'card mb-3';
    mapCard.innerHTML = `
        <div class="row no-gutters">
            <div class="col-md-4">
                <img src="${map.Map_Image}" class="card-img" alt="${map.Map_Name}" style="height: 200px; object-fit: cover; border-radius: 10px;">
            </div>
            <div class="col-md-8">
                <div class="card-body">
                    <h5 class="card-title">${map.Map_Name}</h5>
                    <p class="card-text">${map.Map_Description}</p>
                    <p class="card-text">${map.Map_TotalDistance}km - ${map.Map_TravelTime} hours</p>
                    <p class="card-text"><small class="text-muted">Likes: ${map.Map_LikeNumber} Comments: ${map.Map_NumberCommentary} Rating: ${map.Map_Rating}</small></p>
                </div>
            </div>
        </div>
    `;
    return mapCard;
}
