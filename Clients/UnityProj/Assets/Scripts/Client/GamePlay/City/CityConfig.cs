using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using Sirenix.OdinInspector;

public class CityConfig : IClone<CityConfig>
{
    [ReadOnly]
    [LabelText("建筑列表")]
    [TableList]
    public List<Config> BuildingList = new List<Config>();

    public struct Config
    {
        public string BuildingKey;
        public GridPosR GridPosR;
    }

    public CityConfig Clone()
    {
        return new CityConfig
        {
            BuildingList = BuildingList.Clone(),
        };
    }
}