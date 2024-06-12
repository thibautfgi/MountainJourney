document.getElementById('fetch-data-button').addEventListener('click', fetchData);

function fetchData() {
    const apiUrl = 'http://localhost:8080/api/users'; // Ensure this URL is correct

    fetch(apiUrl)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            displayData(data);
        })
        .catch(error => {
            console.error('There has been a problem with your fetch operation:', error);
        });
}

function displayData(users) {
    const dataContainer = document.getElementById('data-container');
    dataContainer.innerHTML = ''; // Clear previous data

    users.forEach(user => {
        const userElement = document.createElement('div');
        userElement.className = 'user-item';
        userElement.innerHTML = `
            <p><strong>ID:</strong> ${user.User_Id}</p>
            <p><strong>First Name:</strong> ${user.User_FirstName}</p>
            <p><strong>Last Name:</strong> ${user.User_LastName}</p>
            <p><strong>Phone:</strong> ${user.User_Phone}</p>
            <p><strong>Email:</strong> ${user.User_Email}</p>
        `;
        dataContainer.appendChild(userElement);
    });
}
