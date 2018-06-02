using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentManagement.Services.Seo
{
    public static class RobotsIndexManager
    {
        public const RobotsIndex DefaultRobotsNoIndex = RobotsIndex.NoIndexFollow;

        public const string IndexFollowMetaContent = "INDEX, FOLLOW";

        public const string IndexNoFollowMetaContent = "INDEX, NOFOLLOW";

        public const string NoIndexFollowMetaContent = "NOINDEX, FOLLOW";

        public const string NoIndexNoFollowMetaContent = "NOINDEX, NOFOLLOW";

        public static string GetMetaContent(RobotsIndex robotsIndex)
        {
            switch (robotsIndex)
            {
                case RobotsIndex.IndexFollow:
                    return IndexFollowMetaContent;
                case RobotsIndex.IndexNoFollow:
                    return IndexNoFollowMetaContent;
                case RobotsIndex.NoIndexFollow:
                    return NoIndexFollowMetaContent;
                case RobotsIndex.NoIndexNoFollow:
                    return NoIndexNoFollowMetaContent;
                default:
                    string message = $"No mapping found for {nameof(RobotsIndex)} value '{robotsIndex}'.";
                    throw new ArgumentOutOfRangeException(nameof(robotsIndex), message);
            }
        }
    }
}
