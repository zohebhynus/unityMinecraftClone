using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.LightTransport;

public class Chunk
{

    private MeshRenderer m_Renderer;
    private MeshFilter m_Filter;
    private GameObject m_ChunkObject;

    public byte[,,] chunkMap;
    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<int> m_Triangles = new List<int>();
    private List<Vector2> m_Uvs = new List<Vector2>();
    private int m_VertexIndex = 0;

    private World m_World;

    public Vector2Int ChunkCoord;
    private bool m_IsActive;
    public bool isChunkMapPopulated = false;


    public Chunk(Vector2Int chunkCoord, World world, bool generateOnLoad)
    {
        m_World = world;
        ChunkCoord = chunkCoord;

        if(generateOnLoad)
        {
            Init();
        }
    }

    public void Init()
    {
        m_ChunkObject = new GameObject();
        // y coord is used as z
        m_ChunkObject.transform.position = new Vector3(ChunkCoord.x * VoxelData.ChunkWidth, 0.0f, ChunkCoord.y * VoxelData.ChunkWidth);

        m_Renderer = m_ChunkObject.AddComponent<MeshRenderer>();
        m_Filter = m_ChunkObject.AddComponent<MeshFilter>();

        m_ChunkObject.transform.SetParent(m_World.transform);
        m_Renderer.material = m_World.material;

        m_ChunkObject.name = ChunkCoord.x + ", " + ChunkCoord.y;

        GenerateChunkMap();
        UpdateChunkVertexData();
    }

    public bool isActive
    {
        get { return m_IsActive; }
        set 
        {
            m_IsActive = value;
            if(m_ChunkObject != null)
            {
                m_ChunkObject.SetActive(value); 
            }
        }
    }

    public byte GetVoxelFromGlobalVector3(Vector3 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;
        int z = (int)position.z;

        x -= Mathf.FloorToInt(m_ChunkObject.transform.position.x);
        z -= Mathf.FloorToInt(m_ChunkObject.transform.position.z);


        return chunkMap[x, y, z];
    }

    public void EditVoxel (Vector3 position, byte blockID)
    {
        int x = (int)position.x;
        int y = (int)position.y;
        int z = (int)position.z;

        x -= Mathf.FloorToInt(m_ChunkObject.transform.position.x);
        z -= Mathf.FloorToInt(m_ChunkObject.transform.position.z);

        chunkMap[x, y, z] = blockID;

        UpdateSurroundingVoxels(x, y, z);

        UpdateChunkVertexData();
    }

    void UpdateSurroundingVoxels(int x, int y, int z)
    {
        Vector3 thisVoxel = new Vector3(x, y, z);

        for (int i = 0; i < 6; i++)
        {
            Vector3 currentVoxel = thisVoxel + VoxelData.voxelNeighbours[i];

            if (!isWithinChunk(Mathf.FloorToInt(currentVoxel.x), Mathf.FloorToInt(currentVoxel.y), Mathf.FloorToInt(currentVoxel.z)))
            {
                m_World.GetChunkFromVector3(currentVoxel + Position).UpdateChunkVertexData();
            }
        }
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

                byte blockId = chunkMap[(int)chunkPosition.x, (int)chunkPosition.y, (int)chunkPosition.z];
                AddTexture(m_World.blockTypes[blockId].GetTextureID(i));

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
        if(isWithinChunk(x, y, z))
        {
            return m_World.blockTypes[chunkMap[x, y, z]].IsSolid;
        }
        else
        {
            return m_World.CheckForVoxel(position + Position);
        }
        
    }


    bool isWithinChunk(int x, int y, int z)
    {
        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z >  VoxelData.ChunkWidth - 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void GenerateChunkMap()
    {
        chunkMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

        for(int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for(int y = 0; y < VoxelData.ChunkHeight; y++) 
            {
                for(int z = 0; z < VoxelData.ChunkWidth; z++) 
                {
                    chunkMap[x, y, z] = m_World.GetVoxel(new Vector3(x, y, z) + Position);             
                }
            }
        }
        isChunkMapPopulated = true;
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

    void UpdateChunkVertexData()
    {
        ClearChunkVertexData();

        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.ChunkHeight; y++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    if (m_World.blockTypes[chunkMap[x, y, z]].IsSolid)
                    {
                        AddVoxelVertex(new Vector3(x, y, z));
                    }
                }
            }
        }
        CreateChunkMesh();
    }

    void ClearChunkVertexData()
    {
        m_VertexIndex = 0;
        m_Vertices.Clear();
        m_Triangles.Clear();
        m_Uvs.Clear();
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

