// e2e/auth.spec.js
// Täcker US-01 (register) och US-02 (login).

import { test, expect } from "@playwright/test";
import { makeTestUser, registerViaUi, loginViaUi } from "./helpers";

test.describe("Registrering och inloggning", () => {
  test("en ny användare kan registrera sig och kommer till startsidan", async ({
    page,
  }) => {
    const user = makeTestUser();

    await page.goto("/");
    await registerViaUi(page, user);

    // Lyckad registrering loggar in användaren och lämnar AuthPage.
    await expect(page.locator(".home__logout")).toBeVisible();
    await expect(page).toHaveURL("/");
  });

  test("registrering med redan använd e-post visar ett felmeddelande", async ({
    page,
  }) => {
    const user = makeTestUser();

    // Skapa kontot en gång.
    await page.goto("/");
    await registerViaUi(page, user);
    await page.locator(".home__logout").click();

    // Försök registrera samma e-post igen.
    await page.goto("/");
    await registerViaUi(page, user);

    await expect(page.locator(".auth-card__form-error")).toBeVisible();
    // Vi ska fortfarande vara kvar på auth-sidan, inte inloggade.
    await expect(page.locator(".home__logout")).toHaveCount(0);
  });

  test("en registrerad användare kan logga in", async ({ page }) => {
    const user = makeTestUser();

    await page.goto("/");
    await registerViaUi(page, user);
    await page.locator(".home__logout").click();

    await page.goto("/");
    await loginViaUi(page, user);

    await expect(page.locator(".home__logout")).toBeVisible();
  });

  test("fel lösenord vid inloggning visar ett felmeddelande", async ({
    page,
  }) => {
    const user = makeTestUser();

    await page.goto("/");
    await registerViaUi(page, user);
    await page.locator(".home__logout").click();

    await page.goto("/");
    await loginViaUi(page, { ...user, password: "FelLosenord123!" });

    await expect(page.locator(".auth-card__form-error")).toBeVisible();
    await expect(page.locator(".home__logout")).toHaveCount(0);
  });

  test("utloggning skickar tillbaka till auth-sidan", async ({ page }) => {
    const user = makeTestUser();

    await page.goto("/");
    await registerViaUi(page, user);
    await expect(page.locator(".home__logout")).toBeVisible();

    await page.locator(".home__logout").click();

    await expect(page.locator(".auth-tabs")).toBeVisible();
  });
});