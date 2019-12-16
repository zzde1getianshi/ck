﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
using Pixeval.Core;
using Pixeval.Data.ViewModel;
using Pixeval.Data.Web;
using Pixeval.Data.Web.Delegation;
using Pixeval.Data.Web.Request;
using Pixeval.Objects;
using Pixeval.Objects.Exceptions;

namespace Pixeval
{
    /// <summary>
    ///     Interaction logic for UserViewer.xaml
    /// </summary>
    public partial class UserViewer
    {
        private readonly SnackbarMessageQueue messageQueue = new SnackbarMessageQueue();
        private readonly User user;

        private bool atUploadSelector;

        public UserViewer(User usr)
        {
            user = usr;
            DataContext = user;

            InitializeComponent();
            if (Dispatcher != null) Dispatcher.UnhandledException += DispatcherOnUnhandledException;

            UserViewerSnackBar.MessageQueue = messageQueue;
        }

        public static async void Show(string id)
        {
            var info = await HttpClientFactory.AppApiService.GetUserInformation(new UserInformationRequest {Id = id});
            var v = new UserViewer(new User
            {
                Avatar = info.UserEntity.ProfileImageUrls.Medium,
                Id = info.UserEntity.Id.ToString(),
                Introduction = info.UserEntity.Comment,
                IsFollowed = info.UserEntity.IsFollowed,
                Name = info.UserEntity.Name
            });
            v.Show();
        }

        private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception.InnerException == null || !e.Exception.InnerException.Message.Contains("The server returned an invalid or unrecognized response"))
            {
                messageQueue.Enqueue(e.Exception is QueryNotRespondingException ? Externally.QueryNotResponding : e.Exception.Message);

                e.Handled = true;
            }
        }

        private async void SetupUploads()
        {
            atUploadSelector = true;
            var c = UiHelper.NewItemsSource<Illustration>(ImageListView);

            var pages = await PixivHelper.GetUploadPagesCount(user.Id);
            var iterator = new UploadIterator(user.Id, pages);

            await foreach (var illust in iterator.MoveNextAsync()) c.AddIllust(illust);
        }

        private async void SetupFavorite()
        {
            atUploadSelector = false;
            var c = UiHelper.NewItemsSource<Illustration>(ImageListView);

            var iterator = new GalleryIterator(user.Id);
            while (iterator.HasNext())
                await foreach (var illust in iterator.MoveNextAsync())
                    c.AddIllust(illust);
        }

        private void ShowcaseContainer_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!ShowcaseTranslateTransform.Y.Equals(0)) ShowcaseTranslateTransform.SetCurrentValue(TranslateTransform.YProperty, -e.NewSize.Height);
        }

        private void UserViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            SetupBackgroundImage();
        }

        private async void SetupBackgroundImage()
        {
            var link = $"https://public-api.secure.pixiv.net/v1/users/{user.Id}/works.json?page=1&publicity=public&per_page=1&image_sizes=large";
            var httpClient = HttpClientFactory.PixivApi(ProtocolBase.PublicApiBaseUrl);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer");

            var res = (await httpClient.GetStringAsync(link)).FromJson<dynamic>();
            if (((IEnumerable<JToken>) res.response).Any())
            {
                var img = res.response[0].image_urls.large.ToString();
                UiHelper.SetImageSource(BackgroundImage, await PixivImage.FromUrl(img));
            }
        }

        private void UploadSelector_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.GetResources<Storyboard>("SelectorSnackBarMoveLeftAnimation").Begin();
            this.GetResources<Storyboard>("SelectorOpacityMaskMoveLeftAnimation").Begin();

            SetupUploads();
        }

        private void FavoriteSelector_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.GetResources<Storyboard>("SelectorSnackBarMoveRightAnimation").Begin();
            this.GetResources<Storyboard>("SelectorOpacityMaskMoveRightAnimation").Begin();

            SetupFavorite();
        }

        private void IllustrationContainer_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IllustViewer.Show(sender.GetDataContext<Illustration>(), ImageListView.ItemsSource as IEnumerable<Illustration>);
        }

        private async void DownloadNowMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var illust = sender.GetDataContext<Illustration>();

            DownloadList.Remove(illust);
            await PixivImage.DownloadIllustInternal(illust);
            messageQueue.Enqueue(Externally.DownloadComplete(illust));
        }

        private async void DownloadAllNowMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            DownloadList.ToDownloadList.Clear();
            await PixivImage.DownloadIllustsInternal((IEnumerable<Illustration>) ImageListView.ItemsSource, Path.Combine(user.Name, $"{(atUploadSelector ? "作品" : "收藏")}"));
            messageQueue.Enqueue(Externally.AllDownloadComplete);
        }

        private void AddToDownloadListMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            DownloadList.Add(sender.GetDataContext<Illustration>());
            messageQueue.Enqueue(Externally.AddedAllToDownloadList);
        }

        private void AddAllToDownloadListMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            DownloadList.AddRange(((IEnumerable<Illustration>) ImageListView.ItemsSource).ToList());
            messageQueue.Enqueue(Externally.AddedAllToDownloadList);
        }

        private async void Thumbnail_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dataContext = sender.GetDataContext<Illustration>();

            if (dataContext != null && Uri.IsWellFormedUriString(dataContext.Thumbnail, UriKind.Absolute))
                UiHelper.SetImageSource((Image) sender, await PixivImage.GetAndCreateOrLoadFromCacheInternal(dataContext.Thumbnail, dataContext.Id));

            UiHelper.StartDoubleAnimationUseCubicEase(sender, "(Image.Opacity)", 0, 1, 500);
        }

        private void Thumbnail_OnUnloaded(object sender, RoutedEventArgs e)
        {
            UiHelper.ReleaseImage((Image) sender);
        }

        private void FavoriteButton_OnClick(object sender, RoutedEventArgs e)
        {
            PixivClient.Instance.PostFavoriteAsync(sender.GetDataContext<Illustration>());
        }

        private void RemoveFavoriteButton_OnClick(object sender, RoutedEventArgs e)
        {
            PixivClient.Instance.RemoveFavoriteAsync(sender.GetDataContext<Illustration>());
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            UiHelper.ReleaseItemsSource(ImageListView);
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            UploadSelector.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) {RoutedEvent = MouseLeftButtonDownEvent});
        }

        private async void ShowcaseContainer_OnLoaded(object sender, RoutedEventArgs e)
        {
            UiHelper.SetImageSource(UserAvatar, await PixivImage.FromUrl(this.GetDataContext<User>().Avatar));
        }

        private async void FollowButton_OnClick(object sender, RoutedEventArgs e)
        {
            await PixivClient.Instance.FollowArtist(user);
            messageQueue.Enqueue(Externally.SuccessfullyFollowUser);
        }

        private async void UnFollowButton_OnClick(object sender, RoutedEventArgs e)
        {
            await PixivClient.Instance.UnFollowArtist(user);
            messageQueue.Enqueue(Externally.SuccessfullyUnFollowUser);
        }
    }
}