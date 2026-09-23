// playwright.config.js
// Kräver att JoinTogether.API körs separat på https://localhost:7211
// (dotnet run --launch-profile https), eftersom frontend anropar det
// direkt via httpClient.js och Playwright inte kan starta ett .NET-API.

import { defineConfig, devices } from "@playwright/test";

export default defineConfig({
  testDir: "./e2e",
  fullyParallel: false, // testerna delar samma användarflöde/databas
  retries: 0,
  reporter: "html",

  use: {
    baseURL: "http://localhost:5173",
    trace: "on-first-retry",
    screenshot: "only-on-failure",
  },

  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] },
    },
  ],

  // Startar `npm run dev` automatiskt om inget redan lyssnar på porten.
  webServer: {
    command: "npm run dev",
    url: "http://localhost:5173",
    reuseExistingServer: true,
    timeout: 30_000,
  },
});