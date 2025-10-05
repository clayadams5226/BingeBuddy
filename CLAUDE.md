# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

BingeBuddy is a .NET MAUI cross-platform mobile application for tracking and discovering TV shows. The app integrates with The Movie Database (TMDb) API to provide show information and uses SQLite for local data persistence.

## Build and Run Commands

### Build the solution
```bash
dotnet build BingeBuddy.sln
```

### Build for specific platforms
```bash
# Android
dotnet build BingeBuddy/BingeBuddy.Droid/BingeBuddy.Droid.csproj

# iOS
dotnet build BingeBuddy/BingeBuddy.iOS/BingeBuddy.iOS.csproj
```

### Run on Android
```bash
dotnet build BingeBuddy/BingeBuddy.Droid/BingeBuddy.Droid.csproj -t:Run
```

### Clean build artifacts
```bash
dotnet clean BingeBuddy.sln
```

## Architecture

### Solution Structure
The solution consists of three projects:
- **BingeBuddy** (BingeBuddy/BingeBuddy/): Shared .NET MAUI project containing all UI, ViewModels, Services, Models, and business logic
- **BingeBuddy.Droid** (BingeBuddy/BingeBuddy.Droid/): Android-specific head project
- **BingeBuddy.iOS** (BingeBuddy/BingeBuddy.iOS/): iOS-specific head project

### MVVM Pattern with CommunityToolkit
The app uses the MVVM pattern with CommunityToolkit.Mvvm:
- ViewModels inherit from `BaseViewModel` which extends `ObservableObject`
- Use `[ObservableProperty]` attributes for bindable properties
- Use `[RelayCommand]` attributes for command methods
- All ViewModels are registered as singletons in dependency injection

### Dependency Injection
Service registration is centralized in `MauiProgramExtensions.cs`:
- Services are registered as singletons: `DatabaseService`, `ITVShowApiService` (implemented by `TMDbApiService`)
- ViewModels are registered as singletons: `GlobalSearchViewModel`, `DiscoverViewModel`, `MyShowsViewModel`, `WatchlistViewModel`
- Views are registered as transient
- Constructor injection is used throughout the application

### Data Layer

#### SQLite Database
- Database file: `binge_buddy.db3` stored in app data directory
- Connection initialized lazily on first use in `DatabaseService`
- Two tables: `user_shows` and `watched_episodes`
- The `DatabaseService` handles all CRUD operations for user data

#### TMDb API Integration
- API service interface: `ITVShowApiService`
- Implementation: `TMDbApiService`
- API key stored in `Helpers/Constants.cs`
- Endpoints: search shows, get show details, get season episodes, trending shows, popular shows, similar shows
- Models use Newtonsoft.Json attributes for deserialization

### Navigation Structure
- Uses .NET MAUI Shell with a TabBar containing three tabs: Discover, My Shows, Watchlist
- Global search accessible via button in Shell.TitleView
- Search modal is pushed as a modal navigation page
- Detail pages can be navigated to from search results and lists

### Models
- **Show**: Represents TV show data from TMDb API with helper properties for image URLs
- **UserShow**: SQLite entity for shows added to user's library with tracking status
- **WatchedEpisode**: SQLite entity for tracking individual watched episodes
- **Season/Episode/Network/Genre**: Supporting models for show metadata

## Key Patterns

### ViewModel Lifecycle
- ViewModels persist as singletons, maintaining state across navigation
- Use `LoadTrendingShows()`, `LoadPopularShows()`, etc. commands to refresh data
- `IsRefreshing` and `IsBusy` properties control UI loading states

### Service Integration
When creating new pages or features:
1. Add ViewModel to `MauiProgramExtensions.cs` if needed
2. Add View to `MauiProgramExtensions.cs` as transient if needed
3. Inject required services (DatabaseService, ITVShowApiService) into ViewModel constructor
4. Inject ViewModel into View constructor
5. Use RelayCommand methods for user actions

### Database Operations
- Database initializes tables automatically on first access
- All database methods are async
- Use `await InitAsync()` pattern at start of each database method
- UserShow tracks: ShowId, Title, PosterUrl, Status (Watching/Completed/Dropped), LastWatchedSeason, LastWatchedEpisode

## Important Configuration

### TMDb API Key
Located in `BingeBuddy/BingeBuddy/Helpers/Constants.cs`:
- Constant: `TMDB_API_KEY`
- Used by `TMDbApiService` for all API calls

### Database Configuration
Located in `BingeBuddy/BingeBuddy/Helpers/Constants.cs`:
- Database filename: `binge_buddy.db3`
- SQLite flags: ReadWrite, Create, SharedCache
- Path computed dynamically using `FileSystem.AppDataDirectory`

### Supported Platforms
- Android: Minimum SDK 21 (Android 5.0)
- iOS: Minimum version 15.0
- Target framework: .NET 9.0

## Dependencies
- Microsoft.Maui.Controls
- CommunityToolkit.Mvvm (8.4.0)
- Newtonsoft.Json (13.0.4)
- sqlite-net-pcl (1.9.172)
- SQLitePCLRaw.bundle_green (2.1.11)
