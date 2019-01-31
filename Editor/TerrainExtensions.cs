using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class TerrainExtensions
{
    public static void AddTexture(this Terrain t, Texture2D tex, int size)
    {
        List<SplatPrototype> protos = t.terrainData.splatPrototypes.ToList();

        SplatPrototype newSplatProto = new SplatPrototype();
        newSplatProto.texture = tex;
        newSplatProto.tileOffset = Vector2.zero;
        newSplatProto.tileSize = Vector2.one * size;
        newSplatProto.texture.Apply(true);

        protos.Add(newSplatProto);

        t.terrainData.splatPrototypes = protos.ToArray();
        newSplatProto = null;
        protos.Clear();
    }
}
