import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getQuizByLocationId } from "../api/quizApi";
import { ApiError } from "../api/httpClient";
import "./QuizPage.css";

const STAGE = {
  START: "start",
  QUESTION: "question",
  DONE: "done",
};

export function QuizPage() {
  const { locationId } = useParams();
  const navigate = useNavigate();

  const [quiz, setQuiz] = useState(null);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  const [stage, setStage] = useState(STAGE.START);
  const [questionIndex, setQuestionIndex] = useState(0);
  const [selectedOptionId, setSelectedOptionId] = useState(null);

  useEffect(() => {
    let isMounted = true;

    getQuizByLocationId(locationId)
      .then((data) => {
        if (isMounted) setQuiz(data);
      })
      .catch((err) => {
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

  if (isLoading) return <div className="quiz-page">Laddar quiz...</div>;

  if (error) {
    return (
      <div className="quiz-page">
        <p>{error}</p>
        <button onClick={() => navigate("/")}>Tillbaka till kartan</button>
      </div>
    );
  }

  const questions = quiz.questions ?? [];
  const currentQuestion = questions[questionIndex];

  const handleStart = () => {
    setQuestionIndex(0);
    setSelectedOptionId(null);
    setStage(STAGE.QUESTION);
  };

  const handleSelectOption = (optionId) => {
    setSelectedOptionId(optionId);
  };

  const handleNext = () => {
    if (selectedOptionId === null) return; // require an answer before advancing

    if (questionIndex + 1 < questions.length) {
      setQuestionIndex((i) => i + 1);
      setSelectedOptionId(null);
    } else {
      setStage(STAGE.DONE);
    }
  };

  if (stage === STAGE.START) {
    return (
      <div className="quiz-page">
        <div className="quiz-start-card">
          <p className="quiz-start-card__eyebrow">{quiz.locationName}</p>
          <h1>Quiz: {quiz.locationName}</h1>
          <p className="quiz-start-card__description">
            Testa vad du vet om platsen innan du utforskar den själv.
          </p>
          <div className="quiz-start-card__meta">
            <span>{questions.length} frågor</span>
            <span>~{Math.max(1, Math.round(questions.length * 0.5))} min</span>
          </div>
          <button
            className="quiz-start-card__submit"
            onClick={handleStart}
            disabled={questions.length === 0}
          >
            Starta quiz
          </button>
          <button className="quiz-start-card__back" onClick={() => navigate("/")}>
            Tillbaka till kartan
          </button>
        </div>
      </div>
    );
  }

  if (stage === STAGE.DONE) {
    return (
      <div className="quiz-page">
        <div className="quiz-start-card">
          <h1>Bra jobbat!</h1>
          <p className="quiz-start-card__description">
            Du har gått igenom alla {questions.length} frågor om {quiz.locationName}.
          </p>
          <button className="quiz-start-card__submit" onClick={() => navigate("/")}>
            Tillbaka till kartan
          </button>
        </div>
      </div>
    );
  }

  // stage === STAGE.QUESTION
  const progressPercent = ((questionIndex + 1) / questions.length) * 100;

  return (
    <div className="quiz-page">
      <div className="quiz-question-card">
        <div className="quiz-progress">
          <span className="quiz-progress__label">
            Fråga {questionIndex + 1} av {questions.length}
          </span>
          <div className="quiz-progress__track">
            <div
              className="quiz-progress__fill"
              style={{ width: `${progressPercent}%` }}
            />
          </div>
        </div>

        <p className="quiz-question">{currentQuestion.text}</p>

        <div className="quiz-options">
          {currentQuestion.options.map((option) => (
            <button
              key={option.id}
              className={
                "quiz-option" +
                (selectedOptionId === option.id ? " quiz-option--selected" : "")
              }
              onClick={() => handleSelectOption(option.id)}
            >
              {option.text}
            </button>
          ))}
        </div>

        {selectedOptionId === null && (
          <p className="quiz-question-card__hint">Välj ett alternativ för att fortsätta.</p>
        )}

        <button
          className="quiz-start-card__submit"
          onClick={handleNext}
          disabled={selectedOptionId === null}
        >
          {questionIndex + 1 < questions.length ? "Nästa fråga" : "Avsluta quiz"}
        </button>
      </div>
    </div>
  );
}