using Profitocracy.Mobile.ViewModels.Settings;

namespace Profitocracy.Mobile.Views.Settings.Pages;

public partial class ImportExportSettingsPage
{
    private readonly ImportExportSettingsPageViewModel _viewModel;

    public ImportExportSettingsPage(ImportExportSettingsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private void ImportButton_OnClicked(object? sender, EventArgs e)
        => ProcessAction(_viewModel.ImportAsync);

    private void ExportButton_OnClicked(object? sender, EventArgs e)
        => ProcessAction(_viewModel.ExportAsync);
}