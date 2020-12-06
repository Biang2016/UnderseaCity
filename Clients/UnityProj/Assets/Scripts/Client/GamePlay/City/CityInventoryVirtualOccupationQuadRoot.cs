using System.Collections.Generic;
using UnityEngine;

public class CityInventoryVirtualOccupationQuadRoot : MonoBehaviour
{
    internal List<CityInventoryVirtualOccupationQuad> cityInventoryVirtualOccupationQuads = new List<CityInventoryVirtualOccupationQuad>();

    internal void Clear()
    {
        foreach (CityInventoryVirtualOccupationQuad quad in cityInventoryVirtualOccupationQuads)
        {
            Destroy(quad.gameObject);
        }

        cityInventoryVirtualOccupationQuads.Clear();
    }
}