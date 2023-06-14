using System;
using System.Linq;
using TopDownShooter.World.Data;

namespace TopDownShooter.World.CSV
{
    public class CsvToWorldDataBuilder
    {
        private const string TileSeparator = ",";

        private WorldData _worldData;
        private string _csv;

        public CsvToWorldDataBuilder(string csv)
        {
            _csv = csv;
        }

        public WorldData Build()
        {
            if (_worldData != null)
                return _worldData;

            _worldData = BuildInternal();
            return _worldData;
        }

        private WorldData BuildInternal()
        {
            var descriptions = SplitCsvToTileDescriptions(_csv);

            var tiles = TurnDescriptionsIntoTiles(descriptions);

            return new WorldData()
            {
                Tiles = tiles
            };
        }

        private static string[,] SplitCsvToTileDescriptions(string csv)
        {
            var lines = csv.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            int emptyLines = 0;
            foreach (var line in lines.Reverse())
            {
                if (string.IsNullOrEmpty(line))
                    emptyLines++;
                else
                    break;
            }
            var height = lines.Length - emptyLines;
            var width = lines.Max(l => l.Split(TileSeparator, StringSplitOptions.None).Length);

            var tileDescriptions = new string[width, height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    tileDescriptions[j, i] = string.Empty;
                }
                var t = lines[i].Split(TileSeparator, StringSplitOptions.None);

                for (int j = 0; j < width; j++)
                {
                    tileDescriptions[j, i] = t[j];
                }
            }

            return tileDescriptions;
        }

        private static Tile[,] TurnDescriptionsIntoTiles(string[,] descriptions)
        {
            var width = descriptions.GetLength(0);
            var height = descriptions.GetLength(1);
            var tiles = new Tile[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tiles[i, j] = DescriptionToTileConverter.FromDescription(descriptions[i, j], i, j);
                }
            }
            return tiles;
        }
    }
}