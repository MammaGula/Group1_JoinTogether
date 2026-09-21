import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getActivitiesByLocationId } from '../api/activityApi';

function formatDateTime(value) {
    return new Date(value).toLocaleString('sv-SE', {
        dateStyle: 'medium',
        timeStyle: 'short',
    });
}

export default function LocationCard({ location, onClose }) {
    const navigate = useNavigate();
    const [activities, setActivities] = useState([]);
    const [activitiesError, setActivitiesError] = useState(null);

    useEffect(() => {
        if (!location) return;

        let cancelled = false;
        setActivities([]);
        setActivitiesError(null);

        getActivitiesByLocationId(location.id)
            .then((list) => {
                if (cancelled) return;
                const sorted = [...list].sort(
                    (a, b) => new Date(a.scheduledAt) - new Date(b.scheduledAt),
                );
                setActivities(sorted);
            })
            .catch((err) => {
                if (!cancelled) setActivitiesError(err.message);
            });

        return () => {
            cancelled = true;
        };
    }, [location?.id]);

    if (!location) return null;

    const handleStartQuiz = () => {
        navigate(`/quiz/${location.id}`);
    };

    const handleCreateActivity = () => {
        navigate(`/activities/new?locationId=${location.id}`);
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

            <div className="location-activities">
                <div className="location-activities__header">
                    <h3>Aktiviteter</h3>
                    <button
                        className="location-activities__create"
                        onClick={handleCreateActivity}
                    >
                        + Skapa här
                    </button>
                </div>

                {activitiesError && (
                    <p className="location-activities__empty">
                        Kunde inte hämta aktiviteter: {activitiesError}
                    </p>
                )}

                {!activitiesError && activities.length === 0 && (
                    <p className="location-activities__empty">
                        Inga aktiviteter här ännu.
                    </p>
                )}

                <ul className="location-activities__list">
                    {activities.map((a) => (
                        <li key={a.id} className="location-activity">
                            <span className="location-activity__title">{a.title}</span>
                            <span className="location-activity__meta">
                                {formatDateTime(a.scheduledAt)} ·{' '}
                                {a.isFull
                                    ? 'Full'
                                    : `${a.currentParticipants}/${a.maxParticipants} deltagare`}
                            </span>
                        </li>
                    ))}
                </ul>
            </div>
        </div>
    );
}