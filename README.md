# DAVE — Data Assurance + Verification Engine

[![Unit Tests](https://github.com/AndrewFreemantle/dave-wrap/actions/workflows/unit-tests-action.yml/badge.svg?branch=main)](https://github.com/AndrewFreemantle/dave-wrap/actions/workflows/unit-tests-action.yml)
[![Build, Test & Publish](https://github.com/AndrewFreemantle/dave-wrap/actions/workflows/publish-action.yml/badge.svg?branch=main)](https://github.com/AndrewFreemantle/dave-wrap/actions/workflows/publish-action.yml)

DAVE is a Windows desktop application for verifying Data Capture Spreadsheet submissions. It reads submitted Excel workbooks and runs a set of automated checks (e.g. numeric comparisons, date range validation) to flag inconsistencies for review.

> The **Unit Tests** badge tracks a dedicated [`Unit Tests`](.github/workflows/unit-tests-action.yml) workflow that runs the full xUnit test suite on every push and pull request to `main`. Each run publishes a detailed pass/fail report as a GitHub check (via [`dorny/test-reporter`](https://github.com/dorny/test-reporter)), viewable on the run's summary page and on pull requests, along with a downloadable `.trx` results artifact.

## Getting Started
DAVE is distributed as a self-contained Windows executable — there are no pre-requeisites to install.

1. Download the latest `DAVE-Release-*-win-x64.zip` from [Releases](https://github.com/AndrewFreemantle/dave-wrap/releases/latest)
2. Unzip it
3. Run `DAVE.exe`


## Development & Contributions

### Tech Stack
- [.NET 10](https://dotnet.microsoft.com/) / C#
- [Avalonia UI](https://avaloniaui.net/) (MVVM, via [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/))
- [ExcelDataReader](https://github.com/ExcelDataReader/ExcelDataReader) for reading `.xls`/`.xlsx` submissions
- [xUnit](https://xunit.net/) for unit tests

### Project Structure

| Project | Description |
|---|---|
| `DAVE.Core` | The validation checks, `CheckSet` (the checks DAVE runs, in order) and the Data Capture Spreadsheet reader |
| `DAVE.UI` | The Avalonia desktop application (views, view models, services) |
| `DAVE.Cli` | `dave-cli`, which runs the same checks headless and prints the results as JSON |
| `DAVE.UnitTests` | xUnit test suite covering the validation checks, the check order and the CLI report |

### Build
Building from source requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet build DAVE.slnx
```

### Test

```bash
dotnet test
```

### Run

```bash
dotnet run --project DAVE.UI/DAVE.UI.csproj
```

### Command line

`dave-cli` runs the same 45 checks as the desktop app, against a current submission and optionally the previous year's, and prints one JSON report to stdout (diagnostics go to stderr):

```bash
dotnet run --project DAVE.Cli/DAVE.Cli.csproj -- check current.xlsx --previous previous.xlsx --json
```

Each entry in `checks` carries the check's registration `index`, the `number` and `name` the results grid shows, whether it `passed`, the query `message` the email would include (empty when passed), and the grid's `current` and previous-year (`expected`) columns. Exit codes: `0` the checks ran, whatever the results; `2` a file could not be opened or is not a Data Capture Sheet; `1` anything else.

## CI/CD
Continuous Integration / Continuous Development. Changes to the `main` branch are automatically built, tested and [released](https://github.com/AndrewFreemantle/dave-wrap/releases/latest).

| Workflow | File | Trigger | Purpose |
|---|---|---|---|
| Unit Tests | [`unit-tests-action.yml`](.github/workflows/unit-tests-action.yml) | Push/PR to `main` | Runs the xUnit suite and publishes a detailed pass/fail report |
| Build, Test & Publish | [`publish-action.yml`](.github/workflows/publish-action.yml) | Push/PR to `main` | Builds and tests the app, then (on push to `main`) releases it to [GitHub Releases](https://github.com/AndrewFreemantle/dave-wrap/releases) as a self-contained Windows (`win-x64`) executable, alongside self-contained `dave-cli` executables for `win-x64`, `osx-arm64` and `linux-x64` |
