using CommunityToolkit.Maui.Storage;
using Profitocracy.Core.Integrations;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Resources.Strings;
using System.Diagnostics;

namespace Profitocracy.Mobile.ViewModels.Settings;

public class ImportExportSettingsPageViewModel : BaseNotifyObject
{
    private const string BackupFileName = "profitocracy_backup";
    private readonly IBackupProvider _backupProvider;

    private bool _isExportingProfiles;
    private bool _isExportingCategories;
    private bool _isExportingTransactions;
    private bool _isShowImportProgress;
    private int _currentImportIndex;
    private int _totalImportIndex;
    private float _importProgress;

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

    public bool IsShowImportProgress
    {
        get => _isShowImportProgress;
        set => SetProperty(ref _isShowImportProgress, value);
    }

    public int CurrentImportIndex
    {
        get => _currentImportIndex;
        set => SetProperty(ref _currentImportIndex, value);
    }

    public int TotalImportIndex
    {
        get => _totalImportIndex;
        set => SetProperty(ref _totalImportIndex, value);
    }

    public float ImportProgress
    {
        get => _importProgress;
        set => SetProperty(ref _importProgress, value);
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

        Exception? exception = null;

        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsShowImportProgress = true;
            });

            await using var backupFileStream = await result.OpenReadAsync();

            await foreach (var (current, total) in _backupProvider.ImportDataAsync(backupFileStream))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    CurrentImportIndex = current;
                    TotalImportIndex = total;
                    ImportProgress = total > 0 ? current / (float)total : 0;
                });
            }
        }
        catch (Exception ex)
        {
            exception = ex;
        }
        finally
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsShowImportProgress = false;
                CurrentImportIndex = 0;
                TotalImportIndex = 0;
                ImportProgress = 0;
            });
        }

        if (exception is not null)
        {
            throw exception;
        }
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
