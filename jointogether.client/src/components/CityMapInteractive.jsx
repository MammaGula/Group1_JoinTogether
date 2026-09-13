// import { useRef, useEffect, useState } from 'react'
// import mapboxgl from 'mapbox-gl'

// import 'mapbox-gl/dist/mapbox-gl.css';
// import LocationCard from './LocationCard';
// import { getLocations } from '../api/locationApi';

// export function CityMapInteractive() {
//   const mapRef = useRef()
//   const mapContainerRef = useRef()
//   const markersRef = useRef([])

//   const [locations, setLocations] = useState([])
//   const [selectedLocation, setSelectedLocation] = useState(null)
//   const [error, setError] = useState(null)

//   // Init map once
//   useEffect(() => {
//     mapRef.current = new mapboxgl.Map({
//       accessToken: import.meta.env.VITE_MAPBOX_ACCESS_TOKEN,
//       container: mapContainerRef.current,
//       style: 'mapbox://styles/mapbox/standard',
//       center: [13.0038, 55.605],
//       zoom: 13,
//     });

//     return () => {
//       mapRef.current.remove()
//     }
//   }, [])

//   // Fetch locations from the API once
//   useEffect(() => {
//     getLocations()
//       .then(setLocations)
//       .catch((err) => setError(err.message));
//   }, [])

//   // Add/update markers whenever locations change
//   useEffect(() => {
//     if (!mapRef.current) return;

//     // clear old markers first (avoids duplicates on re-fetch)
//     markersRef.current.forEach(m => m.remove());
//     markersRef.current = [];

//     locations.forEach(location => {
//       const marker = new mapboxgl.Marker()
//         .setLngLat([location.longitude, location.latitude])
//         .addTo(mapRef.current);

//       marker.getElement().addEventListener('click', () => {
//         setSelectedLocation(location);
//       });

//       markersRef.current.push(marker);
//     });
//   }, [locations])

//   return (
//     <>
//       <div id='mapbox_container' ref={mapContainerRef} />

//       {error && <p className="map-error">Could not load locations: {error}</p>}

//       <LocationCard
//         location={selectedLocation}
//         onClose={() => setSelectedLocation(null)} />
//     </>
//   )
// }

// export default CityMapInteractive


import { useRef, useEffect } from 'react'
import mapboxgl from 'mapbox-gl'

import 'mapbox-gl/dist/mapbox-gl.css';

export function CityMapInteractive({ locations, selectedLocation, onSelectLocation }) {
  const mapRef = useRef()
  const mapContainerRef = useRef()
  const markersRef = useRef([])

  // Init map once
  useEffect(() => {
    mapRef.current = new mapboxgl.Map({
      accessToken: import.meta.env.VITE_MAPBOX_ACCESS_TOKEN,
      container: mapContainerRef.current,
      style: 'mapbox://styles/mapbox/standard',
      center: [13.0038, 55.605],
      zoom: 13,
    });

    return () => {
      mapRef.current.remove()
    }
  }, [])

  // Add/update markers whenever the (filtered) location list changes
  useEffect(() => {
    if (!mapRef.current) return;

    markersRef.current.forEach(m => m.remove());
    markersRef.current = [];

    locations.forEach(location => {
      const marker = new mapboxgl.Marker()
        .setLngLat([location.longitude, location.latitude])
        .addTo(mapRef.current);

      marker.getElement().addEventListener('click', () => {
        onSelectLocation(location);
      });

      markersRef.current.push(marker);
    });
  }, [locations, onSelectLocation])

  // Pan/zoom to the selected location, e.g. when clicked from the sidebar list
  useEffect(() => {
    if (selectedLocation && mapRef.current) {
      mapRef.current.flyTo({
        center: [selectedLocation.longitude, selectedLocation.latitude],
        zoom: 15,
      });
    }
  }, [selectedLocation])

  return <div id="mapbox_container" ref={mapContainerRef} />
}

export default CityMapInteractive