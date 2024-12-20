using UnityEngine;

public class World : MonoBehaviour
{
    public Material material;
    public BlockType[] blockTypes;

    private Chunk[,] allChunks = new Chunk[VoxelData.WorldWidthInChunks, VoxelData.WorldWidthInChunks];
    private Chunk chunk;

    private void Start()
    {
        GenerateWorld();
    }

    private void GenerateWorld() 
    {
        for(int x = 0; x < VoxelData.WorldWidthInChunks; x++) 
        {
            for(int y = 0; y < VoxelData.WorldWidthInChunks; y++)
            {
                CreateChunk(new Vector2Int(x, y));
            }
        }
    }

    private void CreateChunk(Vector2Int chunkCoord)
    {
        allChunks[chunkCoord.x, chunkCoord.y] = new Chunk(chunkCoord, this);
    }

}

[System.Serializable]
public class BlockType
{
    public string Name;
    public bool IsSolid;


    public int FrontFaceTextureID;
    public int BackFaceTextureID;
    public int LeftFaceTextureID;
    public int RightFaceTextureID;
    public int TopFaceTextureID;
    public int BottomFaceTextureID;

    public int GetTextureID(int faceIndex)
    {
        switch (faceIndex) 
        {
            case 0: return FrontFaceTextureID;
            case 1: return BackFaceTextureID;
            case 2: return LeftFaceTextureID;
            case 3: return RightFaceTextureID;
            case 4: return TopFaceTextureID;
            case 5: return BottomFaceTextureID;
            default: 
                Debug.Log("Invalid face index");
                return 0;
        }
    }
}
