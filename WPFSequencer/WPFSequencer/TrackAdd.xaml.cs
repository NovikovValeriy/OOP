using SequencerLibrary.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFSequencer
{
    /// <summary>
    /// Логика взаимодействия для TrackAdd.xaml
    /// </summary>
    public partial class TrackAdd : Window
    {
        public TrackAdd(string cont1, string cont2)
        {
            InitializeComponent();
            Title = cont2;
            button.Content = cont1;
            ResizeMode = ResizeMode.NoResize;
            List<Instruments> lst = Enum.GetValues(typeof(Instruments)).Cast<Instruments>().ToList();
            instrumentList.Items.Clear();
            foreach (Instruments instrument in lst)
            {
                instrumentList.Items.Add(instrument);
            }
            instrumentList.SelectedItem = lst.FirstOrDefault();
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public Instruments Instrument
        {
            get { return (Instruments)instrumentList.SelectedItem; }
        }
    }
}
