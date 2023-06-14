using System.Collections.Generic;

namespace TopDownShooter.World.Data
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
        public int X { get; set; }
        public int Y { get; set; }
        public IEnumerable<TileModifier> Modifiers { get; set; }
    }

    public class TileModifier
    {
    }

    public enum EnemyType
    {
        Follower,
        Orbiter
    }

    public class EnemySpawnModifier : TileModifier
    {
        public EnemyType EnemyType { get; set; }
        public int Count { get; set; }

        public EnemySpawnModifier(EnemyType type, int count)
        {
            EnemyType = type;
            Count = count;
        }
    }

    public class PlayerSpawnModifier : TileModifier
    {
    }
}