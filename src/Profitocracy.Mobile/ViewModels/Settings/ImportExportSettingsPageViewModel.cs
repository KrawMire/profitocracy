using CommunityToolkit.Maui.Storage;
using Profitocracy.Core.Integrations;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Resources.Strings;

namespace Profitocracy.Mobile.ViewModels.Settings;

public class ImportExportSettingsPageViewModel : BaseNotifyObject
{
    private const string BackupFileName = "profitocracy_backup";
    private readonly IBackupProvider _backupProvider;

    private bool _isExportingProfiles;
    private bool _isExportingCategories;
    private bool _isExportingTransactions;

    public ImportExportSettingsPageViewModel(IBackupProvider backupProvider)
    {
        _backupProvider = backupProvider;
    }

    public bool IsExportingProfiles
    {
        get => _isExportingProfiles;
        set
        {
            if (_isExportingProfiles == value)
            {
                return;
            }

            if (!value)
            {
                IsExportingCategories = false;
                IsExportingTransactions = false;
            }

            SetProperty(ref _isExportingProfiles, value);
        }
    }
    public bool IsExportingCategories
    {
        get => _isExportingCategories;
        set
        {
            if (_isExportingCategories == value)
            {
                return;
            }

            if (value)
            {
                IsExportingProfiles = true;
            }
            else
            {
                IsExportingTransactions = false;
            }

            SetProperty(ref _isExportingCategories, value);
        }
    }

    public bool IsExportingTransactions
    {
        get => _isExportingTransactions;
        set
        {
            if (_isExportingTransactions == value)
            {
                return;
            }

            if (value)
            {
                IsExportingProfiles = true;
                IsExportingCategories = true;
            }

            SetProperty(ref _isExportingTransactions, value);
        }
    }

    public async Task ImportAsync()
    {
        var pickOptions = new PickOptions
        {
            PickerTitle = AppResources.ImportExport_SelectBackupFile,
        };

        var result = await FilePicker.Default.PickAsync(pickOptions);

        if (result is null)
        {
            throw new Exception(AppResources.CommonError_NoDataToImport);
        }

        var correctExtension = result.FileName.EndsWith(
            _backupProvider.BackupFileExtension,
            StringComparison.InvariantCultureIgnoreCase);

        if (!correctExtension)
        {
            throw new Exception(AppResources.CommonError_WrongFileExtension);
        }

        await using var backupFileStream = await result.OpenReadAsync();
        await _backupProvider.ImportDataAsync(backupFileStream);
    }

    public async Task ExportAsync()
    {
        if (!IsExportingProfiles && !IsExportingCategories && !IsExportingTransactions)
        {
            throw new Exception(AppResources.CommonError_ExportDataIsEmpty);
        }

        await using var stream = await _backupProvider.ExportDataAsync(
            IsExportingProfiles,
            IsExportingCategories,
            IsExportingTransactions);

        var filename = $"{BackupFileName}.{_backupProvider.BackupFileExtension}";

#pragma warning disable CA1416
        var saveResult = await FileSaver.Default.SaveAsync(filename, stream);
#pragma warning restore CA1416

        saveResult.EnsureSuccess();
    }
}