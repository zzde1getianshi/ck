using Refit;

namespace Pixeval.Data.Web.Request
{
    public class ToggleR18StateRequest
    {
        [AliasAs("mode")]
        public string Mode { get; } = "mod";

        [AliasAs("user_language")]
        public string UserLang { get; } = "zh";

        [AliasAs("r18")]

        public string R18 { get; set; }

        [AliasAs("r18g")]
        public string R18G { get; set; }

        [AliasAs("submit")]
        public string Submit { get; } = "保存";

        [AliasAs("tt")]
        public string Tt { get; set; }
    }
}