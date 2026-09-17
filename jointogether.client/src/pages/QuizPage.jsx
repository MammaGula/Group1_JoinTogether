import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { 
  getQuizByLocationId,
  submitQuiz
 } from "../api/quizApi";
import { ApiError } from "../api/httpClient";
import "./QuizPage.css";


const STAGE = {
  START: "start",
  QUESTION: "question",
  RESULT: "result"
};

export function QuizPage() {
  const { locationId } = useParams();
  const navigate = useNavigate();

  const [quiz, setQuiz] = useState(null);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  const [stage, setStage] = useState(STAGE.START);
  const [questionIndex, setQuestionIndex] = useState(0);

  // Stores the selected option for every question, keyed by question id,
  // e.g. { 1: 3, 2: 7 } — so answers survive moving between questions
  // and are all available at once when it's time to submit the quiz (US-09).
  const [answers, setAnswers] = useState({});
  const [result, setResult] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

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

  if (!quiz) return null;

  const questions = quiz.questions ?? [];
  const currentQuestion = questions[questionIndex];

  // The selected option for the question currently on screen, derived from
  // the answers map.
  const selectedOptionId = currentQuestion
    ? (answers[currentQuestion.id] ?? null)
    : null;

  const handleStart = () => {
    setQuestionIndex(0);
    setAnswers({});
    setResult(null);
    setStage(STAGE.QUESTION);
  };

  const handleSelectOption = (optionId) => {
    setAnswers((prev) => ({
      ...prev,
      [currentQuestion.id]: optionId,
    }));
  };

  const handleNext = async() => {
    if (selectedOptionId === null || isSubmitting) return; // require an answer before advancing

    if (questionIndex + 1 < questions.length) {
      setQuestionIndex((i) => i + 1);
      return;
    }

    // Last question answered, submit the quiz
    setIsSubmitting(true);
    setError(null);

    try{
      const formattedAnswers = Object.entries(answers).map(
        ([questionId, selectedOptionId]) => ({
          questionId: Number(questionId),
          selectedOptionId: Number(selectedOptionId)
        })
      );

      // answers contains pervious questions
      // so add the current (last) answer too
      formattedAnswers.push({
        questionId: currentQuestion.id,
        selectedOptionId: selectedOptionId
      });
      const quizResult = await submitQuiz(locationId, formattedAnswers);

      setResult(quizResult);
      setStage(STAGE.RESULT);

    } catch (err) {
      setError(
        err instanceof ApiError
          ? err.message
          : "Could not submit quiz"
      );
    } finally {
      setIsSubmitting(false);
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

  if (stage === STAGE.RESULT && result) {
    return (
      <div className="quiz-page">
        <div className="quiz-start-card">
          <h1>
            {result.passed ? "Bra jobbat!" : "Försök igen!"}
          </h1>
          <p className="quiz-start-card__score">
            {/* {result.scorePercentage.toFixed(0)}%  */}
            Du fick {typeof result?.scorePercent === "number"
              ? result.scorePercent.toFixed(0)
              : "—"}% rätt
          </p>

          {result.passed ? (
            <>
              <p className="quiz-start-card__description">
                Du klarade quizet! Du kan nu gå vidare.
              </p>

              <button
                type="button"
                onClick={() => {
                  // Later: navigate to activity creation
                  navigate("/");
                }}
              >
                Skapa aktivitet
              </button>

              <button
                type="button"
                onClick={() => navigate("/")}
              >
                Se aktiviteter
              </button>
            </>
          ) : (
            <>
              <p className="quiz-start-card__description">
                Du behöver minst 75% för att gå vidare.
              </p>

              <button
                type="button"
                onClick={handleStart}
              >
                Försök igen
              </button>
            </>
          )}

          <button
            type="button"
            onClick={() => navigate("/")}
          >
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
          <div
            className="quiz-progress__track"
            role="progressbar"
            aria-valuenow={questionIndex + 1}
            aria-valuemin={1}
            aria-valuemax={questions.length}
          >
            <div
              className="quiz-progress__fill"
              style={{ width: `${progressPercent}%` }}
            />
          </div>
        </div>

        <p className="quiz-question">{currentQuestion.text}</p>

        <div className="quiz-options" role="radiogroup">
          {currentQuestion.options.map((option) => (
            <button
              key={option.id}
              type="button"
              role="radio"
              aria-checked={selectedOptionId === option.id}
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
          disabled={selectedOptionId === null || isSubmitting}
        >
            {isSubmitting
              ? "Skickar..."
              : questionIndex + 1 < questions.length
                ? "Nästa fråga"
                : "Avsluta quiz"}
        </button>
      </div>
    </div>
  );
}