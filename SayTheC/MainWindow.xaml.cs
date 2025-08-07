using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Speech.Synthesis;
using Microsoft.Data.Sqlite;
using Models;
using Data;

namespace SayTheC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<WordEntry> words = new List<WordEntry>();
        private int[] map = [];
        private WordEntry currentWord = new WordEntry { Word = "", ImageUrl = "" };
        private readonly Random r = new Random();
        SpeechSynthesizer synth = new SpeechSynthesizer();

        public MainWindow()
        {
            InitializeComponent();
            LoadWordsFromDatabase();
            SetGame();
        }

        // Load words from database
        private void LoadWordsFromDatabase()
        {
            try
            {
                var dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "words.db");
                var repo = new WordRepository(dbPath);
                words = repo.GetAllWords();
                map = new int[words.Count];

                // If no words are loaded, show error and disable controls
                if (words.Count == 0)
                {
                    MessageBox.Show("No words found in the database. Please check your database file.", "No Words", MessageBoxButton.OK, MessageBoxImage.Warning);
                    DisableControls();
                }
                else
                {
                    EnableControls();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load words from database:\n" + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                words = new List<WordEntry>();
                map = new int[0];
                DisableControls();
            }
        }

        // Set game to initial state
        private void SetGame()
        {
            infoPopup.Visibility = Visibility.Visible;
            Array.Fill(map, 0);
        }

        // Choose word from list
        public void ChooseWord()
        {
            var selection = words[r.Next(0, words.Count)];
            if (currentWord == null || map.All(o => o == map[0]))
            {
                currentWord = selection;
            }
            else
            {
                // Ensure the new word is not the same as the current word
                while (map[words.IndexOf(selection)] >= map[words.IndexOf(currentWord)])
                {
                    selection = words[r.Next(0, words.Count)];
                }
                currentWord = selection;
            }
            map[words.IndexOf(selection)]++;
        }

        // Apply visual changes
        public void UpdateWordDisplay()
        {
            txt.Inlines.Clear();
            var color = Brushes.Black;

            for (int i = 0; i < currentWord.Word.Length; i++)
            {
                char letter = currentWord.Word[i];
                if (letter == 'c' || letter == 'C')
                {
                    // Choose cyan for soft C, red for hard C
                    if (i + 1 < currentWord.Word.Length &&
                        (currentWord.Word[i + 1] == 'e' || currentWord.Word[i + 1] == 'i' || currentWord.Word[i + 1] == 'y'))
                    {
                        color = Brushes.Cyan;
                    }
                    else
                        color = Brushes.Red;

                    txt.Inlines.Add(new Run(letter.ToString()) { Foreground = color });
                    if (i + 1 < currentWord.Word.Length)
                    {
                        txt.Inlines.Add(new Run(currentWord.Word[i + 1].ToString()) { Foreground = color });
                        i++;
                    }
                }
                else
                    txt.Inlines.Add(new Run(letter.ToString()) { Foreground = Brushes.Black });
            }

            // Use ImageUrl
            img.Source = new BitmapImage(new Uri(currentWord.ImageUrl, UriKind.RelativeOrAbsolute));
        }

        // Disable controls
        private void DisableControls()
        {
            generateButton.IsEnabled = false;
            playButton.IsEnabled = false;
            helpButton.IsEnabled = false;
        }

        // Enable controls
        private void EnableControls()
        {
            generateButton.IsEnabled = true;
            playButton.IsEnabled = true;
            helpButton.IsEnabled = true;
        }

        // Generate word button
        private void GenerateWord(object sender, RoutedEventArgs e)
        {
            txt.Text = "";
            ChooseWord();
            UpdateWordDisplay();
        }

        // Play sound button
        private void PlaySound(object sender, RoutedEventArgs e)
        {
            if (currentWord != null && !string.IsNullOrWhiteSpace(currentWord.Word))
            {
                synth.Rate = -2;
                synth.SpeakAsync(currentWord.Word);
            }
            else
            {
                MessageBox.Show("Please generate a word first!", "No word", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }        

        // Click to show info popup
        private void ShowHelpPopup(object sender, RoutedEventArgs e)
        {
            infoPopup.Visibility = Visibility.Visible;
        }

        // Hide info popup on mouse down
        private void HideHelpPopup(object sender, MouseButtonEventArgs e)
        {
            infoPopup.Visibility = Visibility.Hidden;
        }

        // Handle window closing event to dispose of the synthesizer
        protected override void OnClosed(EventArgs e)
        {
            synth.Dispose();
            base.OnClosed(e);
        }        
    }
}