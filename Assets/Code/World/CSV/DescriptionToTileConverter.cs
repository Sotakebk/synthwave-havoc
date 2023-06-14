using System.Collections.Generic;
using System.Linq;
using TopDownShooter.World.Data;

namespace TopDownShooter.World.CSV
{
    public static class DescriptionToTileConverter
    {
        private const string ModifierSeparator = ":";
        private const string DetailSeparator = "-";

        public static Tile FromDescription(string description, int x, int y)
        {
            var values = description.Split(ModifierSeparator);

            var tile = new Tile()
            {
                TileType = TypeFromDescription(values[0]),
                Modifiers = ModifiersFromDescriptions(values.Skip(1)),
                X = x,
                Y = y
            };

            return tile;
        }

        public static IEnumerable<TileModifier> ModifiersFromDescriptions(IEnumerable<string> descriptions)
        {
            return descriptions.Select(d => ModifierFromDescription(d)).Where(m => m != null).ToArray();
        }

        private const string Player = "PLAYER";
        private const string Enemy1 = "ENEMY1";
        private const string Enemy2 = "ENEMY2";
        private static TileModifier ModifierFromDescription(string description)
        {
            switch (description.ToUpper())
            {
                case Player:
                    return new PlayerSpawnModifier();

                case Enemy1:
                case Enemy2:
                    return EnemySpawnModifierFromDescription(description);

                default:
                    return null;
            }
        }

        private static EnemySpawnModifier EnemySpawnModifierFromDescription(string description)
        {
            var parts = description.Split(DetailSeparator);
            var type = EnemyTypeFromPart(parts[0]);
            var enemyCount = (parts.Length > 1) ? EnemyCountFromPart(parts[1]) : 1;

            return new EnemySpawnModifier(type, enemyCount);
        }

        private static EnemyType EnemyTypeFromPart(string part)
        {
            switch (part.ToUpper())
            {
                default:
                case Enemy1:
                    return EnemyType.Follower;
                case Enemy2:
                    return EnemyType.Orbiter;
            }
        }
        
        private static int EnemyCountFromPart(string part)
        {
            if (int.TryParse(part, out var result))
            {
                return result;
            }
            else
            {
                return 1;
            }
        }

        private const string Wall = "WALL";
        private const string Floor = "FLOOR";
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
    }
}