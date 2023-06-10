using System.Collections.Generic;

namespace TopDownShooter.WorldGeneration
{
    public enum TileType
    {
        Empty,
        Floor,
        Wall
    }

    public class Tile
    {
        public TileType TileType { get; set; }

        public IEnumerable<TileModifier> Modifiers { get; set; }
    }

    public class TileModifier
    {
    }

    public class EnemySpawnModifier
    {
    }

    public class PlayerSpawnModifier
    {
    }
}