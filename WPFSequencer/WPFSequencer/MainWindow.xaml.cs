﻿using SequencerLibrary.Entities;
using SequencerLibrary.Enumerators;
using System.Text;
using System.Threading.Channels;
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
        int cellSize = 25;
        Dictionary<byte, StackPanel> grids;
        Track selectedTrack;
        public MainWindow()
        {
            grids = new Dictionary<byte, StackPanel>();
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
            ChangeMenuItems(false);

            //DrawNoteNames();
        }

        private System.Windows.Controls.Grid MakeMeasure(Signatures s, byte bound)
        {
            System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(cellSize) });
            for (int i = 1; i < bound; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(cellSize) });
                for (int j = 0; j < (byte)s; j++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(cellSize) });
                    Label label = new Label() { Background = Brushes.LightGray, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
                    label.MouseDown += (o, e) =>
                    {
                        label.Background = Brushes.LightGreen;
                    };
                    System.Windows.Controls.Grid.SetRow(label, i);
                    System.Windows.Controls.Grid.SetColumn(label, j);
                    grid.Children.Add(label);
                }
            }
            
            Label label1 = new Label() { Background = Brushes.LightGray, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1), Content = "Measure" };
            System.Windows.Controls.Grid.SetRow(label1, 0);
            System.Windows.Controls.Grid.SetColumn(label1, 0);
            System.Windows.Controls.Grid.SetColumnSpan(label1, (byte)s);
            grid.Children.Add(label1);

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1) });
            Label label2 = new Label() { Background = Brushes.Black };
            System.Windows.Controls.Grid.SetColumn(label2, (byte)s);
            System.Windows.Controls.Grid.SetRow(label2, 0);
            System.Windows.Controls.Grid.SetRowSpan(label2, 129);  
            grid.Children.Add(label2);

            grid.Width = (byte)s * cellSize + 1;
            return grid;
        }
        private void DrawNoteNames()
        {
            MeasuresScroll.Width = 775;
            NamesGrid.RowDefinitions.Clear();
            NamesGrid.ColumnDefinitions.Clear();
            List<NoteNames> lst = Enum.GetValues(typeof(NoteNames)).Cast<NoteNames>().ToList();
            NamesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(cellSize + 15) });
            for (int i = 1; i < 129; i++)
            {
                NamesGrid.RowDefinitions.Add(new RowDefinition() { Height= new GridLength(cellSize)});
                Label label = new Label() { Background = Brushes.Gray, Content = lst[128 - i].ToString(), BorderBrush = Brushes.DarkGray, BorderThickness = new Thickness(1) };
                System.Windows.Controls.Grid.SetRow(label, i);
                NamesGrid.Children.Add(label);
            }
            NamesGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(cellSize) });
            NamesGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(cellSize) });
        }
        private void DrawPercussionNames()
        {
            MeasuresScroll.Width = 705;
            NamesGrid.RowDefinitions.Clear();
            NamesGrid.ColumnDefinitions.Clear();
            List<PercussionNames> lst = Enum.GetValues(typeof(PercussionNames)).Cast<PercussionNames>().ToList();
            NamesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(110) });
            for (int i = 1; i < 48; i++)
            {
                NamesGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(cellSize) });
                Label label = new Label() { Background = Brushes.Gray, Content = lst[47 - i].ToString(), BorderBrush = Brushes.DarkGray, BorderThickness = new Thickness(1) };
                System.Windows.Controls.Grid.SetRow(label, i);
                NamesGrid.Children.Add(label);
            }
            NamesGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(cellSize) });
            NamesGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(cellSize) });
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
            outerStackPanel.MouseDown += OuterStackPanel_MouseDown;

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
                var track = composition?.Tracks[(byte)channel!];
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
                var track = composition?.Tracks[(byte)channel!];
                track.IsActive = true;
            };
            activeCheckBox.Unchecked += (object s, RoutedEventArgs e) =>
            {
                var track = composition?.Tracks[(byte)channel!];
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
                    var track = composition?.Tracks[(byte)channel!];
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
                //byte channel = Convert.ToByte(outerStackPanel.Name.Remove(0, 5));
                composition?.removeTrack((byte)channel!);
                TrackStack.Children.Remove(outerStackPanel);
                grids[(byte)channel!] = null;
                if(TrackStack.Children.Count == 0)
                {
                    MeasuresStack.Children.Clear();
                    selectedTrack = null;
                    return;
                }

                var name = ((StackPanel)TrackStack.Children[0]).Name;
                var ch = Convert.ToByte(name.Remove(0, 5));
                if (ch == 10) selectedTrack = composition!.PercussionTrack!;
                else selectedTrack = composition!.Tracks[ch];
                TransferFromGridsToMeasureStack(selectedTrack);
                return;


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

        private StackPanel CreateUIPercussionTrack()
        {
            StackPanel outerStackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Background = Brushes.LightGreen,
                Margin = new Thickness(10, 10, 10, 0),
                Name = $"Track10"
            };
            outerStackPanel.MouseDown += OuterStackPanel_MouseDown;

            StackPanel trackStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };

            Label trackLabel = new Label()
            {
                Content = "Channel"
            };

            Label channelLabel = new Label()
            {
                Name = "ChannelLabel",
                Content = "10"
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
                Content = "Percussion"
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
                var track = composition?.PercussionTrack;
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
                var track = composition?.PercussionTrack;
                track.IsActive = true;
            };
            activeCheckBox.Unchecked += (object s, RoutedEventArgs e) =>
            {
                var track = composition?.PercussionTrack;
                track.IsActive = false;
            };

            StackPanel buttonsStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            Button deleteButton = new Button()
            {
                Name = "DeleteTracksButton",
                Content = "Delete track",
                Margin = new Thickness(5, 0, 0, 5)
            };
            deleteButton.Click += (object s, RoutedEventArgs e) =>
            {
                //byte channel = Convert.ToByte(outerStackPanel.Name.Remove(0, 5));
                composition?.removePercussionTrack();
                TrackStack.Children.Remove(outerStackPanel);
                grids[10] = null;
                if (TrackStack.Children.Count == 0)
                {
                    MeasuresStack.Children.Clear();
                    selectedTrack = null;
                    return;
                }

                var name = ((StackPanel)TrackStack.Children[0]).Name;
                var ch = Convert.ToByte(name.Remove(0, 5));
                selectedTrack = composition!.Tracks[ch];
                TransferFromGridsToMeasureStack(selectedTrack);
                return;
            };
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

        private void OuterStackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var channel = Convert.ToByte(((StackPanel)sender).Name.Remove(0, 5));
            Track track;
            if (channel == 10)
            {
                track = composition!.PercussionTrack!;
            }
            else track = composition!.Tracks[channel];
            if(track == selectedTrack) return;

            TransferFromMeasureStackToGrids(selectedTrack);
            TransferFromGridsToMeasureStack(track);

            selectedTrack = track;
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
                Track track = composition?.Tracks[(byte)channel!]!;
                if (channel != null && channel < 17)
                {
                    var trackStack = CreateUITrack(trackAdd.Instrument, channel);
                    TrackStack.Children.Add(trackStack);
                    grids[(byte)channel] = new StackPanel();
                    for (int i = 0; i < 5; i++)
                    {
                        grids[(byte)channel].Children.Add(MakeMeasure(composition.Signature, 129));
                    }
                    

                    if (selectedTrack != null)
                        TransferFromMeasureStackToGrids(selectedTrack);
                    selectedTrack = track;
                    TransferFromGridsToMeasureStack(selectedTrack);
                }
            }
        }
        private void MenuAddPercussionTrackItem_Click(object sender, RoutedEventArgs e)
        {
            if(!composition!.addPercussionTrack()) return;
            Track track = composition.PercussionTrack!;
            var trackStack = CreateUIPercussionTrack();
            TrackStack.Children.Add(trackStack);
            grids[10] = new StackPanel();
            for (int i = 0; i < 5; i++)
            {
                grids[10].Children.Add(MakeMeasure(composition.Signature, 48));
            }


            if (selectedTrack != null)
                TransferFromMeasureStackToGrids(selectedTrack);
            selectedTrack = track;
            TransferFromGridsToMeasureStack(selectedTrack);
        }

        private void TransferFromMeasureStackToGrids(Track track)
        {
            grids[(byte)track.Channel].Children.Clear();
            int amount = MeasuresStack.Children.Count;
            for (int i = 0; i < amount; i++)
            {
                var item = MeasuresStack.Children[0];
                MeasuresStack.Children.Remove(item);
                grids[(byte)track.Channel].Children.Add(item);
            }
        }
        private void TransferFromGridsToMeasureStack(Track track)
        {
            MeasuresStack.Children.Clear();
            int amount = grids[(byte)track.Channel].Children.Count;
            for (int i = 0; i < amount; i++)
            {
                var item = grids[(byte)track.Channel].Children[0];
                grids[(byte)track.Channel].Children.Remove(item);
                MeasuresStack.Children.Add(item);
            }
            if (track.Channel == 10) DrawPercussionNames();
            else DrawNoteNames();
        }


        private void NamesScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            NamesScroll.ScrollToVerticalOffset(MeasuresScroll.VerticalOffset);
        }

        private void MeasuresScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            NamesScroll.ScrollToVerticalOffset(MeasuresScroll.VerticalOffset);
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