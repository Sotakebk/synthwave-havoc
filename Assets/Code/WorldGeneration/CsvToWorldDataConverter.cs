using System;
using System.Collections.Generic;
using System.Linq;

namespace TopDownShooter.WorldGeneration
{
    public static class CsvToWorldDataConverter
    {
        private const string TileSeparator = ",";
        private const string ModifierSeparator = ":";
        private const string Wall = "WALL";
        private const string Floor = "FLOOR";

        public static IEnumerable<TileModifier> FromModifierDescription(IEnumerable<string> description)
        {
            return Array.Empty<TileModifier>();
        }

        public static Tile FromDescription(string description)
        {
            var values = description.Split(ModifierSeparator);

            var tile = new Tile()
            {
                TileType = TypeFromDescription(values[0]),
                Modifiers = FromModifierDescription(values.Skip(1))
            };

            return tile;
        }

        public static TileType TypeFromDescription(string description)
        {
            switch (description.ToUpper())
            {
                case Wall:
                    return TileType.Wall;

                case Floor:
                default:
                    return TileType.Floor;
            }
        }

        public static WorldData ConvertFromCsv(string csv)
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

            var textTiles = new string[height, width];
            for(int i = 0; i < height; i++)
            {
                for(int j = 0; j < width; j++)
                {
                    textTiles[i, j] = string.Empty;
                }
                var t = lines[i].Split(TileSeparator, StringSplitOptions.None);

                for (int j = 0; j < width; j++)
                {
                    textTiles[i, j] = t[j];
                }
            }

            var tiles = new Tile[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    tiles[i, j] = FromDescription(textTiles[i, j]);
                }
            }

            return new WorldData()
            {
                Tiles = tiles
            };
        }
    }
}