export default function LocationCard({ location, onClose }) {
    if (!location) return null;

    return (
        <div className="location-card">

            <button onClick={onClose}>
                ×
            </button>

            <h2>{location.name}</h2>

            <p>
                Explore activities and quizzes at this location.
            </p>

        </div>
    );
}