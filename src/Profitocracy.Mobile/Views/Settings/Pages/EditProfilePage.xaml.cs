using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.ViewModels.Profiles;

namespace Profitocracy.Mobile.Views.Settings.Pages;

public partial class EditProfilePage : BaseContentPage
{
    private readonly EditProfilePageViewModel _viewModel;

    public EditProfilePage(EditProfilePageViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
        ProfileCurrencyPicker.ItemsSource = _viewModel.AvailableCurrencies;
    }

    public void AddProfileId(Guid profileId)
    {
        _viewModel.ProfileId = profileId;
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await _viewModel.CreateProfile();
            await Navigation.PopModalAsync();
        });
    }

    private void CloseButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await Navigation.PopModalAsync();
        });
    }

    private void EditProfilePage_OnLoaded(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await _viewModel.Initialize();
        });
    }
}
