using UnityEngine;




public static class VoxelData
{

    public static readonly int TextureAtlasSizeInBlocks = 4;
    public static float NormalizedBlockTextureSize
    {
        get { return 1.0f / (float)TextureAtlasSizeInBlocks; }
    }

    public static readonly Vector3[] voxelVerts = new Vector3[24]
        {
            // Front face
            new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(0, 1, 0),
            // Back face
            new Vector3(1, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(1, 1, 1),
            // Left face
            new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 1),
            // Right face
            new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 0),
            // Top face
            new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1), new Vector3(0, 1, 1),
            // Bottom face
            //new Vector3(1, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(1, 0, 0),

            new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 0, 0), new Vector3(0, 0, 0)
        };


    public static readonly int[] voxelIndices = new int[]
    {
            // Front face
            0, 3, 1, 1, 3, 2,
            // Back face
            4, 7, 5, 5, 7, 6,
            // Left face
            8, 11, 9, 9, 11, 10,
            // Right face
            12, 15, 13, 13, 15, 14,
            // Top face
            16, 19, 17, 17, 19, 18,
            // Bottom face
            20, 23, 21, 21, 23, 22
    };

    public static readonly Vector3[] voxelNeighbours = new Vector3[6]
    {
        // Front Face
        new Vector3(0, 0, -1),
        // Back Face
        new Vector3(0, 0, 1),
        // Left Face
        new Vector3(-1, 0, 0),
        // Right Face
        new Vector3(1, 0, 0),
        // Top Face
        new Vector3(0, 1, 0),
        // Bottom Face
        new Vector3(0, -1, 0)
    };

    public static readonly Vector2[] voxelFaceUvs = new Vector2[4]
    {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1)
    };

}
