using System;
using System.Globalization;

namespace TorrentEpisodePrettyNameLib
{
    public class EpisodeInfo
    {
        public string SourceFilePath { get; set; }

        public int ShowId { get; set; }

        public string ShowName { get; set; }

        public int Season { get; set; }

        public int Episode { get; set; }

        public string Name { get; set; }

        public string GetFormatedSeasonEpisodeString()
        {
            return String.Concat(Season.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'), "x", Episode.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'));
        }
    }
}
