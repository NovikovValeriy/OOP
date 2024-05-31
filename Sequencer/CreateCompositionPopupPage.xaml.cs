using CommunityToolkit.Maui.Views;
using SequencerLibrary.Enumerators;

namespace Sequencer;

public partial class CreateCompositionPopupPage : Popup
{
	public CreateCompositionPopupPage()
	{
		InitializeComponent();
		List<Signatures> signatures = Enum.GetValues(typeof(Signatures)).Cast<Signatures>().ToList();
		SignaturesPicker.ItemsSource = signatures;
		SignaturesPicker.SelectedItem = signatures.FirstOrDefault();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
		try
		{
			byte bpm = Convert.ToByte(BpmEntry.Text);
            if (bpm < 1 && bpm > 250) IncorrectMessage.Text = "Incorrect input";
			else
			{
                var result = new CompositionCreateModel { Bpm = bpm, Signature = (Signatures)SignaturesPicker.SelectedItem };
                Close(result);
			}
        }
		catch
		{
            IncorrectMessage.Text = "Incorrect input";
        }
    }
}