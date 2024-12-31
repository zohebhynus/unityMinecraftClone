using UnityEngine;
using System.Collections.Generic;


public class World : MonoBehaviour
{
    public Transform Player;
    public Vector3 SpawnPoint;

    public Material material;
    public BlockType[] blockTypes;

    public Chunk[,] allChunks = new Chunk[VoxelData.WorldWidthInChunks, VoxelData.WorldWidthInChunks];

    private Vector2Int playerLastChunkCoord;
    private List<Vector2Int> activeChunkCoords = new List<Vector2Int>();

    private void Start()
    {
        Player.position = SpawnPoint;
        playerLastChunkCoord = getChunkCoordFromVector3(Player.transform.position);
        updateActiveChunks();
    }

    private void Update()
    {
        if(!getChunkCoordFromVector3(Player.transform.position).Equals(playerLastChunkCoord)) 
        {
            updateActiveChunks();
        }
    }

    private void updateActiveChunks()
    {
        int playerChunkX = Mathf.FloorToInt(Player.position.x / VoxelData.ChunkWidth);
        int playerChunkZ = Mathf.FloorToInt(Player.position.z / VoxelData.ChunkWidth);

        List<Vector2Int> previousActiveChunksCoords = new List<Vector2Int>(activeChunkCoords);

        for(int x = playerChunkX - VoxelData.ViewDistanceInChunks / 2; x < playerChunkX + VoxelData.ViewDistanceInChunks/2; x++) 
        {
            for(int z = playerChunkZ - VoxelData.ViewDistanceInChunks / 2; z < playerChunkZ + VoxelData.ViewDistanceInChunks / 2; z++)
            {
                Vector2Int chunkCoord = new Vector2Int(x, z);
                if (IsChunkCoordInWorld(chunkCoord))
                {
                    if (allChunks[x, z] == null)
                    {
                        CreateChunk(chunkCoord);
                        activeChunkCoords.Add(chunkCoord);
                    }
                    else if (!allChunks[x, z].isActive)
                    {
                        allChunks[x, z].isActive = true;
                        activeChunkCoords.Add(chunkCoord);
                    }

                    for(int i = 0; i < previousActiveChunksCoords.Count; i++)
                    {
                        if (previousActiveChunksCoords[i].x == x && previousActiveChunksCoords[i].y == z)
                        {
                            previousActiveChunksCoords.RemoveAt(i);
                        }
                    }
                }
            }
        }

        foreach(Vector2Int chunkCoord in previousActiveChunksCoords)
        {
            allChunks[chunkCoord.x, chunkCoord.y].isActive = false;
        }
    }

    private void CreateChunk(Vector2Int chunkCoord)
    {
        allChunks[chunkCoord.x, chunkCoord.y] = new Chunk(chunkCoord, this);
        allChunks[chunkCoord.x, chunkCoord.y].ChunkMeshFunctions();
    }

    private Vector2Int getChunkCoordFromVector3(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(position.z / VoxelData.ChunkWidth);

        return new Vector2Int(x, z);
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
