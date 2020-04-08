// Pixeval - A Strong, Fast and Flexible Pixiv Client
// Copyright (C) 2019 Dylech30th
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Pixeval.Core;
using Pixeval.Data.ViewModel;
using Pixeval.Objects.Caching;

namespace Pixeval
{
    public static class AppContext
    {
        public const string AppIdentifier = "Pixeval";

        public const string CurrentVersion = "1.7.5";

        public const string ConfigurationFileName = "pixeval_conf.json";

        public static bool LogoutExit = false;

        public static readonly string ProjectFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppIdentifier.ToLower());

        public static readonly string ConfFolder = ProjectFolder;

        public static readonly string SettingsFolder = ProjectFolder;

        public static readonly string ExceptionReportFolder = Path.Combine(ProjectFolder, "crash-reports");

        public static readonly string CacheFolder = Path.Combine(ProjectFolder, "cache");

        public static IWeakCacheProvider<BitmapImage, Illustration> DefaultCacheProvider;

        public static readonly IDownloadPathProvider DownloadPathProvider = new DefaultDownloadPathProvider();

        public static readonly IIllustrationFileNameFormatter FileNameFormatter = new DefaultIllustrationFileNameFormatter();

        public static readonly ObservableCollection<DownloadableIllustration> Downloading = new ObservableCollection<DownloadableIllustration>();

        public static readonly ObservableCollection<DownloadableIllustration> Downloaded = new ObservableCollection<DownloadableIllustration>();

        public static readonly ObservableCollection<TrendingTag> TrendingTags = new ObservableCollection<TrendingTag>();

        private static readonly ObservableCollection<string> SearchingHistory = new ObservableCollection<string>();

        static AppContext()
        {
            Directory.CreateDirectory(ProjectFolder);
            Directory.CreateDirectory(SettingsFolder);
            Directory.CreateDirectory(ExceptionReportFolder);
        }

        public static void EnqueueSearchHistory(string keyword)
        {
            if (SearchingHistory.Count == 4) SearchingHistory.RemoveAt(SearchingHistory.Count - 1);
            SearchingHistory.Insert(0, keyword);
        }

        public static IEnumerable<string> GetSearchingHistory()
        {
            return SearchingHistory;
        }

        public static void EnqueueDownloadItem(Illustration illustration)
        {
            if (Downloading.Any(i => illustration.Id == i.DownloadContent.Id))
                return;

            static DownloadableIllustration CreateDownloadableIllustration(Illustration downloadContent, bool isFromMange, int index = -1)
            {
                var model = new DownloadableIllustration(downloadContent, isFromMange, index);
                model.DownloadStat.ValueChanged += (sender, args) => Application.Current.Dispatcher.Invoke(() =>
                {
                    switch (args.NewValue)
                    {
                        case DownloadStatEnum.Finished:
                            model.Freeze();
                            Downloading.Remove(model);
                            if (Downloaded.All(i => model.DownloadContent.GetDownloadUrl() != i.DownloadContent.GetDownloadUrl())) Downloaded.Add(model);
                            break;
                        case DownloadStatEnum.Downloading:
                            Downloaded.Remove(model);
                            Downloading.Add(model);
                            break;
                        case var stat when stat == DownloadStatEnum.Canceled || stat == DownloadStatEnum.Queue || stat == DownloadStatEnum.Exceptional:
                            if (stat == DownloadStatEnum.Canceled)
                                Downloading.Remove(model);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
                return model;
            }

            if (illustration.IsManga)
            {
                for (var j = 0; j < illustration.MangaMetadata.Length; j++)
                { 
                    var cpy = j;
                    Task.Run(() => CreateDownloadableIllustration(illustration.MangaMetadata[cpy], true, cpy).Download());
                }
            }
            else Task.Run(() => CreateDownloadableIllustration(illustration, false).Download());
        }

        public static async Task<bool> UpdateAvailable()
        {
            const string url = "http://47.95.218.243/Pixeval/version.txt";
            var httpClient = new HttpClient();
            return await httpClient.GetStringAsync(url) != CurrentVersion;
        }
    }
}