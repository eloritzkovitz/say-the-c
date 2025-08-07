# Say the C

**Say the C** is a simple educational desktop application built with WPF (.NET) to help pupils learn the pronunciation of the letter **C** in various English words.

## Features

- **Random Word Generator**: Shows a random word from a database, containing the letter "C"
- **Speech Support**: Reads the selected word aloud using built-in speech synthesis
- **Visual Highlighting**: Colors the "C" sound based on soft (`Cyan`) or hard (`Red`) pronunciation
- **Instructions Popup**: Displays a guide on how to use the app
- **MVVM Architecture**: Clean separation of UI and logic using the MVVM pattern
- **SQLite Database**: Words and image paths are loaded from a local SQLite database
  
## How It Works

1. Press **Generate Word!** to get a random word with the letter **C**
2. The app visually highlights the pronunciation:
   - `Soft C` (before `e`, `i`, or `y`) → *Cyan*
   - `Hard C` → *Red*
3. Click the **sound icon** to hear the word spoken aloud

## Technology Stack

- WPF (.NET)
- MVVM
- SQLite (via `Microsoft.Data.Sqlite`)
- System.Speech.Synthesis

## How to Run

### Prerequisites
- Windows
- Visual Studio with .NET Desktop Development installed

### Running the App

1. Clone the repository

2. Open `SayTheC.sln` in Visual Studio

3. Build and run the project

## Project Structure

```
SayTheC/
├── App.xaml                  # Application definition
├── App.xaml.cs               # Application startup logic
├── AssemblyInfo.cs           # Assembly metadata
├── MainWindow.xaml           # Main WPF window UI
├── MainWindow.xaml.cs        # Main window code-behind (UI logic only)
├── SayTheC.csproj            # Project file
├── Converters/               # Value converters (e.g., BooleanToVisibilityConverter.cs)
├── Data/                     # Data access layer
├── Database/                 # SQLite database file
├── Icons/                    # App icons
├── Images/                   # Word images referenced by the database 
├── Models/                   # Data models
├── ViewModels/               # MVVM ViewModels
└── README.md                 # Project documentation
```
