import { MAPBOX_ACCESS_TOKEN } from './config.js';

document.addEventListener('DOMContentLoaded', () => {
    mapboxgl.accessToken = MAPBOX_ACCESS_TOKEN;
    const mapId = new URLSearchParams(window.location.search).get('mapId');
    if (!mapId) {
        alert('No map ID provided!');
        return;
    }

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

    // Fetch and display map details
    fetchMapDetails(mapId);

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

    // Handle form submission for adding a new marker
    document.getElementById('add-marker-form').addEventListener('submit', addMarker);

    // Handle form submission for adding a new route
    document.getElementById('add-route-form').addEventListener('submit', addRoute);

    // Fetch and populate marks for route creation
    fetchUserMarks(mapId); // Fetch marks for the specific map
});

function fetchMapDetails(mapId) {
    fetch(`http://localhost:8080/api/maps/${mapId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(map => {
            const mapInfoContainer = document.getElementById('map-info-container');
            mapInfoContainer.innerHTML = `
                <h3>${map.Map_Name}</h3>
                <p>${map.Map_Description}</p>
                <p>${map.Map_TotalDistance}km - ${map.Map_TravelTime} hours</p>
                <p>Likes: ${map.Map_LikeNumber} Comments: ${map.Map_NumberCommentary} Rating: ${map.Map_Rating}</p>
            `;
        })
        .catch(error => console.error('Error fetching map details:', error));
}

function fetchUserMarks(mapId) {
    fetch(`http://localhost:8080/api/maps/${mapId}/marks`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(marks => {
            populateMarkDropdown('mark-start', marks);
            populateMarkDropdown('mark-end', marks);
        })
        .catch(error => console.error('Error fetching marks:', error));
}

function populateMarkDropdown(dropdownId, marks) {
    const dropdown = document.getElementById(dropdownId);
    dropdown.innerHTML = ''; // Clear any existing content
    marks.forEach(mark => {
        const option = document.createElement('option');
        option.value = mark.Mark_Id;
        option.text = mark.Mark_Name;
        dropdown.appendChild(option);
    });
}

function addMarker(event) {
    event.preventDefault();

    const markerName = document.getElementById('marker-name').value;
    const markerDescription = document.getElementById('marker-description').value;
    const markerLat = parseFloat(document.getElementById('marker-lat').value);
    const markerLng = parseFloat(document.getElementById('marker-lng').value);

    const markerData = {
        Map_Id: 1, // Use the actual map ID
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
        body: JSON.stringify(markerData)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    })
    .then(() => {
        location.reload(); // Reload the page on successful marker creation
    })
    .catch(error => console.error('Error creating marker:', error));
}

function addRoute(event) {
    event.preventDefault();

    const routeName = document.getElementById('route-name').value;
    const routeDescription = document.getElementById('route-description').value;
    const markStart = parseInt(document.getElementById('mark-start').value);
    const markEnd = parseInt(document.getElementById('mark-end').value);

    const routeData = {
        Route_Name: routeName,
        Route_Description: routeDescription,
        Mark_Start: markStart,
        Mark_End: markEnd,
        Route_Distance: null // Calculate this on the server side if needed
    };

    fetch('http://localhost:8080/api/routes', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer abcdef123456'
        },
        body: JSON.stringify(routeData)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    })
    .then(() => {
        location.reload(); // Reload the page on successful route creation
    })
    .catch(error => console.error('Error creating route:', error));
}
