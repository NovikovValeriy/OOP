using SequencerLibrary.Entities;
using SequencerLibrary.Enumerators;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFSequencer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Composition? composition;
        public MainWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
            ChangeMenuItems(false);
        }

        private void ChangeMenuItems(bool b)
        {
            BpmSlider.IsEnabled = b;
            MenuSaveItem.IsEnabled = b;
            MenuSaveAsNewItem.IsEnabled = b;
            MenuAddTrackItem.IsEnabled = b;
            MenuAddPercussionTrackItem.IsEnabled = b;
            MenuRemoveAllTracksItem.IsEnabled = b;
            PlayButton.IsEnabled = b;
            PauseButton.IsEnabled = b;
            StopButton.IsEnabled = b;
        }

        private void Create_New_Composition_Click(object sender, RoutedEventArgs e)
        {
            CompositionCreate compositionCreate = new CompositionCreate();

            if(compositionCreate.ShowDialog() == true)
            {
                composition?.midiDispose();
                composition = new Composition(compositionCreate.Signature, compositionCreate.Bpm);
                ChangeMenuItems(true);
                SignatureLabel.Content = composition.Signature;
                BpmLabel.Content = composition.Bpm;
            }

            
        }

        private StackPanel CreateUITrack(Instruments instrument, byte? channel)
        {
            StackPanel outerStackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Background = Brushes.LightGray,
                Margin = new Thickness(10,10,10,0),
                Name = $"Track{ channel.ToString()}"
            };

            StackPanel trackStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };

            Label trackLabel = new Label()
            {
                Content = "Channel:"
            };

            Label channelLabel = new Label()
            {
                Name = "ChannelLabel",
                Content = channel.ToString()
            };

            trackStackPanel.Children.Add(trackLabel);
            trackStackPanel.Children.Add(channelLabel);

            StackPanel instrumentStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };

            Label instrumentLabel = new Label()
            {
                Content = "Instrument:"
            };

            Label instrumentNameLabel = new Label()
            {
                Name = "InstrumentLabel",
                Content = instrument
            };

            instrumentStackPanel.Children.Add(instrumentLabel);
            instrumentStackPanel.Children.Add(instrumentNameLabel);

            StackPanel volumeStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };

            Label volumeLabel = new Label()
            {
                Content = "Volume:"
            };

            Label volumeValueLabel = new Label()
            {
                Name = "VolumeLabel",
                Content = "0",
                MinWidth = 30
            };

            Slider volumeSlider = new Slider()
            {
                Width = 100,
                Value = 127,
                Minimum = 0,
                Maximum = 127,
                VerticalAlignment = VerticalAlignment.Center
            };
            volumeSlider.ValueChanged += (object s, RoutedPropertyChangedEventArgs<double> e) =>
            {
                var value = Convert.ToByte(volumeSlider.Value);
                volumeValueLabel.Content = value.ToString();
                var track = composition?.Tracks[Convert.ToByte(outerStackPanel.Name.Remove(0, 5))];
                track.Volume = value;
            };
            volumeValueLabel.Content = Convert.ToByte(volumeSlider.Value).ToString();

            volumeStackPanel.Children.Add(volumeLabel);
            volumeStackPanel.Children.Add(volumeValueLabel);
            volumeStackPanel.Children.Add(volumeSlider);

            StackPanel activeStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };

            Label activeLabel = new Label()
            {
                Content = "Active:"
            };

            CheckBox activeCheckBox = new CheckBox()
            {
                VerticalAlignment = VerticalAlignment.Center,
                IsChecked = true
            };
            activeCheckBox.Checked += (object s, RoutedEventArgs e) =>
            {
                var track = composition?.Tracks[Convert.ToByte(outerStackPanel.Name.Remove(0, 5))];
                track.IsActive = true;
            };
            activeCheckBox.Unchecked += (object s, RoutedEventArgs e) =>
            {
                var track = composition?.Tracks[Convert.ToByte(outerStackPanel.Name.Remove(0, 5))];
                track.IsActive = false;
            };

            StackPanel buttonsStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };

            Button changeButton = new Button()
            {
                Name = "ChangeInstrumentButton",
                Content = "Change instrument",
                Margin = new Thickness(5, 0, 0, 5)
            };
            changeButton.Click += (object s, RoutedEventArgs e) =>
            {
                TrackAdd trackAdd = new TrackAdd();
                if(trackAdd.ShowDialog() == true)
                {
                    var track = composition?.Tracks[Convert.ToByte(outerStackPanel.Name.Remove(0, 5))];
                    track?.changeInstrument(trackAdd.Instrument);
                    instrumentNameLabel.Content = track?.InstrumentName;
                }
            };
            Button deleteButton = new Button()
            {
                Name = "DeleteTracksButton",
                Content = "Delete track",
                Margin = new Thickness(5, 0, 0, 5)
            };
            deleteButton.Click += (object s, RoutedEventArgs e) =>
            {
                composition?.removeTrack(Convert.ToByte(outerStackPanel.Name.Remove(0, 5)));
                TrackStack.Children.Remove(outerStackPanel);
            };
            buttonsStackPanel.Children.Add(changeButton);
            buttonsStackPanel.Children.Add(deleteButton);

            activeStackPanel.Children.Add(activeLabel);
            activeStackPanel.Children.Add(activeCheckBox);

            outerStackPanel.Children.Add(trackStackPanel);
            outerStackPanel.Children.Add(instrumentStackPanel);
            outerStackPanel.Children.Add(volumeStackPanel);
            outerStackPanel.Children.Add(activeStackPanel);
            outerStackPanel.Children.Add(buttonsStackPanel);

            return outerStackPanel;
        }

        private void ActiveCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MenuAddTrackItem_Click(object sender, RoutedEventArgs e)
        {
            TrackAdd trackAdd = new TrackAdd();
            if(trackAdd.ShowDialog() == true)
            {
                byte? channel = composition?.addTrack(trackAdd.Instrument);
                if (channel != null && channel < 17)
                {
                    var trackStack = CreateUITrack(trackAdd.Instrument, channel);
                    TrackStack.Children.Add(trackStack);
                }
            }
        }
    }
}


/*System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();
            for (int i = 0; i < 128; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < 500; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < 128; i++)
            {
                for (int j = 0; j < 500; j++)
                {
                    Button button = new Button();
                    System.Windows.Controls.Grid.SetRow(button, i);
                    System.Windows.Controls.Grid.SetColumn(button, j);
                    grid.Children.Add(button);
                }
            }
            BaseGrid.Children.Add(grid);*/