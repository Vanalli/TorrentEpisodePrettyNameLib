using System;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace TorrentEpisodePrettyNameLib
{
    public class FileNameParser
    {
        private const string EpisodeInfoPattern = "s[eason]* *[0-9]* *e[pisode]* *[0-9]*";

        private string _originalFileName;
        private string _fileNameWithoutExtensionAndClean;
        private string _episodeInfoOnFileName;

        public string OriginalFileName
        {
            get { return _originalFileName; }
            set
            {
                _originalFileName = value;
                Parse();
            }
        }

        public string FileNameExtension { get; set; }

        public string ShowName { get; set; }

        public int Season { get; set; }

        public int Episode { get; set; }

        public EpisodeInfo EpisodeInfo { get; set; }

        public bool Success { get; set; }

        private void Parse()
        {
            try
            {
                StripExtensionAndCleanFileName();
                StripEpisodeInfo();
                GetShowName();

                EpisodeInfo = new EpisodeInfo
                {
                    ShowName = ShowName,
                    Season = Season,
                    Episode = Episode
                };
            }
            catch
            {
                Success = false;
            }
        }

        private void StripExtensionAndCleanFileName()
        {
            FileNameExtension = OriginalFileName.Substring(OriginalFileName.LastIndexOf('.'));
            _fileNameWithoutExtensionAndClean = Regex.Replace(OriginalFileName.Replace(FileNameExtension, String.Empty), @"[\._-]+", " ");
        }

        private void GetShowName()
        {
            var episodeInfo = new Regex(EpisodeInfoPattern, RegexOptions.IgnoreCase).Match(_fileNameWithoutExtensionAndClean);

            Success = episodeInfo.Success;

            if (!episodeInfo.Success)
             throw new Exception();

            _episodeInfoOnFileName = episodeInfo.Value;

            ShowName = _fileNameWithoutExtensionAndClean.Substring(0, _fileNameWithoutExtensionAndClean.IndexOf(_episodeInfoOnFileName, StringComparison.Ordinal)).Trim();
        }

        private void StripEpisodeInfo()
        {
            var episodeInfo = new Regex(EpisodeInfoPattern, RegexOptions.IgnoreCase).Match(_fileNameWithoutExtensionAndClean);

            Success = episodeInfo.Success;

            if (!episodeInfo.Success)
                throw new Exception();

            _episodeInfoOnFileName = episodeInfo.Value;

            var episodeString = _episodeInfoOnFileName
                .ToUpper()
                .Replace(" ", String.Empty)
                .Replace("0", String.Empty)
                .Replace("SEASON", "S")
                .Replace("EPISODE", "E")
                .Replace("S", String.Empty);

            var seasonEpisodeGroup = episodeString.Split('E');

            Season = int.Parse(seasonEpisodeGroup[0]);
            Episode = int.Parse(seasonEpisodeGroup[1]);
        }
    }
}
