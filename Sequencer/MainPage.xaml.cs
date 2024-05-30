using SequencerLibrary;
using SequencerLibrary.Enumerators;

namespace Sequencer
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        Color inactive = Colors.LightGray, active = Colors.LightGreen;
        public MainPage()
        {
            InitializeComponent();
            for (int i = 0; i < 128; i++)
            {
                NoteGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 25 });
                NoteGrid.RowDefinitions.Add(new RowDefinition { Height = 25 });
                NamesGrid.RowDefinitions.Add(new RowDefinition { Height = 25 });
            }
            for (int i = 0; i < 128; i++)
            {
                var element = new BoxView { Color = Colors.Gray, Margin = new Thickness(1) };
                var text = new Label { Text = ((NoteNames)(127 - i)).ToString(), FontSize = 10 };
                Grid.SetRow(element, i);
                Grid.SetColumn(element, 0);
                Grid.SetRow(text, i);
                Grid.SetColumn(text, 0);
                NamesGrid.Children.Add(element);
                NamesGrid.Children.Add(text);
                for (int j = 0; j < 128; j++)
                {
                    element = new BoxView { Color = inactive, Margin = new Thickness(1)};
                    var tap = new TapGestureRecognizer();
                    tap.Tapped += (s, e) =>
                    {
                        var box = (BoxView)s;
                        if (box.Color == inactive) box.Color = active;
                        else box.Color = inactive;
                    };
                    element.GestureRecognizers.Add(tap);
                    Grid.SetRow(element, i);
                    Grid.SetColumn(element, j);
                    NoteGrid.Children.Add(element);
                }
            }
        }

        private void GridScrolled(object sender, ScrolledEventArgs e)
        {
            ScrollNames.ScrollToAsync(ScrollGrid.ScrollX, ScrollGrid.ScrollY, false);
        }

        //private void OnCounterClicked(object sender, EventArgs e)
        //{
        //    count++;

        //    if (count == 1)
        //        CounterBtn.Text = $"Clicked {count} time";
        //    else
        //        CounterBtn.Text = $"Clicked {count} times";

        //    SemanticScreenReader.Announce(CounterBtn.Text);
        //}
    }

}
