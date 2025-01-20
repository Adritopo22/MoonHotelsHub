# MoonHotelsHub

## 📌 Description
MoonHotelsHub is a microservice responsible for managing hotel availability searches across multiple providers, consolidating the responses into a single standard format.

## 🚀 Features
- 🔹 Receives requests in the HUB's standard format.
- 🔹 Translates the request to each provider's format.
- 🔹 Calls provider connectors in parallel.
- 🔹 Consolidates responses into a single result.
- 🔹 Error handling and logging with `ILogger`.

## 🏗️ Architecture
```plaintext
HubService → [IProviderConnector] → [HotelLegsConnector] → [IHotelLegsAPI] → HotelLegs API
```

## 🛠️ Configuration
- The list of providers is configured externally in `appsettings.json` or via environment variables.
- Custom exceptions (`ProviderException`) have been implemented to manage provider errors.

## 🧪 Unit Tests
- Tests have been developed using **xUnit and Moq**.
- The tests validate response aggregation, failure handling, and expected HUB behavior.
- To run the tests:
  ```sh
  dotnet test --logger "console;verbosity=detailed"
  ```

## 💡 Improvements and Considerations
- The HUB response does not include `currency` as it is assumed from the request. This was done to strictly follow the requirements, but i'd explicitly included it to avoid ambiguity and prevent confusions.

## 📜 License
This project is for learning and demonstration purposes only.
