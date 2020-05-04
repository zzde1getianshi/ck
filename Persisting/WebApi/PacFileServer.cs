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

using System.IO;
using EmbedIO;

namespace Pixeval.Persisting.WebApi
{
    /// <summary>
    ///     A small server in order to represent the proxy-auto-configuration file
    /// </summary>
    public static class PacFileServer
    {
        public static WebServer Create(string hostname, int port)
        {
            var asm = Path.GetDirectoryName(typeof(PacFileServer).Assembly.Location);
            var server = new WebServer(o => o
                .WithUrlPrefix($"http://{hostname}:{port}")
                .WithMode(HttpListenerMode.EmbedIO)
            ).WithStaticFolder("/", Path.Combine(asm, "Resource"), false);
            return server;
        }
    }
}