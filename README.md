# DAVE — Data Assurance + Verification Engine

[![Unit Tests](https://github.com/AndrewFreemantle/dave-wrap/actions/workflows/unit-tests-action.yml/badge.svg?branch=main)](https://github.com/AndrewFreemantle/dave-wrap/actions/workflows/unit-tests-action.yml)
[![Build, Test & Publish](https://github.com/AndrewFreemantle/dave-wrap/actions/workflows/publish-action.yml/badge.svg?branch=main)](https://github.com/AndrewFreemantle/dave-wrap/actions/workflows/publish-action.yml)

DAVE is a Windows desktop application for verifying [WRAP](https://www.wrap.ngo/) Data Capture Spreadsheet submissions. It reads submitted Excel workbooks and runs a set of automated checks (e.g. numeric comparisons, date range validation) to flag inconsistencies for review.

> The **Unit Tests** badge tracks a dedicated [`Unit Tests`](.github/workflows/tests.yml) workflow that runs the full xUnit test suite on every push and pull request to `main`. Each run publishes a detailed pass/fail report as a GitHub check (via [`dorny/test-reporter`](https://github.com/dorny/test-reporter)), viewable on the run's summary page and on pull requests, along with a downloadable `.trx` results artifact.

## Tech Stack

- [.NET 10](https://dotnet.microsoft.com/) / C#
- [Avalonia UI](https://avaloniaui.net/) (MVVM, via [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/))
- [ExcelDataReader](https://github.com/ExcelDataReader/ExcelDataReader) for reading `.xls`/`.xlsx` submissions
- [xUnit](https://xunit.net/) for unit tests

## Project Structure

| Project | Description |
|---|---|
| `DAVE.UI` | The Avalonia desktop application (views, view models, models, services) |
| `DAVE.UnitTests` | xUnit test suite covering the validation checks |

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Build

```bash
dotnet build DAVE.slnx
```

### Run

```bash
dotnet run --project DAVE.UI/DAVE.UI.csproj
```

### Test

```bash
dotnet test
```

## CI/CD

| Workflow | File | Trigger | Purpose |
|---|---|---|---|
| Unit Tests | [`tests.yml`](.github/workflows/tests.yml) | Push/PR to `main` | Runs the xUnit suite and publishes a detailed pass/fail report |
| Build & Publish | [`publish-action.yml`](.github/workflows/publish-action.yml) | Push/PR to `main` | Builds, packages, and (on push to `main`) releases the app to [GitHub Releases](https://github.com/AndrewFreemantle/dave-wrap/releases) as a self-contained Windows (`win-x64`) executable |
