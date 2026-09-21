// activityApi.js — Activity endpoint calls only.
import { getJson, postJson } from "./httpClient";

// POST /Activity — returns ActivityDto
export function createActivity({ title, scheduledAt, maxParticipants, locationId }) {
  return postJson("/Activity", {
    title,
    scheduledAt, // "YYYY-MM-DDTHH:mm:ss" (local time, no timezone)
    maxParticipants: Number(maxParticipants),
    locationId: Number(locationId),
  });
}

// GET /Activity/location/{locationId} — returns ActivityDto[]
export function getActivitiesByLocationId(locationId) {
  return getJson(`/Activity/location/${locationId}`);
}