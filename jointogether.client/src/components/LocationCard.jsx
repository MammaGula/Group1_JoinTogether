import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getActivitiesByLocationId, joinActivity } from "../api/activityApi";
import { getQuizStatus } from "../api/quizApi";

function formatDateTime(value) {
  return new Date(value).toLocaleString("sv-SE", {
    dateStyle: "medium",
    timeStyle: "short",
  });
}

export default function LocationCard({ location, onClose }) {
  const navigate = useNavigate();
  const [activities, setActivities] = useState([]);
  const [activitiesError, setActivitiesError] = useState(null);

  const [hasPassed, setHasPassed] = useState(false);
  const [quizStatusLoading, setQuizStatusLoading] = useState(true);

  useEffect(() => {
    if (!location) return;

    let cancelled = false;
    setActivities([]);
    setActivitiesError(null);
    setHasPassed(false);
    setQuizStatusLoading(true);

    // check if the quiz has passed
    getQuizStatus(location.id)
      .then((status) => {
        if (cancelled) return;
        setHasPassed(status.hasPassed);
      })
      .catch((err) => {
        if (!cancelled) {
          console.error("Failed to fetch quiz status:", err);
          setHasPassed(false);
        }
      })
      .finally(() => {
        if (!cancelled) {
          setQuizStatusLoading(false);
        }
      });

    // fetch activities for the location
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

  const handleViewActivities = () => {
    document
      .querySelector(".location-activities")
      ?.scrollIntoView({ behavior: "smooth" });
  };

  const handleJoinActivity = (activityTitle, scheduledAt) => {
    alert(
      `You have joined activity ${activityTitle} scheduled at ${scheduledAt}!`,
    );
  };

  return (
    <div className="location-card">
      <button onClick={onClose} aria-label="Close">
        ×
      </button>

      <h2>{location.name}</h2>

      {quizStatusLoading ? (
        <p>Laddar quizstatus...</p>
      ) : hasPassed ? (
        <div className="location-card_passed">
          <p> You have already passed this quiz.</p>
          <button className="start-quiz-button" onClick={handleViewActivities}>
            Visa activiteter
          </button>


          <button className="start-quiz-button" onClick={handleCreateActivity}>
            Skapa activitet
          </button>
        </div>
      ) : (
        <div className="location-card_quiz">
          <p className="location-card-description">{location.description}</p>
          <button className="start-quiz-button" onClick={handleStartQuiz}>
            Starta quiz
          </button>
        </div>
      )}

      <div className="location-activities">
        <div className="location-activities__header">
          <h3>Aktiviteter</h3>
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
              <div>
                <span className="location-activity__title">{a.title}</span>
                <span className="location-activity__meta">
                  {formatDateTime(a.scheduledAt)} ·{" "}
                  {a.isFull
                    ? "Full"
                    : `${a.currentParticipants}/${a.maxParticipants} deltagare`}
                </span>
              </div>

              {hasPassed && !a.isFull && (
                <button
                  className="start-quiz-button"
                  onClick={() => handleJoinActivity(a.title, a.scheduledAt)}
                >
                  Gå med
                </button>
              )}
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}