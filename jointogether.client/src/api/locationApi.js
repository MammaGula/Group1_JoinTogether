// locationApi.js — Location endpoint calls only.
import { getJson } from "./httpClient";

// GET /Location — returns LocationSummaryDto[]
export function getLocations() {
  return getJson("/Location");
}

// GET /Location/{id} — returns LocationDetailDto
export function getLocationById(id) {
  return getJson(`/Location/${id}`);
}
