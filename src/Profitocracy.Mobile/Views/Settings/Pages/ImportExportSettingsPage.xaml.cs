using Profitocracy.Mobile.ViewModels.Settings;

namespace Profitocracy.Mobile.Views.Settings.Pages;

public partial class ImportExportSettingsPage
{
    private readonly ImportExportSettingsPageViewModel _viewModel;

    public ImportExportSettingsPage(ImportExportSettingsPageViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
    }
}