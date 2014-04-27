using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorrentEpisodePrettyNameLib;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("(P)esquisar ou (S)air? ");
            var opcao = Console.ReadKey();
            while (opcao.Key.ToString().ToUpper() == "P")
            {
                Console.WriteLine(Environment.NewLine);
                Console.Write("File name: ");
                var episode = Console.ReadLine();

                var fileNameParser = new FileNameParser
                {
                    OriginalFileName = episode
                };

                if (!fileNameParser.Success)
                    Console.WriteLine("Deu bosta!");

                var episodeInfo = new EpisodeInfo
                {
                    ShowName = fileNameParser.ShowName,
                    Season = fileNameParser.Season,
                    Episode = fileNameParser.Episode
                };

                var tvService = new TVRageClient();
                var episodeName = tvService.GetEpisodeName(episodeInfo);

                Console.WriteLine("Show name: " + fileNameParser.ShowName);
                Console.WriteLine("Season : " + fileNameParser.Season);
                Console.WriteLine("Episode: " + fileNameParser.Episode);
                Console.WriteLine("Name: " + episodeName);
                Console.WriteLine("");

                Console.WriteLine("(P)esquisar ou (S)air? ");
                opcao = Console.ReadKey();
            }

            Console.Read();
        }
    }
}
