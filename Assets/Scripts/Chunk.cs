using System.Collections.Generic;
using UnityEngine;

public class Chunk
{

    private MeshRenderer m_Renderer;
    private MeshFilter m_Filter;
    private GameObject m_ChunkObject;

    private BlockType[,,] chunkMap;
    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<int> m_Triangles = new List<int>();
    private List<Vector2> m_Uvs = new List<Vector2>();
    private int m_VertexIndex = 0;

    private World m_World;

    public Vector2 ChunkCoord;


    public Chunk(Vector2 chunkCoord, World world)
    {
        ChunkCoord = chunkCoord;
        m_ChunkObject = new GameObject();
        // y coord is used as z
        m_ChunkObject.transform.position = new Vector3(ChunkCoord.x * VoxelData.ChunkWidth, 0.0f, ChunkCoord.y * VoxelData.ChunkWidth);

        m_Renderer = m_ChunkObject.AddComponent<MeshRenderer>();
        m_Filter = m_ChunkObject.AddComponent<MeshFilter>();
        m_World = world;

        m_ChunkObject.transform.SetParent(world.transform);
        m_Renderer.material = world.material;

        m_ChunkObject.name = ChunkCoord.x + ", " + ChunkCoord.y;

        GenerateChunkMap();
        CreateChunkVertexData();
        CreateChunkMesh();
    }

    public bool isActive
    {
        get { return m_ChunkObject.activeSelf; }
        set { m_ChunkObject.SetActive(value); }
    }

    Vector3 Position 
    {
        get { return m_ChunkObject.transform.position; }
    }

    void AddVoxelVertex(Vector3 chunkPosition)
    {
        for(int i = 0; i < 6; i++)
        {
            if(!isSolidVoxel(chunkPosition + VoxelData.voxelNeighbours[i]))
            {
                m_Vertices.Add(chunkPosition + VoxelData.voxelVerts[(i * 4) + 0]);
                m_Vertices.Add(chunkPosition + VoxelData.voxelVerts[(i * 4) + 1]);
                m_Vertices.Add(chunkPosition + VoxelData.voxelVerts[(i * 4) + 2]);
                m_Vertices.Add(chunkPosition + VoxelData.voxelVerts[(i * 4) + 3]);

                //m_Uvs.AddRange(VoxelData.voxelFaceUvs);
                AddTexture(chunkMap[(int)chunkPosition.x, (int)chunkPosition.y, (int)chunkPosition.z].GetTextureID(i));

                m_Triangles.Add(0 + m_VertexIndex);
                m_Triangles.Add(3 + m_VertexIndex);
                m_Triangles.Add(1 + m_VertexIndex);
                m_Triangles.Add(1 + m_VertexIndex);
                m_Triangles.Add(3 + m_VertexIndex);
                m_Triangles.Add(2 + m_VertexIndex);
                m_VertexIndex += 4;
            }
        }

    }

    bool isSolidVoxel(Vector3 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;
        int z = (int)position.z;
        if(x < 0 || x >= VoxelData.ChunkWidth || y < 0 || y >= VoxelData.ChunkHeight || z < 0 || z >= VoxelData.ChunkWidth)
        {
            return false;
        }
        return chunkMap[(int)position.x, (int)position.y, (int)position.z].IsSolid;
    }

    void GenerateChunkMap()
    {
        chunkMap = new BlockType[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

        for(int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for(int y = 0; y < VoxelData.ChunkHeight; y++) 
            {
                for(int z = 0; z < VoxelData.ChunkWidth; z++) 
                {
                    if(y < 1)
                    {
                        chunkMap[x, y, z] = m_World.blockTypes[1];
                    }
                    else if(y == VoxelData.ChunkHeight - 1)
                    {
                        chunkMap[x, y, z] = m_World.blockTypes[2];
                    }
                    else
                    {
                        chunkMap[x, y, z] = m_World.blockTypes[0];
                    }
                }
            }
        }
    }

    private void CreateChunkMesh()
    {

        // Create the mesh
        Mesh mesh = new Mesh();
        mesh.vertices = m_Vertices.ToArray();
        mesh.triangles = m_Triangles.ToArray();
        mesh.uv = m_Uvs.ToArray();

        // Recalculate the mesh normals for lighting
        mesh.RecalculateNormals();

        // Assign the mesh to the MeshFilter
        m_Filter.mesh = mesh;
    }

    void CreateChunkVertexData()
    {

        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.ChunkHeight; y++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    if (!chunkMap[x, y, z].IsSolid)
                    {
                        continue;
                    }

                    AddVoxelVertex(new Vector3(x, y, z));
                }
            }
        }
    }

    void AddTexture(int textureID)
    {
        float y = (textureID / VoxelData.TextureAtlasSizeInBlocks);
        float x = (textureID - (y * VoxelData.TextureAtlasSizeInBlocks));

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        m_Uvs.Add(new Vector2(x, y));
        m_Uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        m_Uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));
        m_Uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
    }
}

