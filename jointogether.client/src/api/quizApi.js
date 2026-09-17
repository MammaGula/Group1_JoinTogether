// quizApi.js — Quiz endpoint calls only.
import { getJson, postJson } from "./httpClient";

// GET /Quiz/location/{locationId} — returns LocationQuizDto
export function getQuizByLocationId(locationId) {
  return getJson(`/Quiz/location/${locationId}`);
}

// POST /Quiz/submit — return QuizResultDto
export function submitQuiz(locationId, answers) {
  return postJson(`/Quiz/submit`, { 
    locationId: Number(locationId), 
    answers 
  });
}
