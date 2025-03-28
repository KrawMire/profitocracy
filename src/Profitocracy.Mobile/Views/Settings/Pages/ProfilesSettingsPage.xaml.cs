using Profitocracy.Core.Exceptions;
using Profitocracy.Mobile.Abstractions;
using Profitocracy.Mobile.Models.Profiles;
using Profitocracy.Mobile.Resources.Strings;
using Profitocracy.Mobile.ViewModels.Profiles;

namespace Profitocracy.Mobile.Views.Settings.Pages;

public partial class ProfilesSettingsPage : BaseContentPage
{
    private readonly ProfileSettingsPageViewModel _viewModel;
    
    public ProfilesSettingsPage(ProfileSettingsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        ProfilesCollectionView.ItemsSource = _viewModel.Profiles;
    }

    private void AddProfileButton_OnClicked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            await OpenEditProfilePage(null);
        });
    }
    
    private void EditProfile_OnInvoked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            if (sender is not SwipeItemView swipeItem)
            {
                throw new InvalidCastException(AppResources.CommonError_InternalErrorTryAgain);
            }
			
            var profile = swipeItem.BindingContext as ProfileModel;

            if (profile?.Id is null)
            {
                throw new ArgumentNullException(AppResources.CommonError_GetProfileById);
            }
			
            await OpenEditProfilePage((Guid)profile.Id);
        });
    }
    
    private void UpdateProfilesList(object? sender, NavigatedToEventArgs e)
    {
        ProcessAction(async () =>
        {
            await _viewModel.Initialize();
        });
    }

    private async Task OpenEditProfilePage(Guid? profileId)
    {
        var addPage = Handler?.MauiContext?.Services.GetService<EditProfilePage>();

        if (addPage is null)
        {
            throw new ArgumentNullException(AppResources.CommonError_OpenAddCategoryPage);
        }

        if (profileId is not null)
        {
            addPage.AddProfileId((Guid)profileId);
        }

        await Navigation.PushModalAsync(addPage);
    }

    private void ProfileCard_OnTapped(object? sender, TappedEventArgs e)
    {
        ProcessAction(async () =>
        {
            if (sender is not Border profileCardBorder)
            {
                throw new InvalidCastException(AppResources.CommonError_InternalErrorTryAgain);
            }
			
            var profile = profileCardBorder.BindingContext as ProfileModel;

            if (profile?.Id is null)
            {
                throw new ArgumentNullException(AppResources.CommonError_GetProfileById);
            }

            var changeCurrentProfile = await DisplayAlert(
                AppResources.ProfileSettings_ChangeAlert_Title,
                string.Format(AppResources.ProfileSettings_ChangeAlert_Desription, profile.Name),
                AppResources.ProfileSettings_ChangeAlert_Ok,
                AppResources.ProfileSettings_ChangeAlert_Cancel);

            if (changeCurrentProfile)
            {
                await _viewModel.SetCurrentProfile((Guid)profile.Id);
            }
        });
    }

    private void DeleteProfile_OnInvoked(object? sender, EventArgs e)
    {
        ProcessAction(async () =>
        {
            if (sender is not SwipeItemView swipeItem)
            {
                throw new InvalidCastException(AppResources.CommonError_InternalErrorTryAgain);
            }
			
            var profile = swipeItem.BindingContext as ProfileModel;

            if (profile?.Id is null)
            {
                throw new ArgumentNullException(AppResources.CommonError_FindCategoryToDelete);
            }

            var isDelete = await DisplayAlert(
                AppResources.ProfileSettings_DeleteAlert_Title,
                string.Format(AppResources.ProfileSettings_DeleteAlert_Description, profile.Name),
                AppResources.ProfileSettings_DeleteAlert_Ok,
                AppResources.ProfileSettings_DeleteAlert_Cancel);

            if (isDelete)
            {
                try
                {
                    await _viewModel.DeleteProfile((Guid)profile.Id);
                }
                catch (LastProfileDeletionException ex)
                {
                    await DisplayAlert(
                        AppResources.ProfileSettings_LastProfileErrorAlert_Title,
                        string.Format(AppResources.ProfileSettings_LastProfileErrorAlert_Description, ex.ProfileName),
                        AppResources.ProfileSettings_LastProfileErrorAlert_Ok);
                }
            }
        });
    }
}