import { useParams } from "react-router-dom";

export function QuizPage() {
  const { locationId } = useParams();
  return <div>Quiz for location {locationId} — coming soon</div>;
}
