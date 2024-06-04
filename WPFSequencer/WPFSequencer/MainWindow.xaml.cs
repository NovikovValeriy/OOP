using SequencerLibrary.Entities;
using SequencerLibrary.Enumerators;
using System;
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
        bool isPlaying = false;
        Dictionary<byte, StackPanel> grids;
        Track selectedTrack;
        Brush emptyNoteColor = Brushes.LightGray;
        Brush singleNoteColor = Brushes.Blue;
        Brush startNoteColor = Brushes.Green;
        Brush endNoteColor = Brushes.Red;
        Brush middleNoteColor = Brushes.Yellow;
        public MainWindow()
        {
            grids = new Dictionary<byte, StackPanel>();
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
            ChangeMenuItems(false);
        }

        private System.Windows.Controls.Grid MakeMeasure(Signatures s, ushort index, Track track)
        {
            System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(cellSize) });
            for (int i = 1; i < 129; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(cellSize) });
                for (byte j = 0; j < (byte)s; j++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(cellSize) });
                    Label label = new Label() { Background = emptyNoteColor, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
                    label.MouseDown += (o, e) =>
                    {
                        if (isPlaying) return;
                        byte row = (byte)System.Windows.Controls.Grid.GetRow((Label)o);
                        row -= 1;
                        byte col = (byte)System.Windows.Controls.Grid.GetColumn((Label)o);
                        if ((bool)AddRadioButton.IsChecked!)
                        {
                            composition?.addNote(track, (NoteNames)(127 - row), index, col);
                        }
                        else if ((bool)DeleteRadioButton.IsChecked!)
                        {
                            composition?.deleteNote(track, (NoteNames)(127 - row), index, col);
                        }
                        else if ((bool)IncreaseRadioButton.IsChecked!)
                        {
                            composition?.increaseDuration(track, (NoteNames)(127 - row), index, col);
                        }
                        else
                        {
                            composition?.decreaseDuration(track, (NoteNames)(127 - row), index, col);
                        }
                        
                    };
                    System.Windows.Controls.Grid.SetRow(label, i);
                    System.Windows.Controls.Grid.SetColumn(label, j);
                    grid.Children.Add(label);
                }
            }
            
            Label label1 = new Label() { Background = emptyNoteColor, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1), Content = $"Measure {index + 1}" };
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

        private System.Windows.Controls.Grid MakePercussionMeasure(Signatures s, ushort index)
        {
            System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(cellSize) });
            for (int i = 1; i < 48; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(cellSize) });
                for (byte j = 0; j < (byte)s; j++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(cellSize) });
                    Label label = new Label() { Background = emptyNoteColor, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
                    label.MouseDown += (o, e) =>
                    {
                        if (isPlaying) return;
                        int row = Math.Abs(System.Windows.Controls.Grid.GetRow((Label)o) - 82);
                        byte col = (byte)System.Windows.Controls.Grid.GetColumn((Label)o);
                        if ((bool)AddRadioButton.IsChecked!)
                        {
                            composition?.addNote(composition!.PercussionTrack!, (PercussionNames)row, index, col);
                        }
                        else if ((bool)DeleteRadioButton.IsChecked!)
                        {
                            composition?.deleteNote(composition!.PercussionTrack!, (PercussionNames)row, index, col);
                        }
                        else if ((bool)IncreaseRadioButton.IsChecked!)
                        {
                            composition?.increaseDuration(composition!.PercussionTrack!, (PercussionNames)row, index, col);
                        }
                        else
                        {
                            composition?.decreaseDuration(composition!.PercussionTrack!, (PercussionNames)row, index, col);
                        }

                    };
                    System.Windows.Controls.Grid.SetRow(label, i);
                    System.Windows.Controls.Grid.SetColumn(label, j);
                    grid.Children.Add(label);
                }
            }

            Label label1 = new Label() { Background = emptyNoteColor, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1), Content = $"Measure {index + 1}" };
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
                MeasuresStack.Children.Clear();
                TrackStack.Children.Clear();
                NamesGrid.Children.Clear();
                NamesGrid.RowDefinitions.Clear();
                StackPanel panel = new StackPanel();
                for (byte i = 1; i < 17; i++)
                {
                    if (grids.TryGetValue(i, out panel)) grids[i].Children.Clear();
                }
                CreateComposition(compositionCreate.Signature, compositionCreate.Bpm);
                ChangeMenuItems(true);
                isPlaying = false;
                PlayButton.IsEnabled = true;
                PauseButton.IsEnabled = false;
                StopButton.IsEnabled = false;
                SignatureLabel.Content = composition.Signature;
                BpmLabel.Content = composition.Bpm;
            }
        }
        private void Load_Composition_Click(object sender, RoutedEventArgs e)
        {
            CreateComposition();
            composition?.loadFromBinary("C:\\Users\\dzmitry\\source\\repos\\4sem\\OOP\\TestingProject\\file.bin");
            ChangeMenuItems(true);
            isPlaying = false;
            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            StopButton.IsEnabled = false;
            SignatureLabel.Content = composition?.Signature;
            BpmLabel.Content = composition.Bpm;
            foreach (var item in composition.Tracks.Values)
            {
                CreateTrack((byte)item.Channel, item.InstrumentName, item.Volume, item.IsActive);
            }
            if (composition.PercussionTrack != null) CreatePercussionTrack(composition.PercussionTrack.Volume, composition.PercussionTrack.IsActive);
        }

        private StackPanel CreateUITrack(Instruments instrument, byte? channel, byte volume = 127, bool isAct = true)
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
                Value = volume,
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
                IsChecked = isAct
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

        private StackPanel CreateUIPercussionTrack(byte volume = 127, bool isAct = true)
        {
            StackPanel outerStackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Background = Brushes.LightGray,
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
                Value = volume,
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
                IsChecked = isAct
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

        private void CreateComposition(Signatures s = Signatures.FourZFour, byte bpm = 120)
        {
            composition?.midiDispose();
            composition = new Composition(s, bpm);
            composition.Playing += (o, e) =>
            {
                isPlaying = true;
                PlayButton.IsEnabled = false;
                PauseButton.IsEnabled = true;
                StopButton.IsEnabled = true;
            };
            composition.Paused += (o, e) =>
            {
                isPlaying = false;
                PlayButton.IsEnabled = true;
                PauseButton.IsEnabled = false;
                StopButton.IsEnabled = true;
            };
            composition.Stopped += (o, e) =>
            {
                isPlaying = false;
                PlayButton.IsEnabled = true;
                PauseButton.IsEnabled = false;
                StopButton.IsEnabled = false;
            };
        }
        private void CreateTrack(byte channel, Instruments instrument, byte volume = 127, bool isAct = true)
        {
            Track track = composition?.Tracks[(byte)channel!]!;
            track.AddedMeasuresEvent += (o, e) =>
            {
                if (grids[(byte)track.Channel].Children.Count == 0)
                {
                    TransferFromMeasureStackToGrids(track);
                }
                UpdateGrid(track);
                TransferFromGridsToMeasureStack(track);
            };
            track.TrackGrid.AddedNoteEvent += (o, e) =>
            {
                int row = Math.Abs((int)e.NoteName - 128);
                int col = e.StartCol;
                Label note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.StartMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == col);
                note.Background = singleNoteColor;
            };
            track.TrackGrid.DeletedNoteEvent += DeleteNoteHandler;
            track.TrackGrid.IncreasedNoteEvent += IncreaseNoteHandler;
            track.TrackGrid.DecreasedNoteEvent += DecreaseNoteHandler;
            if (channel != null && channel < 17)
            {
                var trackStack = CreateUITrack(instrument, channel, volume, isAct);
                TrackStack.Children.Add(trackStack);
                grids[(byte)channel] = new StackPanel();
                UpdateGrid(track);


                if (selectedTrack != null)
                    TransferFromMeasureStackToGrids(selectedTrack);
                selectedTrack = track;
                TransferFromGridsToMeasureStack(selectedTrack);
            }
        }
        private void CreatePercussionTrack(byte volume = 127, bool isAct = true)
        {
            Track track = composition.PercussionTrack!;
            track.AddedMeasuresEvent += (o, e) =>
            {
                if (grids[(byte)track.Channel].Children.Count == 0)
                {
                    TransferFromMeasureStackToGrids(track);
                }
                UpdatePercussionGrid();
                TransferFromGridsToMeasureStack(track);
            };
            track.TrackGrid.AddedNoteEvent += (o, e) =>
            {
                int row = Math.Abs((int)e.NoteName - 82);
                int col = e.StartCol;
                Label note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.StartMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == col);
                note.Background = singleNoteColor;
                if (track.TrackGrid.MeasureAmount > MeasuresStack.Children.Count)
                {
                    if (grids[(byte)track.Channel].Children.Count == 0)
                    {
                        TransferFromMeasureStackToGrids(track);
                    }
                    UpdatePercussionGrid();
                    TransferFromGridsToMeasureStack(track);
                }
            };
            track.TrackGrid.DeletedNoteEvent += DeletePercussionNoteHandler;
            track.TrackGrid.IncreasedNoteEvent += IncreasePercussionNoteHandler;
            track.TrackGrid.DecreasedNoteEvent += DecreasePercussionNoteHandler;
            var trackStack = CreateUIPercussionTrack(volume, isAct);
            TrackStack.Children.Add(trackStack);
            grids[10] = new StackPanel();
            UpdatePercussionGrid();


            if (selectedTrack != null)
                TransferFromMeasureStackToGrids(selectedTrack);
            selectedTrack = track;
            TransferFromGridsToMeasureStack(selectedTrack);
        }
        private void MenuAddTrackItem_Click(object sender, RoutedEventArgs e)
        {
            TrackAdd trackAdd = new TrackAdd();
            if(trackAdd.ShowDialog() == true)
            {
                byte? channel = composition?.addTrack(trackAdd.Instrument);
                CreateTrack((byte)channel, trackAdd.Instrument);
            }
        }
        private void MenuAddPercussionTrackItem_Click(object sender, RoutedEventArgs e)
        {
            if(!composition!.addPercussionTrack()) return;
            CreatePercussionTrack();
        }

        private void UpdateGrid(Track track)
        {
            for (int p = grids[(byte)track.Channel].Children.Count; p < track.TrackGrid.GridMeasures.Count; p++)
            {
                var item = track.TrackGrid.GridMeasures[p];
                var grid = MakeMeasure(composition.Signature, (ushort)p, track);
                grids[(byte)track.Channel].Children.Add(grid);
                for (byte i = 0; i < 128; i++)
                {
                    for (byte j = 0; j < (byte)composition.Signature; j++)
                    {
                        if (item.notes[i, j] != null)
                        {
                            Label note = (Label)grid
                                .Children
                                .Cast<UIElement>()
                                .First(e =>
                                System.Windows.Controls.Grid.GetRow(e) == Math.Abs(i - 128)
                                && System.Windows.Controls.Grid.GetColumn(e) == j);
                            var form = item.notes[i, j].Form;
                            Brush color = middleNoteColor;
                            if (form == NoteForm.FullNote)
                            {
                                color = singleNoteColor;
                            }
                            else if(form == NoteForm.StartNote)
                            {
                                color = startNoteColor;
                            }
                            else if(form == NoteForm.EndNote)
                            {
                                color = endNoteColor;
                            }
                            note.Background = color;
                        }
                    }
                }
            }
        }

        private void UpdatePercussionGrid()
        {
            Track track = composition.PercussionTrack;
            for (int p = grids[10].Children.Count; p < track.TrackGrid.GridMeasures.Count; p++)
            {
                var item = track.TrackGrid.GridMeasures[p];
                var grid = MakePercussionMeasure(composition.Signature, (ushort)p);
                grids[10].Children.Add(grid);
                for (byte i = 35; i < 82; i++)
                {
                    for (byte j = 0; j < (byte)composition.Signature; j++)
                    {
                        if (item.notes[i, j] != null)
                        {
                            Label note = (Label)grid
                                .Children
                                .Cast<UIElement>()
                                .First(e =>
                                System.Windows.Controls.Grid.GetRow(e) == Math.Abs(i - 82)
                                && System.Windows.Controls.Grid.GetColumn(e) == j);
                            var form = item.notes[i, j].Form;
                            Brush color = middleNoteColor;
                            if (form == NoteForm.FullNote)
                            {
                                color = singleNoteColor;
                            }
                            else if (form == NoteForm.StartNote)
                            {
                                color = startNoteColor;
                            }
                            else if (form == NoteForm.EndNote)
                            {
                                color = endNoteColor;
                            }
                            note.Background = color;
                        }
                    }
                }
            }
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

            foreach (StackPanel child in TrackStack.Children)
            {
                if(Convert.ToByte(child.Name.Remove(0, 5)) == track.Channel) child.Background = Brushes.LightGreen;
                else child.Background = emptyNoteColor;
            }
        }


        private void NamesScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            NamesScroll.ScrollToVerticalOffset(MeasuresScroll.VerticalOffset);
        }

        private void MeasuresScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            NamesScroll.ScrollToVerticalOffset(MeasuresScroll.VerticalOffset);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            composition?.play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            composition?.pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            composition?.stop();
        }
    
        private void IncreaseNoteHandler(object sender, NoteArgs e)
        {
            ushort startMeasure = e.StartMeasure;
            ushort endMeasure = e.EndMeasure;
            byte startNote = e.StartCol;
            byte endNote = e.EndCol;
            byte row = (byte)Math.Abs((int)e.NoteName - 128);
            Label note;

            if (startMeasure == endMeasure)
            {
                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.StartMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == endNote);
                note.Background = endNoteColor;

                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.StartMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == endNote - 1);
                note.Background = middleNoteColor;

                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.StartMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == startNote);
                note.Background = startNoteColor;
                return;
            }
            if (endNote == 0)
            {
                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.EndMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == endNote);
                note.Background = endNoteColor;

                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.EndMeasure - 1]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == (int)composition!.Signature - 1);
                note.Background = middleNoteColor;

                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.StartMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == startNote);
                note.Background = startNoteColor;
                return;
            }

            note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.EndMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == endNote);
            note.Background = endNoteColor;

            note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.EndMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == endNote - 1);
            note.Background = middleNoteColor;
        }
        private void DecreaseNoteHandler(object sender, NoteArgs e)
        {
            ushort startMeasure = e.StartMeasure;
            ushort endMeasure = e.EndMeasure;
            byte startNote = e.StartCol;
            byte endNote = e.EndCol;
            byte row = (byte)Math.Abs((int)e.NoteName - 128);
            Label note;
            if (endNote == (byte)composition.Signature - 1)
            {
                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.EndMeasure + 1]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == 0);
                note.Background = emptyNoteColor;

                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.EndMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == endNote);
                note.Background = endNoteColor;
            }
            else
            {
                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.EndMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == endNote + 1);
                note.Background = emptyNoteColor;

                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.EndMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == endNote);
                note.Background = endNoteColor;
            }
            note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.StartMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == startNote);
            if (startMeasure == endMeasure && startNote == endNote)
            {
                note.Background = singleNoteColor;
            }
            else note.Background = startNoteColor;
        }
        private void IncreasePercussionNoteHandler(object sender, NoteArgs e)
        {
            ushort startMeasure = e.StartMeasure;
            ushort endMeasure = e.EndMeasure;
            byte startNote = e.StartCol;
            byte endNote = e.EndCol;
            byte row = (byte)Math.Abs((int)e.NoteName - 82);
            Label note;

            if (startMeasure == endMeasure)
            {
                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.StartMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == endNote);
                note.Background = endNoteColor;

                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.StartMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == endNote - 1);
                note.Background = middleNoteColor;

                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.StartMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == startNote);
                note.Background = startNoteColor;
                return;
            }
            if (endNote == 0)
            {
                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.EndMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == endNote);
                note.Background = endNoteColor;

                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.EndMeasure - 1]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == (int)composition!.Signature - 1);
                note.Background = middleNoteColor;

                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.StartMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == startNote);
                note.Background = startNoteColor;
                return;
            }

            note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.EndMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == endNote);
            note.Background = endNoteColor;

            note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.EndMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == endNote - 1);
            note.Background = middleNoteColor;
        }
        private void DecreasePercussionNoteHandler(object sender, NoteArgs e)
        {
            ushort startMeasure = e.StartMeasure;
            ushort endMeasure = e.EndMeasure;
            byte startNote = e.StartCol;
            byte endNote = e.EndCol;
            byte row = (byte)Math.Abs((int)e.NoteName - 82);
            Label note;
            if (endNote == (byte)composition.Signature - 1)
            {
                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.EndMeasure + 1]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == 0);
                note.Background = emptyNoteColor;

                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.EndMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == endNote);
                note.Background = endNoteColor;
            }
            else
            {
                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.EndMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == endNote + 1);
                note.Background = emptyNoteColor;

                note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.EndMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == endNote);
                note.Background = endNoteColor;
            }
            note = (Label)(
                    (System.Windows.Controls.Grid)MeasuresStack.Children[e.StartMeasure]
                )
                .Children
                .Cast<UIElement>()
                .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == startNote);
            if (startMeasure == endMeasure && startNote == endNote)
            {
                note.Background = singleNoteColor;
            }
            else note.Background = startNoteColor;
        }
        private void DeleteNoteHandler(object sender, NoteArgs e)
        {
            ushort startMeasure = e.StartMeasure;
            ushort endMeasure = e.EndMeasure;
            byte startNote = e.StartCol;
            byte endNote = e.EndCol;
            byte row = (byte)Math.Abs((int)e.NoteName - 128);
            Label note;
            if (startMeasure == endMeasure)
            {
                for (byte n = startNote; n <= endNote; n++)
                {
                    note = (Label)(
                        (System.Windows.Controls.Grid)MeasuresStack.Children[startMeasure]
                    )
                    .Children
                    .Cast<UIElement>()
                    .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == n);
                    note.Background = emptyNoteColor;
                }
                return;
            }
            for (byte n = startNote; n < (byte)composition.Signature; n++)
            {
                note = (Label)(
                        (System.Windows.Controls.Grid)MeasuresStack.Children[startMeasure]
                    )
                    .Children
                    .Cast<UIElement>()
                    .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == n);
                note.Background = emptyNoteColor;
            }
            for (ushort m = (ushort)(startMeasure + 1); m < endMeasure; m++)
            {
                for (byte n = 0; n < (byte)composition.Signature; n++)
                {
                    note = (Label)(
                        (System.Windows.Controls.Grid)MeasuresStack.Children[m]
                    )
                    .Children
                    .Cast<UIElement>()
                    .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == n);
                    note.Background = emptyNoteColor;
                }
            }
            for (byte n = 0; n <= endNote; n++)
            {
                note = (Label)(
                        (System.Windows.Controls.Grid)MeasuresStack.Children[endMeasure]
                    )
                    .Children
                    .Cast<UIElement>()
                    .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == n);
                note.Background = emptyNoteColor;
            }
        }

        private void DeletePercussionNoteHandler(object sender, NoteArgs e)
        {
            ushort startMeasure = e.StartMeasure;
            ushort endMeasure = e.EndMeasure;
            byte startNote = e.StartCol;
            byte endNote = e.EndCol;
            byte row = (byte)Math.Abs((int)e.NoteName - 82);
            Label note;
            if (startMeasure == endMeasure)
            {
                for (byte n = startNote; n <= endNote; n++)
                {
                    note = (Label)(
                        (System.Windows.Controls.Grid)MeasuresStack.Children[startMeasure]
                    )
                    .Children
                    .Cast<UIElement>()
                    .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == n);
                    note.Background = emptyNoteColor;
                }
                return;
            }
            for (byte n = startNote; n < (byte)composition.Signature; n++)
            {
                note = (Label)(
                        (System.Windows.Controls.Grid)MeasuresStack.Children[startMeasure]
                    )
                    .Children
                    .Cast<UIElement>()
                    .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == n);
                note.Background = emptyNoteColor;
            }
            for (ushort m = (ushort)(startMeasure + 1); m < endMeasure; m++)
            {
                for (byte n = 0; n < (byte)composition.Signature; n++)
                {
                    note = (Label)(
                        (System.Windows.Controls.Grid)MeasuresStack.Children[m]
                    )
                    .Children
                    .Cast<UIElement>()
                    .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == n);
                    note.Background = emptyNoteColor;
                }
            }
            for (byte n = 0; n <= endNote; n++)
            {
                note = (Label)(
                        (System.Windows.Controls.Grid)MeasuresStack.Children[endMeasure]
                    )
                    .Children
                    .Cast<UIElement>()
                    .First(t => System.Windows.Controls.Grid.GetRow(t) == row && System.Windows.Controls.Grid.GetColumn(t) == n);
                note.Background = emptyNoteColor;
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