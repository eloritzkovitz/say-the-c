using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Speech.Synthesis;
using Models;
using Data;

namespace ViewModels
{
    /// <summary>
    /// The ViewModel for the main window, handling all UI logic and data binding.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        // Collection of all words loaded from the database
        public ObservableCollection<WordEntry> Words { get; set; } = new();

        // The currently selected word
        private WordEntry? _currentWord;
        public WordEntry? CurrentWord
        {
            get => _currentWord;
            set
            {
                if (_currentWord != value)
                {
                    _currentWord = value;
                    OnPropertyChanged();
                    UpdateWordDisplay();
                }
            }
        }

        // The colored text inlines for display
        private ObservableCollection<Inline> _wordInlines = new();
        public ObservableCollection<Inline> WordInlines
        {
            get => _wordInlines;
            set { _wordInlines = value; OnPropertyChanged(); }
        }

        // The image to display for the current word
        private ImageSource? _wordImage;
        public ImageSource? WordImage
        {
            get => _wordImage;
            set { _wordImage = value; OnPropertyChanged(); }
        }

        // Command for generating a new word
        public ICommand GenerateWordCommand { get; }
        // Command for playing the word sound
        public ICommand PlaySoundCommand { get; }
        // Command for showing help
        public ICommand ShowHelpCommand { get; }
        // Command for hiding help
        public ICommand HideHelpCommand { get; }

        private readonly Random r = new();
        private int[] map = Array.Empty<int>();
        private readonly SpeechSynthesizer synth = new();

        // For showing/hiding the help popup
        private bool _IsHelpVisible;
        public bool IsHelpVisible
        {
            get => _IsHelpVisible;
            set { _IsHelpVisible = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Constructor: loads words and sets up commands.
        /// </summary>
        public MainViewModel()
        {
            IsHelpVisible = true; // Show help popup on startup
            LoadWords();
            GenerateWordCommand = new RelayCommand(_ => GenerateWord(), _ => Words.Count > 0);
            PlaySoundCommand = new RelayCommand(_ => PlaySound(), _ => CurrentWord != null && !string.IsNullOrWhiteSpace(CurrentWord.Word));
            ShowHelpCommand = new RelayCommand(_ => IsHelpVisible = true);
            HideHelpCommand = new RelayCommand(_ => IsHelpVisible = false);
        }

        /// <summary>
        /// Loads words from the database using the repository.
        /// </summary>
        private void LoadWords()
        {
            try
            {
                var dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "words.db");
                var repo = new WordRepository(dbPath);
                var loadedWords = repo.GetAllWords();
                Words = new ObservableCollection<WordEntry>(loadedWords);
                map = new int[Words.Count];
                OnPropertyChanged(nameof(Words));
            }
            catch
            {
                Words = new ObservableCollection<WordEntry>();
                map = Array.Empty<int>();
                OnPropertyChanged(nameof(Words));
            }
        }

        /// <summary>
        /// Selects a new word and updates the display.
        /// </summary>
        public void GenerateWord()
        {
            if (Words.Count == 0) return;
            var selection = Words[r.Next(0, Words.Count)];
            if (CurrentWord == null || map.All(o => o == map[0]))
            {
                CurrentWord = selection;
            }
            else
            {
                int attempts = 0;
                const int maxAttempts = 100;
                while (map[Words.IndexOf(selection)] >= map[Words.IndexOf(CurrentWord)] && attempts < maxAttempts)
                {
                    selection = Words[r.Next(0, Words.Count)];
                    attempts++;
                }
                CurrentWord = selection;
            }
            map[Words.IndexOf(CurrentWord)]++;
        }

        /// <summary>
        /// Plays the sound for the current word.
        /// </summary>
        public void PlaySound()
        {
            if (CurrentWord != null && !string.IsNullOrWhiteSpace(CurrentWord.Word))
            {
                synth.Rate = -2;
                synth.SpeakAsync(CurrentWord.Word);
            }
        }

        /// <summary>
        /// Updates the colored text and image for the current word.
        /// </summary>
        private void UpdateWordDisplay()
        {
            WordInlines.Clear();

            if (CurrentWord == null || string.IsNullOrEmpty(CurrentWord.Word))
            {
                WordImage = null;
                OnPropertyChanged(nameof(WordInlines)); // Notify UI to clear inlines
                return;
            }

            var word = CurrentWord.Word;
            for (int i = 0; i < word.Length; i++)
            {
                char letter = word[i];
                Brush color = Brushes.Black;

                if (letter == 'c' || letter == 'C')
                {
                    // Choose cyan for soft C, red for hard C
                    if (i + 1 < word.Length &&
                        (word[i + 1] == 'e' || word[i + 1] == 'i' || word[i + 1] == 'y'))
                    {
                        color = Brushes.Cyan;
                    }
                    else
                    {
                        color = Brushes.Red;
                    }

                    WordInlines.Add(new Run(letter.ToString()) { Foreground = color });
                    if (i + 1 < word.Length)
                    {
                        WordInlines.Add(new Run(word[i + 1].ToString()) { Foreground = color });
                        i++;
                    }
                }
                else
                {
                    WordInlines.Add(new Run(letter.ToString()) { Foreground = Brushes.Black });
                }
            }

            // Notify UI to update inlines
            OnPropertyChanged(nameof(WordInlines));

            // Set the image for the current word
            try
            {
                if (!string.IsNullOrWhiteSpace(CurrentWord.ImageUrl))
                {
                    WordImage = new BitmapImage(new Uri(CurrentWord.ImageUrl, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    WordImage = null;
                }
            }
            catch
            {
                WordImage = null;
            }
        }

        /// <summary>
        /// Cleanup resources.
        /// </summary>
        public void Cleanup()
        {
            synth.Dispose();
        }

        // INotifyPropertyChanged implementation for data binding
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}