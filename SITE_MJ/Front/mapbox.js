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
});
