using Profitocracy.Mobile.Resources.Strings;
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
        => ProcessAction(async () =>
        {
            await _viewModel.ImportAsync();
            await DisplayAlert(
                AppResources.ImportExport_SuccessImportAlert_Title,
                AppResources.ImportExport_SuccessImportAlert_Description,
                AppResources.ImportExport_SuccessImportAlert_Ok);
        });

    private void ExportButton_OnClicked(object? sender, EventArgs e)
        => ProcessAction(async () =>
        {
            await _viewModel.ExportAsync();
            await DisplayAlert(
                AppResources.ImportExport_SuccessExportAlert_Title,
                AppResources.ImportExport_SuccessExportAlert_Description,
                AppResources.ImportExport_SuccessExportAlert_Ok);
        });
}