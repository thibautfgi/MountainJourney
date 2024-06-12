document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('create-map-form').addEventListener('submit', createMap);
});

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
        return response.text();
    })
    .then(responseText => {
        document.getElementById('response-message').innerText = responseText;
    })
    .catch(error => {
        console.error('Error creating map:', error);
        document.getElementById('response-message').innerText = `Error creating map: ${error.message}`;
    });
}
