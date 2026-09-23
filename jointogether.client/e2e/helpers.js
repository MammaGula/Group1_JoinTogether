// e2e/helpers.js
// Delade hjälpfunktioner för testerna.

/**
 * Bygger en unik testanvändare, så att varje testkörning registrerar
 * ett konto som inte redan finns i databasen (Email är unikt i AuthService).
 */
export function makeTestUser() {
  const unique = Date.now();
  return {
    fullName: "Test Testsson",
    email: `playwright.${unique}@example.com`,
    password: "Test1234!",
  };
}

/**
 * Registrerar ett nytt konto via UI:t och loggar därmed in användaren
 * (registerUser returnerar en token som AuthContext sparar direkt).
 * Förutsätter att man står på AuthPage ("/").
 */
export async function registerViaUi(page, user) {
  await page.getByRole("tab", { name: "Skapa konto" }).click();
  await page.locator("#register-fullName").fill(user.fullName);
  await page.locator("#register-email").fill(user.email);
  await page.locator("#register-password").fill(user.password);
  await page.locator(".auth-card__submit").click();
}

/**
 * Loggar in ett befintligt konto via UI:t. Förutsätter att man står på
 * AuthPage ("/") och att inloggningsfliken är vald (den är default).
 */
export async function loginViaUi(page, user) {
  await page.locator("#login-email").fill(user.email);
  await page.locator("#login-password").fill(user.password);
  await page.locator(".auth-card__submit").click();
}