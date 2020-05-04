using System;
using PropertyChanged;

namespace Pixeval.Data.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class Trends
    {
        public string PostUser { get; set; }

        public string TrendObjectId { get; set; }

        public DateTime PostDate { get; set; }

        public TrendType Type { get; set; }

        public string TrendObjectThumbnails { get; set; }

        public string PostUserThumbnail { get; set; }
    }

    public enum TrendType
    {
        /// <summary>
        /// Bookmark
        /// </summary>
        AddBookmark,

        /// <summary>
        /// New illust
        /// </summary>
        AddIllust,

        /// <summary>
        /// New follow
        /// </summary>
        AddFavorite
    }
}