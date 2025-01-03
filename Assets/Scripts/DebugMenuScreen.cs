using UnityEngine;
using UnityEngine.UI;

public class DebugMenuScreen : MonoBehaviour
{
    Text text;
    World world;

    float frameRate;
    float timer;

    int halfWorldWidthInBlocks;
    int halfWorldWidthInChunks;


    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
        text = GetComponent<Text>();

        halfWorldWidthInChunks = VoxelData.WorldWidthInChunks / 2;
        halfWorldWidthInBlocks = VoxelData.WorldWidthInBlocks / 2;
    }

    void Update()
    {
        string debugText = "Debug Menu";
        debugText += "\n";
        debugText += "Frame Rate : " + frameRate;
        debugText += "\n";
        debugText += "Player position : " + (Mathf.FloorToInt(world.Player.position.x) - halfWorldWidthInBlocks) + ", " + Mathf.FloorToInt(world.Player.position.y) + ", " + (Mathf.FloorToInt(world.Player.position.z) - halfWorldWidthInBlocks);
        debugText += "\n";
        debugText += "Player Chunk Coord : " + (world.playerChunkCoord.x - halfWorldWidthInChunks) + ", " + (world.playerChunkCoord.y - halfWorldWidthInChunks);

        text.text = debugText;

        if(timer > 1.0f)
        {
            frameRate = (int)(1.0f / Time.unscaledDeltaTime);
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
