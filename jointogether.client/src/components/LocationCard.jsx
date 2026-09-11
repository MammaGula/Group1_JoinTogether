import { useNavigate } from 'react-router-dom';

export default function LocationCard({ location, onClose }) {
    const navigate = useNavigate();

    if (!location) return null;

    const handleStartQuiz = () => {
        navigate(`/quiz/${location.id}`);
    };

    return (
        <div className="location-card">
            <button onClick={onClose} aria-label="Close">
                ×
            </button>

            <h2>{location.name}</h2>

            <p className="location-card-description">
                {location.description}
            </p>

            <button className="start-quiz-button" onClick={handleStartQuiz}>
                Start quiz
            </button>
        </div>
    );
}