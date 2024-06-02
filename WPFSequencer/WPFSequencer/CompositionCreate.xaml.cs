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
    /// Логика взаимодействия для CompositionCreate.xaml
    /// </summary>
    public partial class CompositionCreate : Window
    {
        public CompositionCreate()
        {
            InitializeComponent();
            bpmSlider.Value = 120;
            bpmLabel.Content = Convert.ToByte(bpmSlider.Value).ToString();
            ResizeMode = ResizeMode.NoResize;
            List<Signatures> lst = Enum.GetValues(typeof(Signatures)).Cast<Signatures>().ToList();
            signatureList.Items.Clear();
            foreach (Signatures signature in lst)
            {
                signatureList.Items.Add(signature);
            }
            signatureList.SelectedItem = lst.FirstOrDefault();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public byte Bpm
        {
            get { return Convert.ToByte(bpmSlider.Value); }
        }

        public Signatures Signature
        {
            get { return (Signatures)signatureList.SelectedItem; }
        }

        private void bpmSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(bpmLabel != null) bpmLabel.Content = Convert.ToByte(bpmSlider.Value).ToString();
        }
    }
}
