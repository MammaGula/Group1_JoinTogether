// quizApi.js — Quiz endpoint calls only.
import { getJson, postJson } from "./httpClient";

// GET /Quiz/location/{locationId} — returns LocationQuizDto
export function getQuizByLocationId(locationId) {
  return getJson(`/Quiz/location/${locationId}`);
}

// GET /Quiz/location/{locationId}/status - returns user's quiz status for the location (e.g., completed, in progress, not started)
export function getQuizStatus(locationId) {
  return getJson(`/Quiz/location/${locationId}/status`);
}

// POST /Quiz/submit — return QuizResultDto
export function submitQuiz(locationId, answers) {
  return postJson(`/Quiz/submit`, { 
    locationId: Number(locationId), 
    answers 
  });
}


