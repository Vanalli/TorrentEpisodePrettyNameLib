using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using TorrentEpisodePrettyNameLib;

namespace TorrentShowEpisodePrettyName
{
    public partial class Form1 : Form
    {
        private Dictionary<string, string> _showDestinationBox = new Dictionary<string, string>();

        public Form1()
        {
            InitializeComponent();

            //_showDestinationBox.Add("Vikings", @"\\192.168.1.35\WDTVLiveHub\Video\Series\Vikings");
            //_showDestinationBox.Add("Revolution", @"\\192.168.1.35\WDTVLiveHub\Video\Series\Revolution");
            //_showDestinationBox.Add("The Big Bang Theory", @"\\192.168.1.35\WDTVLiveHub\Video\Series\Big Bang Theory");

            //var execPath = Assembly.GetExecutingAssembly().Location;

            //var confiFileStream = File.OpenWrite(Path.Combine(execPath.Substring(0, execPath.LastIndexOf(@"\", System.StringComparison.Ordinal)), "Config.txt"));

            //var jsonConfig = new JavaScriptSerializer().Serialize(_showDestinationBox);

            //var buffer = new byte[jsonConfig.Length * sizeof(char)];
            //System.Buffer.BlockCopy(jsonConfig.ToCharArray(), 0, buffer, 0, buffer.Length);

            //confiFileStream.Write(buffer, 0, buffer.Length);

            //

            var execPath = Assembly.GetExecutingAssembly().Location;
            using (StreamReader streamReader = new StreamReader(Path.Combine(execPath.Substring(0, execPath.LastIndexOf(@"\", System.StringComparison.Ordinal)), "Config.txt"), Encoding.UTF8))
            {
                var config = streamReader.ReadToEnd();
                _showDestinationBox = (Dictionary<string, string>)new JavaScriptSerializer().Deserialize(config, typeof(Dictionary<string, string>));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AllowDrop = true;
            DragEnter += Form1_DragEnter;
            DragDrop += Form1_DragDrop;

            textBox1.AllowDrop = true;
            textBox1.DragEnter += Form1_DragEnter;
            textBox1.DragDrop += Form1_DragDrop;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var episodes = new List<EpisodeInfo>();
                var fileNameParser = new FileNameParser();
                string[] filePaths = (string[]) (e.Data.GetData(DataFormats.FileDrop));
                foreach (string fileLoc in filePaths)
                {
                    var tvServiceClient = new TVRageClient();
                    fileNameParser.OriginalFileName = Path.GetFileName(fileLoc);
                    if (fileNameParser.Success)
                    {
                        var epInfo = fileNameParser.EpisodeInfo;
                        epInfo.SourceFilePath = fileLoc;
                        episodes.Add(epInfo);
                    }
                }

                episodes = new TVRageClient().GetEpisodeNames(episodes);

                episodes.ForEach(episode =>
                {
                    textBox1.Text +=
                        "Show name: " + episode.ShowName + Environment.NewLine +
                        "Season #: " + episode.Season + Environment.NewLine +
                        "Episode #: " + episode.Episode + Environment.NewLine +
                        "Episode name: " + episode.Name + Environment.NewLine + Environment.NewLine;
                });
            }

        }

        private EpisodeInfo DiscoverEpisodeInfoFrom(string fileName)
        {
            var fileNameParser = new FileNameParser
            {
                OriginalFileName = fileName
            };

            return new EpisodeInfo
            {
                ShowName = fileNameParser.ShowName,
                Season =  fileNameParser.Season,
                Episode = fileNameParser.Episode
            };

            
        }
    }
}
