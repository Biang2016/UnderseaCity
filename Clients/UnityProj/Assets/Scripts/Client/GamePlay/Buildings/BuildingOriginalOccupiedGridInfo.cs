using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;

public class BuildingOriginalOccupiedGridInfo : IClone<BuildingOriginalOccupiedGridInfo>
{
    public List<GridPos> BuildingOccupiedGridPositionList = new List<GridPos>();
    public List<GridPos> BuildingAllSlotLocalPositionsList = new List<GridPos>();

    public BuildingOriginalOccupiedGridInfo Clone()
    {
        BuildingOriginalOccupiedGridInfo info = new BuildingOriginalOccupiedGridInfo();
        info.BuildingOccupiedGridPositionList = BuildingOccupiedGridPositionList.Clone();
        info.BuildingAllSlotLocalPositionsList = BuildingAllSlotLocalPositionsList.Clone();
        return info;
    }
}