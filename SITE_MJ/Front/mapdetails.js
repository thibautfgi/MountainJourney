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

    map.on('load', () => {
        console.log('Map has loaded'); // Debug log
        fetchMapMarks(mapId, map); // Pass the map instance to fetchMapMarks
        fetchAndDrawRoutes(mapId, map); // Fetch and draw routes
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

    document.getElementById('backButton').addEventListener('click', () => {
        window.location.href = 'mapbox.html';
    });

    // Fetch and display map details
    fetchMapDetails(mapId);
    // Fetch and display comments
    fetchComments(mapId);

    // Handle form submission for adding a new comment
    document.getElementById('add-comment-form').addEventListener('submit', addComment);

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

    // Hide sidebar if user is not logged in
    const userId = getCookie('userId');
    if (!userId) {
        document.getElementById('sidebar').style.display = 'none';
        document.getElementById('mainContent').classList.remove('col-8');
        document.getElementById('mainContent').classList.add('col-12');
        document.getElementById('toggleButton').style.display = 'none';
        setTimeout(() => {
            map.resize();
        }, 20);
    }
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
                <img src="${map.Map_Image}" class="card-img p-3" alt="${map.Map_Name}" style="height: 200px; object-fit: cover; border-radius: 10px;">
                <h3>${map.Map_Name}</h3>
                <p>${map.Map_Description}</p>
                <p>${map.Map_TotalDistance}km - ${map.Map_TravelTime} hours</p>
                <p>Likes: ${map.Map_LikeNumber} Comments: ${map.Map_NumberCommentary} Rating: ${map.Map_Rating}</p>
            `;
        })
        .catch(error => console.error('Error fetching map details:', error));
}

function fetchMapMarks(mapId, map) {
    fetch(`http://localhost:8080/api/maps/${mapId}/marks`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(marks => {
            marks.forEach(mark => {
                const marker = new mapboxgl.Marker()
                    .setLngLat([mark.Mark_Longitude, mark.Mark_Latitude])
                    .setPopup(
                        new mapboxgl.Popup({ offset: 25 }) // add popups
                            .setHTML(
                                `<h3>${mark.Mark_Name}</h3>
                                <p>${mark.Mark_Description}</p>
                                <button class="btn-delete" onclick="deleteMarker(${mark.Mark_Id})" style="background-color: #C0392B;">Delete</button>`
                            )
                    )
                    .addTo(map); // Ensure the map instance is used here
            });
        })
        .catch(error => console.error('Error fetching marks:', error));
}

function fetchAndDrawRoutes(mapId, map) {
    fetch(`http://localhost:8080/api/maps/${mapId}/routes`)
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
    const mapId = new URLSearchParams(window.location.search).get('mapId');
    const mapyo = parseInt(mapId);
    const markerName = document.getElementById('marker-name').value;
    const markerDescription = document.getElementById('marker-description').value;
    const markerLat = parseFloat(document.getElementById('marker-lat').value);
    const markerLng = parseFloat(document.getElementById('marker-lng').value);

    const markerData = {
        Map_Id: mapyo, // Use the actual map ID
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
    location.reload();
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
    location.reload();
}

function fetchComments(mapId) {
    fetch(`http://localhost:8080/api/maps/${mapId}/comments`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(async comments => {
            const commentsContainer = document.getElementById('comments-container');
            commentsContainer.innerHTML = ''; // Clear any existing content

            for (let comment of comments) {
                const userResponse = await fetch(`http://localhost:8080/api/users/${comment.User_Id}`);
                if (!userResponse.ok) {
                    throw new Error(`HTTP error! status: ${userResponse.status}`);
                }
                const user = await userResponse.json();
                const commentCard = createCommentCard(comment, user.User_FirstName, user.User_LastName);
                commentsContainer.appendChild(commentCard);
            }
        })
        .catch(error => console.error('Error fetching comments:', error));
}

function createCommentCard(comment, userFirstName, userLastName) {
    const commentCard = document.createElement('div');
    commentCard.className = 'card mb-3';
    commentCard.innerHTML = `
        <div class="card-body">
            <h5 class="card-title">${userFirstName} ${userLastName}</h5>
            <p class="card-text">${comment.Comment_Content}</p>
            <p class="card-text"><small class="text-muted">${comment.Comment_Date}</small></p>
        </div>
    `;
    return commentCard;
}

function addComment(event) {
    event.preventDefault();

    const mapId = new URLSearchParams(window.location.search).get('mapId');
    const userId = getCookie('userId');
    const commentContent = document.getElementById('comment-content').value;
    const commentDate = new Date().toISOString().slice(0, 19).replace('T', ' '); // Format date as 'YYYY-MM-DD HH:MM:SS'

    const commentData = {
        User_Id: parseInt(userId),
        Map_Id: parseInt(mapId),
        Comment_Content: commentContent,
        Comment_Date: commentDate
    };

    fetch('http://localhost:8080/api/comments', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer abcdef123456'
        },
        body: JSON.stringify(commentData)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
        
    })
    .then(() => {
        document.getElementById('comment-content').value = ''; // Clear the comment input field
        fetchComments(mapId); // Reload comments to display the new one
    })
    .catch(error => console.error('Error adding comment:', error));
    location.reload();
}

// Ensure the deleteMarker function is in the global scope
window.deleteMarker = function (markId) {
    fetch(`http://localhost:8080/api/marks/${markId}`, {
        method: 'DELETE',
        headers: {
            'Authorization': 'Bearer abcdef123456'
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        location.reload(); // Reload the page on successful marker deletion
    })
    .catch(error => console.error('Error deleting marker:', error));
    
    
}
