import { MAPBOX_ACCESS_TOKEN } from './config.js';

// Function to get a cookie by name
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

document.addEventListener('DOMContentLoaded', () => {
    mapboxgl.accessToken = MAPBOX_ACCESS_TOKEN;

    // Get User_Id from cookies
    const userId = getCookie('userId');

    // Get lat and lng from URL parameters
    const urlParams = new URLSearchParams(window.location.search);
    const lat = parseFloat(urlParams.get('lat'));
    const lng = parseFloat(urlParams.get('lng'));
    const center = (lat && lng) ? [lng, lat] : [2.3522, 48.8566]; // Default to Paris if no coordinates are provided

    var map = new mapboxgl.Map({
        container: 'map',
        style: 'mapbox://styles/mapbox/streets-v11',
        center: center, // Use the provided coordinates or default to Paris
        zoom: 10 // Starting zoom level
    });

    map.on('load', () => {
        console.log('Map has loaded'); // Debug log
        fetchAllMarksAndRoutes(map); // Fetch and display all markers and routes
    });

    if (userId) {
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

        // Fetch and display user maps and liked maps
        fetchUserMaps(userId);
        fetchUserLikes(userId);

        // Handle form submission for creating a new map
        document.getElementById('create-map-form').addEventListener('submit', (event) => createMap(event, userId));
    } else {
        // Hide sidebar and expand map to full screen if the user is not logged in
        document.getElementById('sidebar').style.display = 'none';
        document.getElementById('mainContent').classList.remove('col-8');
        document.getElementById('mainContent').classList.add('col-12');
        document.getElementById('toggleButton').style.display = 'none';
        setTimeout(() => {
            map.resize();
        }, 20);
    }
});

function fetchAllMarksAndRoutes(map) {
    fetch('http://localhost:8080/api/marks')
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(marks => {
            marks.forEach(mark => {
                new mapboxgl.Marker()
                    .setLngLat([mark.Mark_Longitude, mark.Mark_Latitude])
                    .setPopup(
                        new mapboxgl.Popup({ offset: 25 }) // add popups
                            .setHTML(
                                `<h3>${mark.Mark_Name}</h3><p>${mark.Mark_Description}</p>`
                            )
                    )
                    .addTo(map); // Ensure the map instance is used here
            });
        })
        .catch(error => console.error('Error fetching marks:', error));

    fetch('http://localhost:8080/api/routes')
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(routes => {
            routes.forEach(route => {
                drawRoute(route, map);
            });
        })
        .catch(error => console.error('Error fetching routes:', error));
}

function drawRoute(route, map) {
    Promise.all([
        fetch(`http://localhost:8080/api/marks/${route.Mark_Start}`)
            .then(response => response.json()),
        fetch(`http://localhost:8080/api/marks/${route.Mark_End}`)
            .then(response => response.json())
    ]).then(([startMark, endMark]) => {
        const start = [startMark.Mark_Longitude, startMark.Mark_Latitude];
        const end = [endMark.Mark_Longitude, endMark.Mark_Latitude];

        const directionsRequest = `https://api.mapbox.com/directions/v5/mapbox/driving/${start[0]},${start[1]};${end[0]},${end[1]}?geometries=geojson&access_token=${mapboxgl.accessToken}`;

        fetch(directionsRequest)
            .then(response => response.json())
            .then(data => {
                const routeData = data.routes[0].geometry;

                map.addSource(`route-${route.Route_Id}`, {
                    type: 'geojson',
                    data: {
                        type: 'Feature',
                        properties: {},
                        geometry: routeData
                    }
                });

                map.addLayer({
                    id: `route-${route.Route_Id}`,
                    type: 'line',
                    source: `route-${route.Route_Id}`,
                    layout: {
                        'line-join': 'round',
                        'line-cap': 'round'
                    },
                    paint: {
                        'line-color': '#3887be',
                        'line-width': 5,
                        'line-opacity': 0.75
                    }
                });
            })
            .catch(error => console.error('Error fetching directions:', error));
    }).catch(error => console.error('Error fetching marks for route:', error));
}

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

function createMap(event, userId) {
    event.preventDefault();

    const mapyo = parseInt(userId);
    const mapName = document.getElementById('map-name').value;
    const mapDescription = document.getElementById('map-description').value;
    const mapImage = document.getElementById('map-image').value;

    const mapData = {
        User_Id: mapyo,
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
    mapCard.addEventListener('click', () => {
        window.location.href = `mapdetails.html?mapId=${map.Map_Id}`;
    });
    return mapCard;
}
