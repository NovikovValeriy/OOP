using SequencerLibrary;
using SequencerLibrary.Entities;
using SequencerLibrary.Enumerators;
using System.Diagnostics;

namespace Sequencer
{
    public partial class MainPage : ContentPage
    {
        int cellSize = 25;
        Color inactive = Colors.LightGray, active = Colors.LightGreen;
        private Composition? composition;
        private Dictionary<VerticalStackLayout, HorizontalStackLayout> tracks = new Dictionary<VerticalStackLayout, HorizontalStackLayout>();
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

            //var trackGrid = CreateGrid(5, Signatures.FourFour);
            //MeasureStack.Children.Clear();
            //MeasureStack.Children.Add(trackGrid);

            for (int i = 1; i < 3; i++)
            {
                var track = AddTrack((Instruments)i, (byte)i);
                tracks.Add(track, CreateGrid(5, Signatures.FourFour));
                TracksStack.Add(track);
                //ChangeGrid(track);
            }
        }

        private void ChangeGrid(VerticalStackLayout track)
        {
            MeasureStack.Children.Clear();
            MeasureStack.Children.Add(tracks[track]);
        }

        private VerticalStackLayout AddTrack(Instruments i, byte channel)
        {
            VerticalStackLayout track = new VerticalStackLayout();

            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                var track = (VerticalStackLayout)s;

                Debug.WriteLine($"tapped track");
                ChangeGrid(track);
            };
            track.GestureRecognizers.Add(tap);

            track.BackgroundColor = Colors.LightGray;
            track.Margin = new Thickness(0, 0, 0, 5);

            Label label = new Label();
            label.Text = $"Track {channel}";
            label.FontSize = 14;
            Label instrument = new Label();
            instrument.Text = i.ToString();
            instrument.FontSize = 12;

            HorizontalStackLayout buttons = new HorizontalStackLayout();
            Button b1 = new Button { Text = "b1", Margin = new Thickness(0, 0, 5, 0) };
            Button b2 = new Button { Text = "b2", Margin = new Thickness(0, 0, 5, 0) };
            Button b3 = new Button { Text = "b3", Margin = new Thickness(0, 0, 5, 0) };
            buttons.Add(b1);
            buttons.Add(b2);
            buttons.Add(b3);

            track.Add(label);
            track.Add(instrument);
            track.Add(buttons);
            return track;
        }
        private HorizontalStackLayout CreateGrid(ushort measureAmount, Signatures signature)
        {
            HorizontalStackLayout trackGrid = new HorizontalStackLayout();
            for (ushort i = 0; i < measureAmount; i++)
            {
                Microsoft.Maui.Controls.Grid measure = new Microsoft.Maui.Controls.Grid();
                for (byte k = 0; k < (byte)signature; k++)
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
                trackGrid.Add(measure);
                trackGrid.Add(new BoxView { Color = Colors.Gray, VerticalOptions = LayoutOptions.Fill, WidthRequest = 1, Margin = new Thickness(1) });
            }
            return trackGrid;
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
