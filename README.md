# Travel API - ASP.NET Core

This is a robust REST API built with ASP.NET Core (.NET 8) for managing travel destinations and tours. The project implements best practices for a production-ready backend service, including DTOs, an In-Memory database, filtering, pagination, rate limiting, CORS, and caching.

## 🚀 How to Build and Run Locally

1. Clone this repository to your local machine.
2. Open the solution file (`TravelApi.sln`) in **Visual Studio 2022**.
3. Ensure you have the .NET 8 SDK installed.
4. Press `F5` or click the green "Play" (https) button in Visual Studio to build and run the project.
5. Your browser will automatically open the **Swagger UI**, where you can test all the endpoints.

## 🔐 Setup User Secrets

This API integrates with an external weather service (OpenWeatherMap) and requires an API key. To avoid hardcoding sensitive data, we use User Secrets.

**Follow these steps to configure it:**
1. In Visual Studio, right-click on the `TravelApi` project in the Solution Explorer.
2. Select **"Manage User Secrets"**.
3. Paste the following JSON structure into the `secrets.json` file that opens:

```json
{
  "WeatherApiKey": "your-test-secret-key-123"
}
4.Save the file (Ctrl+S). The API is now ready to run!

⚡ Performance Measurement (Caching)
To optimize performance and reduce database calls, I have implemented Memory Caching on the GET /api/Tours endpoint. The cache is set to expire after 5 minutes and uses Cache Eviction (clears automatically) whenever a new tour is created via POST.

Here are the response times measured locally via Swagger:

Cache Miss: The first request (fetching data from the database and saving it to the cache) took approximately 33 ms.

Cache Hit: The second request (fetching data directly from the memory cache) took only about 25 ms.
