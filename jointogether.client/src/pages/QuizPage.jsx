import { useParams } from 'react-router-dom';

export function QuizPage() {
  const { locationId } = useParams();

  return (
    <div className="quiz-page">
      <h1>Quiz for location {locationId}</h1>
    </div>
  );
}

export default QuizPage;
