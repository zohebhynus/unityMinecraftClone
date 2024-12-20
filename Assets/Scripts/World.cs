using UnityEngine;
using UnityEngine.LightTransport;

public class World : MonoBehaviour
{
    public Transform Player;
    public Vector3 SpawnPoint;

    public Material material;
    public BlockType[] blockTypes;

    public Chunk[,] allChunks = new Chunk[VoxelData.WorldWidthInChunks, VoxelData.WorldWidthInChunks];

    private void Start()
    {
        GenerateWorldData();
        GenerateWorldMesh();
    }

    private void GenerateWorldData() 
    {
        for(int x = 0; x < VoxelData.WorldWidthInChunks; x++) 
        {
            for(int y = 0; y < VoxelData.WorldWidthInChunks; y++)
            {
                CreateChunk(new Vector2Int(x, y));
            }
        }
    }

    private void GenerateWorldMesh()
    {
        for (int x = 0; x < VoxelData.WorldWidthInChunks; x++)
        {
            for (int y = 0; y < VoxelData.WorldWidthInChunks; y++)
            {
                allChunks[x, y].ChunkMeshFunctions();
            }
        }
    }

    private void CreateChunk(Vector2Int chunkCoord)
    {
        allChunks[chunkCoord.x, chunkCoord.y] = new Chunk(chunkCoord, this);
    }

    public bool IsChunkCoordInWorld(Vector2Int chunkCoord)
    {
        if (chunkCoord.x >= 0 && chunkCoord.x < VoxelData.WorldWidthInChunks && chunkCoord.y >= 0 && chunkCoord.y < VoxelData.WorldWidthInChunks)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public byte GetVoxel(Vector3 position)
    {
        if (position.y == (VoxelData.ChunkHeight/2))
        {
            return 0;
        }
        else if(position.y < VoxelData.ChunkHeight && position.y > VoxelData.ChunkHeight / 2)
        {
            return 3;
        }
        else
        {
            return 1;
        }
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
