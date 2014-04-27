using System.Collections.Generic;

namespace TorrentEpisodePrettyNameLib
{
    public interface TVService
    {
        IEnumerable<string> GetEpisodesList(int showId);

        EpisodeInfo GetEpisodeName(EpisodeInfo episode);

        List<EpisodeInfo> GetEpisodeNames(List<EpisodeInfo> episodes);
    }
}
