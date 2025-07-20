namespace Profitocracy.Mobile.Views.Shared.Controls;

public partial class AddNewButton : ContentView
{
    public event EventHandler Clicked = (_, _) => { };

    public AddNewButton()
    {
        InitializeComponent();
    }


    private void InternalAddNewButton_OnClicked(object? sender, EventArgs e)
    {
        Clicked.Invoke(this, EventArgs.Empty);
    }
}