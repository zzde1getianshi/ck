using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Pixeval.Data.ViewModel;
using Pixeval.Data.Web;
using Pixeval.Data.Web.Delegation;
using Pixeval.Objects;
using Pixeval.Objects.Exceptions;

namespace Pixeval.Core
{
    /// <summary>
    /// This class is piece of shit
    /// </summary>
    public class TrendsAsyncEnumerable : AbstractPixivAsyncEnumerable<Trends>
    {
        public static TrendsAsyncEnumerable CurrentSession = new TrendsAsyncEnumerable();

        public override int RequestedPages { get; protected set; }

        public override IAsyncEnumerator<Trends> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TrendsAsyncEnumerator(this);
        }

        private class TrendsAsyncEnumerator : AbstractPixivAsyncEnumerator<Trends>
        {
            private IEnumerator<Trends> trendsEnumerable;
            private TrendsRequestContext requestContext;
            private string tt;

            public TrendsAsyncEnumerator(IPixivAsyncEnumerable<Trends> enumerable) : base(enumerable) { }

            public override async ValueTask<bool> MoveNextAsync()
            {
                if (requestContext == null)
                {
                    if (await GetResponse(BuildRequestUrl()) is (true, var result))
                    {
                        tt = Regex.Match(result, "tt: \"(?<tt>.*)\"").Groups["tt"].Value;
                        trendsEnumerable = (await ParsePreloadJsonFromHtml(result)).NonNull().GetEnumerator();
                        requestContext = ExtractRequestParametersFromHtml(result);
                    } else throw new QueryNotRespondingException();
                    Enumerable.ReportRequestedPages();
                }

                if (trendsEnumerable.MoveNext()) return true;

                if (requestContext.IsLastPage) return false;

                if (await GetResponse(BuildRequestUrl()) is (true, var json))
                {
                    trendsEnumerable = (await ParseRawJson(json)).NonNull().GetEnumerator();
                    requestContext = ExtractRequestParametersFromRawJson(json);
                    Enumerable.ReportRequestedPages();
                    return true;
                }

                return false;
            }

            private string BuildRequestUrl()
            {
                return requestContext == null ? "/stacc?mode=unify" : $"/stacc/my/home/all/activity/{requestContext.Sid}/.json?mode={requestContext.Mode}&unify_token={requestContext.UnifyToken}&tt={tt}";
            }

            private static TrendsRequestContext ExtractRequestParametersFromHtml(string html)
            {
                var json = JObject.Parse(ExtractPreloadJsonSnippet(html));
                return new TrendsRequestContext
                {
                    Mode = json["param"]["mode"].Value<string>(),
                    UnifyToken = json["param"]["unify_token"].Value<string>(),
                    Sid = json["next_max_sid"].Value<long>().ToString(),
                    IsLastPage = json["is_last_page"].Value<int>() == 1
                };
            }

            private static TrendsRequestContext ExtractRequestParametersFromRawJson(string json)
            {
                var obj = JObject.Parse(json);
                return new TrendsRequestContext
                {
                    Mode = obj["stacc"]["param"]["mode"].Value<string>(),
                    UnifyToken = obj["stacc"]["param"]["unify_token"].Value<string>(),
                    Sid = obj["stacc"]["next_max_sid"].Value<long>().ToString(),
                    IsLastPage = obj["stacc"]["is_last_page"].Value<int>() == 1
                };
            }

            private static Task<IEnumerable<Trends>> ParsePreloadJsonFromHtml(string html)
            {
                return ParsePreloadJson(ExtractPreloadJsonSnippet(html));
            }

            private static string ExtractPreloadJsonSnippet(string html)
            {
                var match = Regex.Match(html, "pixiv\\.stacc\\.env\\.preload\\.stacc \\= (?<json>.*);");
                if (!match.Success) throw new QueryNotRespondingException();
                return match.Groups["json"].Value;
            }

            private static Task<IEnumerable<Trends>> ParseRawJson(string json)
            {
                var stacc = JObject.Parse(json)["stacc"].ToString();
                return ParsePreloadJson(stacc);
            }

            private static async Task<IEnumerable<Trends>> ParsePreloadJson(string json)
            {
                var tasks = new List<Task<Trends>>();
                var stacc = JObject.Parse(json);
                var status = stacc["status"];
                var timeline = stacc["timeline"];
                var user = stacc["user"];
                var illust = stacc["illust"];
                foreach (var timelineChild in timeline)
                {
                    var task = Task.Run(() =>
                    {
                        var timelineProp = timelineChild.First;
                        var statusObj = status.FirstOrDefault(sChild => sChild.First["id"].Value<string>() == timelineProp["id"].Value<string>());
                        if (statusObj?.First == null) return null;
                        var statusObjProp = statusObj.First;
                        var trendsObj = new Trends
                        {
                            PostDate = DateTime.Parse(statusObjProp["post_date"].Value<string>(), CultureInfo.CurrentCulture),
                            PostUser = statusObjProp["post_user"]["id"].Value<string>(),
                            TrendObjectId = statusObjProp["type"].Value<string>() switch
                            {
                                var type when type == "add_illust" || type == "add_bookmark" => statusObjProp["ref_illust"]["id"].Value<string>(),
                                "add_favorite" => statusObjProp["ref_user"]["id"].Value<string>(),
                                _ => null
                            }
                        };
                        trendsObj.PostUserThumbnail = user.FirstOrDefault(uChild => uChild.First["id"].Value<string>() == trendsObj.PostUser)?.First["profile_image"].First.First["url"]["m"].Value<string>();
                        trendsObj.Type = statusObjProp["type"].Value<string>() switch
                        {
                            "add_illust" => TrendType.AddIllust,
                            "add_bookmark" => TrendType.AddBookmark,
                            "add_favorite" => TrendType.AddFavorite,
                            _ => (TrendType)(-1)
                        };
                        trendsObj.TrendObjectThumbnails = trendsObj.Type switch
                        {
                            var type when type == TrendType.AddBookmark || type == TrendType.AddIllust => illust.FirstOrDefault(iChild => iChild.First["id"].Value<string>() == trendsObj.TrendObjectId)?.First["url"]["s"].Value<string>(),
                            TrendType.AddFavorite => user.FirstOrDefault(uChild => uChild.First["id"].Value<string>() == trendsObj.TrendObjectId)?.First["profile_image"].First.First["url"]["m"].Value<string>(),
                            (TrendType)(-1) => null,
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        return trendsObj;
                    });
                    tasks.Add(task);
                }

                return await Task.WhenAll(tasks);
            }

            public override Trends Current => trendsEnumerable.Current;

            protected override void UpdateEnumerator()
            {
                throw new NotImplementedException();
            }

            private static async Task<HttpResponse<string>> GetResponse(string url)
            {
                var result = await HttpClientFactory.WebApiHttpClient().Apply(h => h.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "zh-cn")).GetStringAsync(url);
                return !result.IsNullOrEmpty() ? HttpResponse<string>.Wrap(true, result) : HttpResponse<string>.Wrap(false);
            }
        }

        private class TrendsRequestContext
        {

            public string UnifyToken { get; set; }

            public string Sid { get; set; }

            public string Mode { get; set; }

            public bool IsLastPage { get; set; }
        }
    }
}