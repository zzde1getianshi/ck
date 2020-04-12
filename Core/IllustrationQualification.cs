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

using System.Text.RegularExpressions;

namespace Pixeval.Core
{
    public class IllustrationQualification
    {
        public IllustrationQualification(ConditionType type, string condition)
        {
            Type = type;
            Condition = condition;
        }

        public ConditionType Type { get; set; }

        public string Condition { get; set; }

        public static IllustrationQualification Parse(string input)
        {
            var match = Regex.Match(input, @"#(?<type>(?i)id|name|tag(?-i)):(?<content>.+)");
            if (match.Success)
            {
                var header = match.Groups["type"].Value.ToLower();
                var content = match.Groups["content"].Value;
                return header switch
                {
                    "id"                               => new IllustrationQualification(ConditionType.Id, content),
                    "name"                             => new IllustrationQualification(ConditionType.Name, content),
                    "tag" when content.StartsWith("!") => new IllustrationQualification(ConditionType.ExcludeTag, content),
                    "tag"                              => new IllustrationQualification(ConditionType.Tag, content),
                    _                                  => null
                };
            }

            return null;
        }
    }

    public enum ConditionType
    {
        Name,
        Id,
        Tag,
        ExcludeTag
    }
}