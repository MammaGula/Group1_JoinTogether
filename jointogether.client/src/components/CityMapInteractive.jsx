import { useRef, useEffect, useState } from 'react'
import mapboxgl from 'mapbox-gl'

import 'mapbox-gl/dist/mapbox-gl.css';
import LocationCard from './LocationCard';

// hardcoded locations for demonstration purposes
const locations = [
    {
        id: 1,
        name: "Malmö Library",
        latitude: 55.6039,
        longitude: 12.9988
    },
    {
        id: 2,
        name: "Turning Torso",
        latitude: 55.6132,
        longitude: 12.9768
    },
    {
        id: 3,
        name: "Malmö Museum",
        latitude: 55.6059,
        longitude: 12.9998
    }
];

export function CityMapInteractive() {

  const mapRef = useRef()
  const mapContainerRef = useRef()

  const [selectedLocation, setSelectedLocation] = useState(null);

  useEffect(() => {
     mapRef.current = new mapboxgl.Map({
      accessToken: import.meta.env.VITE_MAPBOX_ACCESS_TOKEN,
      container: mapContainerRef.current,
      style: 'mapbox://styles/mapbox/standard',
      center:  [13.0038, 55.605],
      zoom: 13,
    });

    // Add markers for each location
    locations.forEach(location => {
      const marker = new mapboxgl.Marker()
        .setLngLat([location.longitude, location.latitude])
        .addTo(mapRef.current);

      // Make markers clickable
      marker.getElement().addEventListener('click', () => {
        setSelectedLocation(location);
      });
    });

    return () => {
      mapRef.current.remove()
    }
  }, [])


return (
  <>
    <div id='mapbox_container' ref={mapContainerRef} />

    <LocationCard 
      location={selectedLocation} 
      onClose={() => setSelectedLocation(null)} />
  </>
  )
}

export default CityMapInteractive
