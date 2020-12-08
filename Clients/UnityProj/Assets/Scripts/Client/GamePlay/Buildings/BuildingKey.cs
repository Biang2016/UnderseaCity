using System;

public enum BuildingKey
{
    None = 0,
    PioneerHub = 1,
    OxygenGenerator = 2,
    SmallDome = 3,
    MediumDome = 4,
    LargeDome = 5,
    UnderseaApartment_Lower = 6,
    UnderseaApartment_Higher = 7,
    TreasureHunterHub = 8,
}

public enum TerrainType
{
    None,
    Gold_1,
    Gold_2,
    Gold_3,
    Rock,
}

[Serializable]
public struct BuildingCoverMaskGroup
{
    public BuildingCoverMask_Part1 Mask_Part1;
    public BuildingCoverMask_Part2 Mask_Part2;
    public BuildingCoverMask_Part3 Mask_Part3;

    public static BuildingCoverMaskGroup None = new BuildingCoverMaskGroup
    {
        Mask_Part1 = BuildingCoverMask_Part1.None,
        Mask_Part2 = BuildingCoverMask_Part2.None,
        Mask_Part3 = BuildingCoverMask_Part3.None,
    };

    public bool HasFlag(BuildingCoverMaskGroup target)
    {
        return Mask_Part1.HasFlag(target.Mask_Part1) && Mask_Part2.HasFlag(target.Mask_Part2) && Mask_Part3.HasFlag(target.Mask_Part3);
    }

    public static BuildingCoverMaskGroup operator |(BuildingCoverMaskGroup a, BuildingCoverMaskGroup b)
    {
        BuildingCoverMaskGroup res = new BuildingCoverMaskGroup();
        res.Mask_Part1 = a.Mask_Part1 | b.Mask_Part1;
        res.Mask_Part2 = a.Mask_Part2 | b.Mask_Part2;
        res.Mask_Part3 = a.Mask_Part3 | b.Mask_Part3;
        return res;
    }

    public static BuildingCoverMaskGroup operator &(BuildingCoverMaskGroup a, BuildingCoverMaskGroup b)
    {
        BuildingCoverMaskGroup res = new BuildingCoverMaskGroup();
        res.Mask_Part1 = a.Mask_Part1 & b.Mask_Part1;
        res.Mask_Part2 = a.Mask_Part2 & b.Mask_Part2;
        res.Mask_Part3 = a.Mask_Part3 & b.Mask_Part3;
        return res;
    }

    public static bool operator ==(BuildingCoverMaskGroup a, BuildingCoverMaskGroup b)
    {
        return a.Mask_Part1 == b.Mask_Part1 && a.Mask_Part2 == b.Mask_Part2 && a.Mask_Part3 == b.Mask_Part3;
    }

    public static bool operator !=(BuildingCoverMaskGroup a, BuildingCoverMaskGroup b)
    {
        return a.Mask_Part1 != b.Mask_Part1 || a.Mask_Part2 != b.Mask_Part2 || a.Mask_Part3 != b.Mask_Part3;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        BuildingCoverMaskGroup objMask = obj is BuildingCoverMaskGroup ? (BuildingCoverMaskGroup) obj : default;
        return this == objMask;
    }

    public override int GetHashCode()
    {
        return Mask_Part1.GetHashCode() + Mask_Part2.GetHashCode() + Mask_Part3.GetHashCode();
    }
}

[Flags]
public enum BuildingCoverMask_Part1
{
    None = 0,
    PioneerHubCover = 1 << 0,
    PioneerHubOccupy = 1 << 13,
    OxygenGeneratorCover = 1 << 1,
    OxygenGeneratorOccupy = 1 << 14,
    SmallDomeInside = 1 << 2,
    SmallDomeShell = 1 << 3,
    SmallDomAround = 1 << 4,
    SmallDomCover = SmallDomeInside | SmallDomeShell,
    MediumDomeInside = 1 << 5,
    MediumDomeShell = 1 << 6,
    MediumDomAround = 1 << 7,
    MediumDomCover = MediumDomeInside | MediumDomeShell,
    LargeDomeInside = 1 << 8,
    LargeDomeShell = 1 << 9,
    LargeDomAround = 1 << 10,
    LargeDomCover = LargeDomeInside | LargeDomeShell,
    DoomShell = SmallDomeShell | MediumDomeShell | LargeDomeShell,
    UnderseaApartment_LowerCover = 1 << 11,
    UnderseaApartment_LowerOccupy = 1 << 15,
    UnderseaApartment_HigherCover = 1 << 12,
    UnderseaApartment_HigherOccupy = 1 << 16,
    TreasureHunterHubCover = 1 << 17,
    TreasureHunterHubOccupy = 1 << 18,
}

[Flags]
public enum BuildingCoverMask_Part2
{
    None = 0,
    Gold_1 = 1 << 0,
    Gold_2 = 1 << 1,
    Gold_3 = 1 << 2,
    Gold = Gold_1| Gold_2 | Gold_3,
    Rock = 1 << 3,
}

[Flags]
public enum BuildingCoverMask_Part3
{
    None = 0,
}