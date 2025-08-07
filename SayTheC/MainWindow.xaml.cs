using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Speech.Synthesis;

namespace SayTheC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // List of words to choose from
        private readonly List<string> words = new List<string>
        {
            "cat", "coffee", "cake", "circle", "cylinder",
            "bicycle", "cinder", "cinema", "cinnamon",
            "cap", "cow", "clown"
        };

        private readonly int[] map = new int[12];
        private string currentWord = string.Empty;
        private readonly Random r = new Random();
        SpeechSynthesizer synth = new SpeechSynthesizer();

        public MainWindow()
        {
            InitializeComponent();
            SetGame();
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
            string selection = words[r.Next(0, words.Count)]; // Choose word                   
            // If first time or after a full cycle -> choose selection
            if (currentWord == null || map.All(o => o == map[0]))
            {
                currentWord = selection;
            }
            else
            {
                // Otherwise, while the new selection has a larger use count -> choose another word
                while (map[words.IndexOf(selection)] >= map[words.IndexOf(currentWord)])
                {
                    selection = words[r.Next(0, words.Count)];
                }
                currentWord = selection;
            }
            map[words.IndexOf(selection)]++;
        }

        // Apply visual changes
        public void ApplyVisuals()
        {
            var color = Brushes.Black; // Default color

            // Apply text colors
            for (int i = 0; i < currentWord.Length; i++)
            {
                char letter = currentWord[i];
                if (letter == 'c' || letter == 'C')
                {
                    if (currentWord[i + 1] == 'e' || currentWord[i + 1] == 'i' || currentWord[i + 1] == 'y')
                    {
                        color = Brushes.Cyan;
                    }
                    else
                        color = Brushes.Red;

                    txt.Inlines.Add(new Run(letter.ToString()) { Foreground = color });
                    txt.Inlines.Add(new Run(currentWord[i + 1].ToString()) { Foreground = color });
                    i++;
                }
                else
                    txt.Inlines.Add(new Run(letter.ToString()) { Foreground = Brushes.Black });
            }
            // Apply image source
            img.Source = new BitmapImage(new Uri(@"\Images\" + currentWord + ".png", UriKind.Relative));
        }

        // Generate word button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            txt.Text = "";
            ChooseWord();
            ApplyVisuals();
        }

        // Play sound button
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(currentWord))
            {
                synth.Rate = -2;
                synth.SpeakAsync(currentWord);
            }
            else
            {
                MessageBox.Show("Please generate a word first!", "No word", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Inst_Click(object sender, RoutedEventArgs e)
        {
            infoPopup.Visibility = Visibility.Visible;
        }

        private void Info_MouseDown(object sender, MouseButtonEventArgs e)
        {
            infoPopup.Visibility = Visibility.Hidden;
        }
    }
}