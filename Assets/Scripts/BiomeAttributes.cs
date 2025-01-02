using UnityEngine;

[CreateAssetMenu(fileName = "BiomeAttributes", menuName = "Scriptable Objects/Biome Attributes")]
public class BiomeAttributes : ScriptableObject
{
    public string BiomeName;

    public int SolidGroundHeight;
    public int TerrainHeight;
    public float TerrainScale;

    public Lode[] Lodes;
}

[System.Serializable]
public class Lode
{
    public string LodeName;
    public byte BlockID;
    public int MinHeight;
    public int MaxHeight;
    public float Scale;
    public float Threshold;
    public float Offset;
}
