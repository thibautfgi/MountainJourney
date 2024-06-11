mapboxgl.accessToken = 'pk.eyJ1IjoianVzdGFjaGlwcyIsImEiOiJjbHgwZHgxcGowaG01MmlyMnhhb3dyZXJxIn0.dcYR9tSRJ2AUaoCsJ7gbVA'; // Replace with your Mapbox access token

const map = new mapboxgl.Map({
    container: 'map', // Container ID
    style: 'mapbox://styles/mapbox/streets-v11', // Map style to use
    center: [0, 0], // Starting position [lng, lat]
    zoom: 2 // Starting zoom level
});

document.addEventListener('DOMContentLoaded', fetchMarks);

function fetchMarks() {
    const apiUrl = 'http://localhost:8080/api/marks'; // Ensure this URL is correct

    fetch(apiUrl)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            displayMarks(data);
        })
        .catch(error => {
            console.error('There has been a problem with your fetch operation:', error);
        });
}

function displayMarks(marks) {
    marks.forEach(mark => {
        // Create a HTML element for each feature
        const el = document.createElement('div');
        el.className = 'marker';
        el.style.width = '30px';
        el.style.height = '30px';
        el.style.backgroundColor = 'red';
        el.style.borderRadius = '50%';

        // Make a marker for each feature and add to the map
        new mapboxgl.Marker(el)
            .setLngLat([mark.Mark_Longitude, mark.Mark_Latitude])
            .setPopup(new mapboxgl.Popup({ offset: 25 }) // add popups
            .setHTML(`<h3>${mark.Mark_Name}</h3><p>${mark.Mark_Description}</p>`))
            .addTo(map);
    });
}
