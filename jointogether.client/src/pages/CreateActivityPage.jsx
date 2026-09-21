import { useEffect, useMemo, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { FormField } from "../components/FormField";
import { getLocations } from "../api/locationApi";
import { createActivity } from "../api/activityApi";
import { ApiError } from "../api/httpClient";
import "./CreateActivityPage.css";

const TITLE_MIN = 3;
const TITLE_MAX = 100;
const PARTICIPANTS_MIN = 1;
const PARTICIPANTS_MAX = 100;

const pad = (n) => String(n).padStart(2, "0");

// Today's date as YYYY-MM-DD in the user's local time (used for <input min> and validation)
function todayString() {
  const d = new Date();
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
}

function validate({ title, locationId, date, time, maxParticipants }) {
  const errors = {};
  const trimmedTitle = title.trim();

  if (!trimmedTitle) {
    errors.title = "Ange en titel.";
  } else if (trimmedTitle.length < TITLE_MIN) {
    errors.title = `Titeln måste vara minst ${TITLE_MIN} tecken.`;
  } else if (trimmedTitle.length > TITLE_MAX) {
    errors.title = `Titeln får vara högst ${TITLE_MAX} tecken.`;
  }

  if (!locationId) {
    errors.locationId = "Välj en plats.";
  }

  if (!date) {
    errors.date = "Välj ett datum.";
  } else if (date < todayString()) {
    errors.date = "Datumet kan inte ligga i det förflutna.";
  }

  if (!time) {
    errors.time = "Välj en tid.";
  } else if (date && !errors.date && new Date(`${date}T${time}`) <= new Date()) {
    errors.time = "Tiden måste ligga i framtiden.";
  }

  const count = Number(maxParticipants);
  if (maxParticipants === "") {
    errors.maxParticipants = "Ange max antal deltagare.";
  } else if (!Number.isInteger(count)) {
    errors.maxParticipants = "Ange ett heltal.";
  } else if (count < PARTICIPANTS_MIN || count > PARTICIPANTS_MAX) {
    errors.maxParticipants = `Ange ett tal mellan ${PARTICIPANTS_MIN} och ${PARTICIPANTS_MAX}.`;
  }

  return errors;
}

export function CreateActivityPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  const [locations, setLocations] = useState([]);
  const [locationsError, setLocationsError] = useState(null);

  const [values, setValues] = useState({
    title: "",
    // Optional: /activities/new?locationId=3 preselects a location
    locationId: searchParams.get("locationId") ?? "",
    date: "",
    time: "",
    maxParticipants: "10",
  });
  const [touched, setTouched] = useState({});
  const [submitError, setSubmitError] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    getLocations()
      .then(setLocations)
      .catch((err) => setLocationsError(err.message));
  }, []);

  const errors = useMemo(() => validate(values), [values]);

  // Only show an error once the user has left the field (or tried to submit)
  const showError = (name) => (touched[name] ? errors[name] : undefined);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setValues((prev) => ({ ...prev, [name]: value }));
  };

  const handleBlur = (e) => {
    const { name } = e.target;
    setTouched((prev) => ({ ...prev, [name]: true }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitError(null);

    // Reveal all errors on submit
    setTouched({
      title: true,
      locationId: true,
      date: true,
      time: true,
      maxParticipants: true,
    });
    if (Object.keys(errors).length > 0) return;

    setIsSubmitting(true);
    try {
      await createActivity({
        title: values.title.trim(),
        locationId: values.locationId,
        scheduledAt: `${values.date}T${values.time}:00`,
        maxParticipants: values.maxParticipants,
      });
      navigate("/");
    } catch (err) {
      // Includes the API's message, e.g. "You must pass this location's quiz..."
      setSubmitError(
        err instanceof ApiError ? err.message : "Kunde inte skapa aktiviteten.",
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  const locationError = showError("locationId") || locationsError;

  return (
    <div className="create-activity">
      <div className="create-activity__card">
        <h1>Skapa aktivitet</h1>
        <p className="create-activity__lead">
          Bjud in andra till en aktivitet på en plats du har klarat quizet för.
        </p>

        {submitError && <div className="create-activity__error">{submitError}</div>}

        <form onSubmit={handleSubmit} noValidate>
          <FormField
            id="title"
            name="title"
            label="Titel"
            type="text"
            placeholder="T.ex. Fika i parken"
            maxLength={TITLE_MAX}
            value={values.title}
            onChange={handleChange}
            onBlur={handleBlur}
            aria-invalid={!!showError("title")}
            error={showError("title")}
          />

          <div className="form-field">
            <label htmlFor="locationId" className="form-field__label">
              Plats
            </label>
            <select
              id="locationId"
              name="locationId"
              className="form-field__input"
              value={values.locationId}
              onChange={handleChange}
              onBlur={handleBlur}
              aria-invalid={!!showError("locationId")}
              disabled={locations.length === 0}
            >
              <option value="">
                {locationsError
                  ? "Kunde inte ladda platser"
                  : locations.length === 0
                    ? "Laddar platser..."
                    : "Välj en plats"}
              </option>
              {locations.map((l) => (
                <option key={l.id} value={l.id}>
                  {l.name}
                </option>
              ))}
            </select>
            {locationError && (
              <span className="form-field__error">{locationError}</span>
            )}
          </div>

          <div className="create-activity__row">
            <FormField
              id="date"
              name="date"
              label="Datum"
              type="date"
              min={todayString()}
              value={values.date}
              onChange={handleChange}
              onBlur={handleBlur}
              aria-invalid={!!showError("date")}
              error={showError("date")}
            />
            <FormField
              id="time"
              name="time"
              label="Tid"
              type="time"
              value={values.time}
              onChange={handleChange}
              onBlur={handleBlur}
              aria-invalid={!!showError("time")}
              error={showError("time")}
            />
          </div>

          <FormField
            id="maxParticipants"
            name="maxParticipants"
            label="Max antal deltagare"
            type="number"
            min={PARTICIPANTS_MIN}
            max={PARTICIPANTS_MAX}
            step={1}
            value={values.maxParticipants}
            onChange={handleChange}
            onBlur={handleBlur}
            aria-invalid={!!showError("maxParticipants")}
            error={showError("maxParticipants")}
          />

          <div className="create-activity__actions">
            <button
              type="button"
              className="create-activity__cancel"
              onClick={() => navigate("/")}
              disabled={isSubmitting}
            >
              Avbryt
            </button>
            <button
              type="submit"
              className="create-activity__submit"
              disabled={isSubmitting}
            >
              {isSubmitting ? "Skapar..." : "Skapa aktivitet"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}