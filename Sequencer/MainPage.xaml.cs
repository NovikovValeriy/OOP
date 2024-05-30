using SequencerLibrary;
using SequencerLibrary.Enumerators;

namespace Sequencer
{
    public partial class MainPage : ContentPage
    {
        int cellSize = 25;
        Color inactive = Colors.LightGray, active = Colors.LightGreen;
        public MainPage()
        {
            InitializeComponent();
            for (int i = 0; i < 128; i++)
            {
                NamesGrid.RowDefinitions.Add(new RowDefinition { Height = cellSize });
                var element = new BoxView { Color = Colors.Gray, Margin = new Thickness(1) };
                var text = new Label { Text = ((NoteNames)(127 - i)).ToString(), FontSize = 10 };
                NamesGrid.Add(element, 0, i);
                NamesGrid.Add(text, 0, i);
            }
            for (int i = 0; i < 5; i++)
            {
                Grid measure = new Grid();
                for (int k = 0; k < 16; k++)
                {
                    measure.ColumnDefinitions.Add(new ColumnDefinition { Width = cellSize });
                }
                for (int j = 0; j < 128; j++)
                {
                    measure.RowDefinitions.Add(new RowDefinition { Height = cellSize });
                    for (int k = 0; k < 16; k++)
                    {
                        var element = new BoxView { Color = inactive, Margin = new Thickness(1) };
                        var tap = new TapGestureRecognizer();
                        tap.Tapped += (s, e) =>
                        {
                            var box = (BoxView)s;
                            if (box.Color == inactive) box.Color = active;
                            else box.Color = inactive;
                        };
                        element.GestureRecognizers.Add(tap);
                        measure.Add(element, k, j);
                    }
                }
                MeasureStack.Add(measure);
                MeasureStack.Add(new BoxView { Color = Colors.Gray, VerticalOptions = LayoutOptions.Fill, WidthRequest = 1, Margin = new Thickness(1) });
            }
        }

        private void GridScrolled(object sender, ScrolledEventArgs e)
        {
            ScrollNames.ScrollToAsync(ScrollGrid.ScrollX, ScrollGrid.ScrollY, false);
        }
        
        private void NamesScrolled(object sender, ScrolledEventArgs e)
        {
            ScrollGrid.ScrollToAsync(ScrollNames.ScrollX, ScrollNames.ScrollY, false);
        }
    }

}
