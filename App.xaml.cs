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
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Pixeval.Core;
using Pixeval.Data.ViewModel;
using Pixeval.Objects.Caching;
using Pixeval.Persisting;
using Refit;

#if RELEASE
using Pixeval.Objects.Exceptions.Logger;

#endif

namespace Pixeval
{
    public partial class App
    {
        public App()
        {
            if (Dispatcher != null)
                Dispatcher.UnhandledException += (sender, args) => DispatcherOnUnhandledException(args.Exception);
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => DispatcherOnUnhandledException((Exception) args.ExceptionObject);
            TaskScheduler.UnobservedTaskException += (sender, args) => DispatcherOnUnhandledException(args.Exception);
        }

        private static void DispatcherOnUnhandledException(Exception e)
        {
#if RELEASE
            ExceptionLogger.WriteException(e);
#elif DEBUG
            if (e is ApiException apiException) MessageBox.Show(apiException.Content);
#endif
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            if (Process.GetProcessesByName(AppContext.AppIdentifier).Length > 1)
            {
                MessageBox.Show("已经有一个Pixeval实例在运行了", "不能同时运行两个Pixeval实例!", MessageBoxButton.OK);
                Environment.Exit(0);
            }

            await Settings.Restore();
            AppContext.DefaultCacheProvider = Settings.Global.CachingPolicy == CachingPolicy.Memory
                ? (IWeakCacheProvider<BitmapImage, Illustration>) MemoryCache<BitmapImage, Illustration>.Shared
                : new FileCache<BitmapImage, Illustration>(AppContext.CacheFolder, image => image.ToStream(), PixivIO.FromStream);
            AppContext.DefaultCacheProvider.Clear();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await Settings.Global.Store();
            AppContext.DefaultCacheProvider.Clear();
            if (!AppContext.LogoutExit && Identity.Global.AccessToken != null) await Identity.Global.Store();
            base.OnExit(e);
        }
    }
}