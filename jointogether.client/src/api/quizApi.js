// quizApi.js — Quiz endpoint calls only.
import { getJson } from "./httpClient";

// GET /Quiz/location/{locationId} — returns LocationQuizDto
export function getQuizByLocationId(locationId) {
  return getJson(`/Quiz/location/${locationId}`);
}
