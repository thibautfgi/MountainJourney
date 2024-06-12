document.getElementById('user-form').addEventListener('submit', async function(event) {
    event.preventDefault();

    const user = {
        User_FirstName: document.getElementById('first-name').value,
        User_LastName: document.getElementById('last-name').value,
        User_Email: document.getElementById('email').value,
        User_Phone: document.getElementById('phone').value,
        User_Password: document.getElementById('password').value
    };

    const token = 'abcdef123456'; // Replace with your actual token

    try {
        const response = await fetch('http://localhost:8080/api/users', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(user)
        });

        // Log the raw response
        console.log('Raw response:', response);

        // Check the response's content type
        const contentType = response.headers.get('Content-Type');

        let responseData;
        if (contentType && contentType.includes('application/json')) {
            responseData = await response.json();
        } else {
            responseData = await response.text();
        }

        // Log the parsed response data
        console.log('Parsed response:', responseData);

        if (response.ok) {
            alert('User created successfully!');
        } else {
            alert('Error: ' + responseData.message || responseData);
        }
    } catch (error) {
        console.error('Error:', error);
        alert('An error occurred. Please try again.');
    }
});
