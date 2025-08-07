using System.ComponentModel;
using System.Windows;
using ViewModels;

namespace SayTheC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            DataContext = viewModel;

            // Listen for WordInlines property changes
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            
            // Update the inlines
            UpdateWordInlines();
        }

        // Updates the WordTextBlock when the ViewModel's WordInlines property changes.
        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.WordInlines))
            {
                UpdateWordInlines();
            }
        }

        /// <summary>
        /// Updates the WordTextBlock with the current WordInlines from the ViewModel.
        /// </summary>
        private void UpdateWordInlines()
        {
            WordTextBlock.Inlines.Clear();
            foreach (var inline in viewModel.WordInlines)
                WordTextBlock.Inlines.Add(inline);
        }

        // On window closed, clean up the ViewModel.
        protected override void OnClosed(EventArgs e)
        {
            viewModel.Cleanup();
            base.OnClosed(e);
        }
    }
}