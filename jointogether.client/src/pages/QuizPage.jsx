import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getQuizByLocationId } from "../api/quizApi";
import { ApiError } from "../api/httpClient";

export function QuizPage() {
  const { locationId } = useParams();
  const navigate = useNavigate();
  const [quiz, setQuiz] = useState(null);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let isMounted = true;

    getQuizByLocationId(locationId)
      .then((data) => {
        if (isMounted) setQuiz(data);
      })
      .catch((err) => {
        // Handle missing/invalid location data
        if (isMounted) {
          setError(
            err instanceof ApiError
              ? err.message
              : "Could not load quiz for this location",
          );
        }
      })
      .finally(() => {
        if (isMounted) setIsLoading(false);
      });

    return () => {
      isMounted = false;
    };
  }, [locationId]);

  if (isLoading) return <div>Loading quiz...</div>;

  if (error) {
    return (
      <div>
        <p>{error}</p>
        <button onClick={() => navigate("/")}>Back to map</button>
      </div>
    );
  }

  return (
    <div>
      <h1>{quiz.locationName}</h1>
      {quiz.questions.map((q) => (
        <div key={q.id}>
          <p>{q.text}</p>
          <ul>
            {q.options.map((opt) => (
              <li key={opt.id}>{opt.text}</li>
            ))}
          </ul>
        </div>
      ))}
    </div>
  );
}
