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
    - **Custom Shell**: A natively resizable borderless window (via `WM_NCHITTEST` and `WS_THICKFRAME`) with a custom title bar and sidebar.
    - **Responsive UI**: All views **MUST** implement `OnResize` to handle layout adjustments. Use breakpoints (e.g., 1000px for Sidebar mini-mode) and fluid containers (`FlowLayoutPanel`) to ensure the UI looks polished at any window size.
    - **Skeleton Loaders**: Use `SkeletonCard` during data-intensive loads to improve perceived performance.
    - **Navigation**: Managed by `NavigationService` within a central `contentPanel`. Navigations trigger a `Navigated` event used by the shell for state sync (like the sidebar indicator).

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
- **Test**: `dotnet test Tests/Tests.csproj`
- **Migrations**: `dotnet ef migrations add <Name> --project WorkoutTracker.Data --startup-project RealView`

## Development Conventions

### Testing & TDD (MANDATORY)

- **Test-Driven Development**: This project follows a TDD approach. **NO** new business logic or controller methods should be implemented without corresponding unit tests.
- **Service Testing**: Use `Microsoft.EntityFrameworkCore.InMemory` for database interactions and `Moq` for dependencies (like `IAuthService` or `IValidator`).
- **Controller Testing**: Controllers should be tested by injecting the actual service implementation (using an in-memory DB) to ensure the integration between thin controller wrappers and service logic is correct.
- **Coverage**: Every public method in `WorkoutTracker.Service` and `Controller` projects must have at least one success case and relevant failure case (e.g., validation errors, unauthorized access) tests.

### Technical Insights for Future Agents

- **DbContext Configuration**: `WorkoutDbContext` must support a constructor taking `DbContextOptions<WorkoutDbContext>` to allow `InMemoryDatabase` injection in tests.
- **Mocking Void Methods**: When using Moq to mock `void` methods (like `IAuthService.IsAuthenticated`), do **NOT** use `.Returns()`. Simply `Setup(x => x.Method())` is sufficient for a success path.
- **Anonymous Types in Tests**: When a service returns `IEnumerable<dynamic>` with anonymous types, use **Reflection** in your assertions to access properties. The `dynamic` keyword can be unreliable in test projects due to assembly visibility issues.
- **Required Fields**: Always check the Entity definitions for `[Required]` or non-nullable properties. Missing these during test data setup will cause `DbUpdateException` (Required properties are missing).
- **Controller Naming**: Always verify the actual method names in Controllers before writing tests, as they may differ slightly from the Service layer (e.g., `GetWorkoutSetsAsync` vs `GetAllAsync`).

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
