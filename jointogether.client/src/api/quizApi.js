// quizApi.js — Quiz endpoint calls only.
import { getJson, postJson } from "./httpClient";

// GET /Quiz/location/{locationId} — returns LocationQuizDto
export function getQuizByLocationId(locationId) {
  return getJson(`/Quiz/location/${locationId}`);
}

// POST /Quiz/submit — body: SubmitQuizRequest
//   { locationId, answers: [{ questionId, selectedOptionId }] }
// Returns: QuizResultDto
//   { totalQuestions, correctAnswers, scorePercent, passed }
export function submitQuiz({ locationId, answers }) {
  return postJson("/Quiz/submit", { locationId, answers });
}
