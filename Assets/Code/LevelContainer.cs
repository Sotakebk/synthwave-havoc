using System.Linq;
using TopDownShooter.World.CSV;
using TopDownShooter.World.Data;
using UnityEngine;

namespace TopDownShooter
{
    public class LevelContainer : MonoBehaviour
    {
        [SerializeField] private TextAsset[] _levelCsvs;

        public WorldData[] GetLevels()
        {
            return _levelCsvs.Select(csv => new CsvToWorldDataBuilder(csv.text).Build()).ToArray();
        }
    }
}