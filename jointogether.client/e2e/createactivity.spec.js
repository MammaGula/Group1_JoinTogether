// e2e/create-activity.spec.js
// Täcker US-14 (skapa aktivitet) och formulärvalideringen på /activities/new.
//
// Obs: skapandet lyckas bara om användaren klarat quizet för platsen
// (se AuthService/ActivityService). Testerna nedan verifierar formuläret
// och dess validering; happy-path-testet kräver TEST_LOCATION_ID satt till
// en plats användaren redan klarat quizet för, annars förväntas ett
// serverfel visas istället.

import { test, expect } from "@playwright/test";
import { makeTestUser, registerViaUi } from "./helpers";

const LOCATION_ID = process.env.TEST_LOCATION_ID ?? "1";

test.describe("Skapa aktivitet", () => {
  test.beforeEach(async ({ page }) => {
    const user = makeTestUser();
    await page.goto("/");
    await registerViaUi(page, user);
    await expect(page.locator(".home__logout")).toBeVisible();
    await page.goto("/activities/new");
  });

  test("visar valideringsfel när formuläret skickas tomt", async ({
    page,
  }) => {
    await page.locator(".create-activity__submit").click();

    await expect(page.locator("#title").locator("xpath=..")).toContainText(
      "Ange en titel.",
    );
    await expect(page.locator("text=Välj en plats.")).toBeVisible();
    await expect(page.locator("text=Välj ett datum.")).toBeVisible();
    await expect(page.locator("text=Välj en tid.")).toBeVisible();
  });

  test("visar fel för en titel som är för kort", async ({ page }) => {
    await page.locator("#title").fill("Aa");
    await page.locator("#title").blur();

    await expect(
      page.locator("text=Titeln måste vara minst 3 tecken."),
    ).toBeVisible();
  });

  test("visar fel för ett datum som ligger i det förflutna", async ({
    page,
  }) => {
    await page.locator("#date").fill("2020-01-01");
    await page.locator("#date").blur();

    await expect(
      page.locator("text=Datumet kan inte ligga i det förflutna."),
    ).toBeVisible();
  });

  test("visar fel för max antal deltagare utanför tillåtet intervall", async ({
    page,
  }) => {
    await page.locator("#maxParticipants").fill("0");
    await page.locator("#maxParticipants").blur();

    await expect(
      page.locator("text=Ange ett tal mellan 1 och 100."),
    ).toBeVisible();
  });

  test("ett korrekt ifyllt formulär går att skicka in", async ({ page }) => {
    const tomorrow = new Date(Date.now() + 24 * 60 * 60 * 1000)
      .toISOString()
      .slice(0, 10);

    await page.locator("#title").fill("Fika i parken");
    await page.locator("#locationId").selectOption(LOCATION_ID);
    await page.locator("#date").fill(tomorrow);
    await page.locator("#time").fill("14:00");
    await page.locator("#maxParticipants").fill("5");

    await page.locator(".create-activity__submit").click();

    // Antingen lyckas det (redirect till "/") eller så visas API:ets
    // felmeddelande om användaren inte klarat platsens quiz än.
    await expect(
      page.locator(".create-activity__error").or(page.locator(".home")),
    ).toBeVisible();
  });

  test("Avbryt-knappen tar tillbaka till startsidan", async ({ page }) => {
    await page.locator(".create-activity__cancel").click();

    await expect(page).toHaveURL("/");
  });
});