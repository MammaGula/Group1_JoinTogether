import { useState, useEffect, useMemo } from 'react';
import { CityMapInteractive } from '../components/CityMapInteractive';
import { LocationListItem } from '../components/LocationListItem';
import LocationCard from '../components/LocationCard';
import { getLocations } from '../api/locationApi';
import { useAuth } from '../context/AuthContext';
import './HomePage.css';
import { useNavigate } from 'react-router-dom';

export function HomePage() {
  const [locations, setLocations] = useState([]);
  const [error, setError] = useState(null);
  const [selectedLocation, setSelectedLocation] = useState(null);
  const navigate = useNavigate();
  const [activeCategory, setActiveCategory] = useState('Alla');
  const { logout } = useAuth();

  useEffect(() => {
    getLocations()
      .then(setLocations)
      .catch((err) => setError(err.message));
  }, []);

  const categories = useMemo(() => {
    const unique = [...new Set(locations.map((l) => l.category).filter(Boolean))];
    return ['Alla', ...unique];
  }, [locations]);

  const filteredLocations = useMemo(() => {
    if (activeCategory === 'Alla') return locations;
    return locations.filter((l) => l.category === activeCategory);
  }, [locations, activeCategory]);

  return (
    <div className="home">
      <header className="home__header">
        <div className="home__brand">
          <span className="home__brand-dot" />
          <h1>Quizkartan</h1>
        </div>

        <nav className="home__tabs">
          <button className="home__tab home__tab--active">Utforska</button>
          {/* <button className="home__tab">Mina quiz</button> */}
          <button className="home__tab" onClick={() => navigate('/activities/new')}>
            Skapa aktivitet
          </button>
        </nav>

        <div className="home__search">
          <input type="text" placeholder="Sök plats eller kategori" />
        </div>

        <button className="home__logout" onClick={logout}>Logga ut</button>
        

      </header>

      <div className="home__body">
        <aside className="home-sidebar">
          <div className="home-sidebar__filters">
            {categories.map((category) => (
              <button
                key={category}
                className={
                  'filter-pill' +
                  (activeCategory === category ? ' filter-pill--active' : '')
                }
                onClick={() => setActiveCategory(category)}
              >
                {category}
              </button>
            ))}
          </div>

          {error && (
            <p className="home-sidebar__error">Kunde inte hämta platser: {error}</p>
          )}

          <ul className="location-list">
            {filteredLocations.map((location) => (
              <LocationListItem
                key={location.id}
                location={location}
                isSelected={selectedLocation?.id === location.id}
                onSelect={() => setSelectedLocation(location)}
              />
            ))}
          </ul>
        </aside>

        <main className="home-map">
          <CityMapInteractive
            locations={filteredLocations}
            selectedLocation={selectedLocation}
            onSelectLocation={setSelectedLocation}
          />
        </main>

        <aside className="home-detail">
          {selectedLocation ? (
            <LocationCard
              location={selectedLocation}
              onClose={() => setSelectedLocation(null)}
            />
          ) : (
            <p className="home-detail__placeholder">
              Klicka på en plats i listan eller på kartan för att se detaljer och starta ett quiz.
            </p>
          )}
        </aside>
      </div>
    </div>
  );
}