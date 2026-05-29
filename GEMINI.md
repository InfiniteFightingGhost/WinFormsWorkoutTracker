# WorkoutTracker Project Instructions

This project is a Workout Tracker application built using C# and Windows Forms on .NET 8, featuring a modernized custom-drawn user interface.

## Project Overview

The application allows users to manage muscle groups, exercises, workouts, and track their workout sessions with a focus on a fluid, modern user experience.

### Architecture

The solution follows a multi-tier architecture:

- **`WorkoutTracker.Data`**: The Data Access Layer. Contains Entity Framework Core models (Entities), Data Transfer Objects (DTOs), and Migrations.
- **`WorkoutTracker.Service`**: The Business Logic Layer. Contains service interfaces and implementations where all business logic, data validation (FluentValidation), and complex data processing reside.
- **`Controller`**: The API/Controller Layer. Thin wrappers that delegate requests to the Service layer. Controllers should not contain business logic or direct DB context usage.
- **`RealView`**: The primary Presentation Layer. 
    - **Custom Shell**: A borderless window with a custom title bar and sidebar.
    - **Navigation**: Managed by `NavigationService` within a central `contentPanel`.
    - **State**: `WorkoutStateService` manages the active workout session across views.

### Key Technologies & Services

- **UI**: WinForms (.NET 8) with custom GDI+ drawing for rounded corners and micro-animations.
- **Validation**: **FluentValidation** is used for all DTO and Entity validation within the Service layer.
- **Theme Engine**: `UIStyle.cs` centralizes all colors, fonts, and metrics.
- **Notifications**: `ToastService` provides non-blocking feedback (replaces most `MessageBox` calls).
- **Imaging**: `PhotoService` handles saving/loading workout and profile images in AppData.

## Building and Running

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB)
- Entity Framework Core Tools

### Mandatory Commands

- **Build**: `dotnet build --nologo -v q --property WarningLevel=0 /clp:ErrorsOnly`
- **Run**: `dotnet run --project RealView`
- **Migrations**: `dotnet ef migrations add <Name> --project WorkoutTracker.Data --startup-project RealView`

## Development Conventions

### Visual Standards (UIStyle)

- **Colors**: Use `UIStyle.Primary` for actions and `UIStyle.Background` for view containers.
- **Buttons**: Use `ModernButton` instead of standard `Button` for automatic hover animations and rounded corners.
- **Cards**: All views should use the `DrawCard` helper from `BaseView` in their `OnPaint` or custom panel logic.
- **Icons**: Use Unicode symbols (✨, 📷, ×, ◀, ▶) instead of image assets where possible for scalability and ease of rendering.

### Navigation & Caching

- **Static Views**: (e.g., Dashboard) are cached by `NavigationService` for speed.
- **Dynamic Views**: (e.g., `FinishWorkoutView`, `WorkoutSummaryView`) that take constructor arguments **MUST NOT** be cached. `NavigationService` automatically skips caching if `args.Length > 0`.
- **Lifecycle**: Perform data fetching in `OnNavigatedTo` instead of the constructor to avoid race conditions with UI instantiation.

### Data Consistency

- **UI/Model Sync**: When adding data (like ExerciseSets or Exercises) to an active workout, you **MUST** update the in-memory `AppRuntime.WorkoutState.ActiveSession` collection immediately after the database save to prevent duplication on view refresh.

## Engineering Mandates (What NOT to do)

- **DO NOT** use `async void` for UI initialization. Use synchronous `InitializeComponent` and move async work to `OnNavigatedTo`.
- **DO NOT** add business logic or direct `WorkoutDbContext` access to Controllers. Always use the Service layer.
- **DO NOT** perform manual data validation in Services. Use **FluentValidation** validators.
- **DO NOT** use standard `MessageBox.Show` for success feedback; use `AppRuntime.Toasts.Show`. Reserved `MessageBox` for critical confirmations (e.g., "Delete workout?").
- **DO NOT** hardcode colors or fonts. Always reference `UIStyle`.
- **DO NOT** forget to explicitly qualify `System.Windows.Forms.Timer` to avoid ambiguity with `System.Threading.Timer`.
- **DO NOT** leave file locks on DLLs. If building fails with "file in use," ensure `RealView.exe` is closed.

## Complexity Management

- **View Swapping**: When a view becomes too complex, break it into reusable `UserControls` (like `ExerciseCard` or `SetRow`).
- **Focus Flow**: Use `KeyDown` events with `Keys.Enter` to orchestrate focus between inputs (Weight -> Reps -> Next Set) to maintain a high-quality "pro" feel for data entry.
