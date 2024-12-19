using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.PackageManager;
using UnityEngine;


public class Chunck : MonoBehaviour
{

    private MeshRenderer m_Renderer;
    private MeshFilter m_Filter;

    private BlockType[,,] chunkMap;
    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<int> m_Triangles = new List<int>();
    private List<Vector2> m_Uvs = new List<Vector2>();
    private int m_VertexIndex = 0;

    public int ChunkWidth = 1;
    private int ChunkWidthHalf;

    public int ChunkHeight = 1;
    private int ChunkHeightHalf;

    private World world;




    void Start()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        m_Filter = GetComponent<MeshFilter>();
        world = GameObject.Find("World").GetComponent<World>();

        ChunkWidthHalf = ChunkWidth / 2;
        ChunkHeightHalf = ChunkHeight / 2;

        GenerateChunkMap();
        CreateChunkVertexData();
        CreateChunkMesh();
        
    }

    void Update()
    {
        
    }

    void AddCubeVertex(Vector3 chunkPosition)
    {
        for(int i = 0; i < 6; i++)
        {
            if(!isSolidCube(chunkPosition + VoxelData.voxelNeighbours[i]))
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

    bool isSolidCube(Vector3 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;
        int z = (int)position.z;
        if(x < 0 || x >= ChunkWidth || y < 0 || y >= ChunkHeight || z < 0 || z >= ChunkWidth)
        {
            return false;
        }
        return chunkMap[(int)position.x, (int)position.y, (int)position.z].IsSolid;
    }

    void GenerateChunkMap()
    {
        chunkMap = new BlockType[ChunkWidth, ChunkHeight, ChunkWidth];

        for(int x = 0; x < ChunkWidth; x++)
        {
            for(int y = 0; y < ChunkHeight; y++) 
            {
                for(int z = 0; z < ChunkWidth; z++) 
                {
                    if(y < 1)
                    {
                        chunkMap[x, y, z] = world.blockTypes[1];
                    }
                    else if(y == ChunkHeight - 1)
                    {
                        chunkMap[x, y, z] = world.blockTypes[2];
                    }
                    else
                    {
                        chunkMap[x, y, z] = world.blockTypes[0];
                    }
                }
            }
        }
    }

    private void CreateChunkMesh()
    {

        Debug.Log(m_Vertices.Count);
        Debug.Log(m_Triangles.Count);

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

        for (int x = 0; x < ChunkWidth; x++)
        {
            for (int y = 0; y < ChunkHeight; y++)
            {
                for (int z = 0; z < ChunkWidth; z++)
                {
                    if (!chunkMap[x, y, z].IsSolid)
                    {
                        continue;
                    }

                    AddCubeVertex(new Vector3(x, y, z));
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

