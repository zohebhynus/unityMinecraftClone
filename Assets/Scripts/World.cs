using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using static UnityEditor.PlayerSettings;

public class World : MonoBehaviour
{
    public Transform Player;
    private Vector3 SpawnPoint;
    public int Seed;
    public BiomeAttributes biome;

    public Material material;
    public BlockType[] blockTypes;

    public Chunk[,] allChunks = new Chunk[VoxelData.WorldWidthInChunks, VoxelData.WorldWidthInChunks];

    private Vector2Int playerLastChunkCoord;
    public Vector2Int playerChunkCoord;

    private List<Vector2Int> activeChunkCoords = new List<Vector2Int>();
    private List<Vector2Int> chunksToCreate = new List<Vector2Int>();
    private bool isCreatingChunks;

    private void Start()
    {
        Random.InitState(Seed);
        SpawnPoint = new Vector3((VoxelData.WorldWidthInChunks * VoxelData.ChunkWidth) / 2.0f, VoxelData.ChunkHeight - 20.0f, (VoxelData.WorldWidthInChunks * VoxelData.ChunkWidth) / 2.0f);
        Player.position = SpawnPoint;
        playerChunkCoord = playerLastChunkCoord = getChunkCoordFromVector3(Player.transform.position);
        GenerateWorld();
    }

    private void Update()
    {
        playerChunkCoord = getChunkCoordFromVector3(Player.position);

        if (!playerChunkCoord.Equals(playerLastChunkCoord))
        {
            updateActiveChunks();
        }

        if(chunksToCreate.Count > 0 && !isCreatingChunks)
        {
            StartCoroutine("CreateChunks");
        }
    }

    private void GenerateWorld()
    {
        for (int x = playerChunkCoord.x - VoxelData.ViewDistanceInChunks / 2; x < playerChunkCoord.x + VoxelData.ViewDistanceInChunks / 2; x++)
        {
            for (int z = playerChunkCoord.y - VoxelData.ViewDistanceInChunks / 2; z < playerChunkCoord.y + VoxelData.ViewDistanceInChunks / 2; z++)
            {
                Vector2Int coords = new Vector2Int(x, z);
                allChunks[x, z] = new Chunk(coords, this, true);
                activeChunkCoords.Add(coords);
            }
        }
    }

    IEnumerator CreateChunks()
    {
        isCreatingChunks = true;
        while(chunksToCreate.Count > 0) 
        {
            allChunks[chunksToCreate[0].x, chunksToCreate[0].y].Init();
            chunksToCreate.RemoveAt(0);
            yield return null;
        }


        isCreatingChunks = false;
    }

    private void updateActiveChunks()
    {
        int playerChunkX = Mathf.FloorToInt(Player.position.x / VoxelData.ChunkWidth);
        int playerChunkZ = Mathf.FloorToInt(Player.position.z / VoxelData.ChunkWidth);
        playerLastChunkCoord = playerChunkCoord;

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
                        allChunks[x, z] = new Chunk(chunkCoord, this, false);
                        chunksToCreate.Add(chunkCoord);
                    }
                    else if (!allChunks[x, z].isActive)
                    {
                        allChunks[x, z].isActive = true;
                        
                    }
                    activeChunkCoords.Add(chunkCoord);

                    for (int i = 0; i < previousActiveChunksCoords.Count; i++)
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

    public bool IsVoxelInWorld(Vector3 position)
    {
        if (position.x >= 0 && position.x < VoxelData.WorldWidthInBlocks && position.y >= 0 && position.y < VoxelData.ChunkHeight && position.z >= 0 && position.z < VoxelData.WorldWidthInBlocks)
            return true;
        else
            return false;
    }

    public bool CheckForVoxel(float x, float y, float z)
    {
        return CheckForVoxel(new Vector3(x, y, z));
    }

    public bool CheckForVoxel(Vector3 position)
    {
        Vector2Int chunkCoord = GetChunkCoordFromVector3(position);

        //if(IsChunkCoordInWorld(chunkCoord) || position.y < 0 || position.y > VoxelData.ChunkHeight)
        if(!IsVoxelInWorld(position))
        {
            return false;
        }

        if (allChunks[chunkCoord.x, chunkCoord.y] != null && allChunks[chunkCoord.x, chunkCoord.y].isChunkMapPopulated)
        {
            return blockTypes[allChunks[chunkCoord.x, chunkCoord.y].GetVoxelFromGlobalVector3(position)].IsSolid;
        }

        return blockTypes[GetVoxel(position)].IsSolid;
    }

    public Vector2Int GetChunkCoordFromVector3(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x);
        int z = Mathf.FloorToInt(position.z);

        x /= VoxelData.ChunkWidth;
        z /= VoxelData.ChunkWidth;

        return new Vector2Int(x, z);
    }

    /*
     * Grass       : 0
     * Stone       : 1
     * Wood        : 2
     * Air         : 3
     * Bedrock     : 4
     * Dirt        : 5
     * Sand        : 6
     * Cobblestone : 7 
     */
    public byte GetVoxel(Vector3 position)
    {
        int yPosition = Mathf.FloorToInt(position.y);
        byte voxelValue = 0;
        // IMMUTABLE PASS
        {
            // If outside world, return air
            if (!IsVoxelInWorld(position))
                return 3;

            // If Bottom block of chunk, return bedrock
            if (yPosition == 0)
                return 4;
        }



        // BASIC TERRAIN PASS
        {
            int terrainHeight = Mathf.FloorToInt(biome.TerrainHeight * Noise.Get2DPerlinNoise(new Vector2(position.x, position.z), 500, biome.TerrainScale)) + biome.SolidGroundHeight;
            if(yPosition == terrainHeight)
            {
                voxelValue = 0;
            }
            else if(yPosition < terrainHeight && yPosition > terrainHeight - 4.0f)
            {
                voxelValue = 5;
            }
            else if(yPosition > terrainHeight)
            {
                // Air
                return 3;
            }
            else
            {
                voxelValue =  1;
            }
        }

        // SECOND PASS
        {
            if(voxelValue == 1)
            {
                foreach(Lode lode in biome.Lodes)
                {
                    if(yPosition > lode.MinHeight && yPosition < lode.MaxHeight)
                    {
                        if(Noise.Get3DPerlinNoise(position, lode.Offset, lode.Scale, lode.Threshold))
                        {
                            voxelValue = lode.BlockID;
                        }
                    }
                }
            }
        }
        return voxelValue;
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
