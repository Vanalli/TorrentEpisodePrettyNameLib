using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Xml;

namespace TorrentEpisodePrettyNameLib
{
    public class TVRageClient : TVService
    {
        private readonly WebClient webClient = new WebClient();
        private readonly Dictionary<string, KeyValuePair<int, XmlDocument>> showInfoBox = new Dictionary<string, KeyValuePair<int, XmlDocument>>();

        private const string GET_SHOW_ID_URI = "http://services.tvrage.com/feeds/search.php?show={0}";
        private const string GET_SHOW_EPISODE_NAME_URI = "http://services.tvrage.com/feeds/episodeinfo.php?show={0}&exact=1&ep={1}x{2}";
        
        private int GetShowId(string showName)
        {
            var xmlShowName = webClient.DownloadString(String.Format(GET_SHOW_ID_URI, showName));

            if (String.IsNullOrEmpty(xmlShowName))
                throw new InvalidFilterCriteriaException(String.Format("Não foi encontrado o programa \"{0}\"", showName));

            var doc = new XmlDocument();
            doc.LoadXml(xmlShowName);

            var showNodes = doc.SelectNodes(String.Format("/Results/show[name='{0}']/showid", showName));

            if (showNodes == null)
                throw new InvalidFilterCriteriaException(String.Format("Não foi encontrado o programa \"{0}\"", showName));

            if (showNodes.Count > 1)
                return -1;

            return Int32.Parse(showNodes[0].InnerXml);
        }

        public IEnumerable<string> GetEpisodesList(int showId)
        {
            throw new NotImplementedException();
        }

        public EpisodeInfo GetEpisodeName(EpisodeInfo episode)
        {
            if (!showInfoBox.ContainsKey(episode.ShowName))
            {
                var xmlEpisodeInfo = webClient.DownloadString(String.Format(GET_SHOW_EPISODE_NAME_URI, episode.ShowName, episode.Season, episode.Episode));

                if (String.IsNullOrEmpty(xmlEpisodeInfo))
                    throw new InvalidFilterCriteriaException(String.Format("Não foi encontrado o programa \"{0}\"", episode.ShowName));

                var doc = new XmlDocument();
                doc.LoadXml(xmlEpisodeInfo);

                var idNode = int.Parse(doc.SelectSingleNode("/show/@id").InnerText);

                showInfoBox.Add(episode.ShowName, new KeyValuePair<int, XmlDocument>(idNode, doc));
            }

            var showId = showInfoBox[episode.ShowName].Key;
            var showInfo = showInfoBox[episode.ShowName].Value;

            if (showInfo.SelectSingleNode("/show/episode/number").InnerText == episode.GetFormatedSeasonEpisodeString())
            {
                episode.Name = showInfo.SelectSingleNode("/show/episode/title").InnerText;
            }

            return episode;
        }

        public List<EpisodeInfo> GetEpisodeNames(List<EpisodeInfo> episodes)
        {
            episodes.ForEach(episode => episode = GetEpisodeName(episode));

            return episodes;
        }
    }
}
