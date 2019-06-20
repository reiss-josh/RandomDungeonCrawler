using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceBlocks : MonoBehaviour
{
    public Tile highlightTile;
    public Tile highlightTile2;
    public Tilemap highlightMap;

    Vector3Int RoomGenerator(Vector3Int xySize, Vector2Int xyCorner, int lowerBound, int upperBound)
    {
        Vector2Int xyOffset = new Vector2Int(xySize.x, xySize.y);
        int passedSize = xySize.z;
        int roomSize = Random.Range(lowerBound, upperBound);
        int noiseDirection = Random.Range(-10, 10);
        if (noiseDirection < 0)
            noiseDirection = -1;
        if (noiseDirection > 0)
            noiseDirection = 1;
        int noiseOffset = Random.Range(0, passedSize - roomSize) * noiseDirection;
        xyOffset.x += (roomSize * xyCorner.x) + (passedSize * xyCorner.x) + (1 * xyCorner.x) + (noiseOffset * xyCorner.y);
        xyOffset.y += (roomSize * xyCorner.y) + (passedSize * xyCorner.y) + (1 * xyCorner.y) + (noiseOffset * xyCorner.x);

        //origin debug
        //highlightMap.SetTile(new Vector3Int(xyOffset.x, xyOffset.y, 0), highlightTile);

        //make wall outline
        for (int j = -1; j < 2; j += 2)
        {
            for (int i = -roomSize; i < roomSize; i++)
            {
                Vector3Int pos;
                int modifier;
                if (j < 0)
                    modifier = -1;
                else
                    modifier = 0;
                pos = new Vector3Int(i + xyOffset.x, j * roomSize + modifier + xyOffset.y, 0);

                if (highlightMap.GetTile(pos) == null)
                {
                    highlightMap.SetTile(pos, highlightTile2);
                }

                pos = new Vector3Int(j * roomSize + modifier + xyOffset.x, i + xyOffset.y, 0);
                if (highlightMap.GetTile(pos) == null)
                {
                    highlightMap.SetTile(pos, highlightTile2);
                }
            }
        }
        //make Door
        //if size 5 and door on right, we want (xyOffset.x + roomSize, xyOffset.y + roomSize-2) or xyOffset.y - (roomSize-1)
        int doorLoc;
        if (xyCorner.x != 0)
        {
            int xOffset = xyOffset.x - (xyCorner.x * roomSize) - ((xyCorner.x == 1) ? 1 : 0);
            doorLoc = Random.Range(xyOffset.y + (roomSize - 2), xyOffset.y - (roomSize - 1));
            highlightMap.SetTile(new Vector3Int(xOffset, doorLoc, 0), null);
        }
        else if (xyCorner.y != 0)
        {
            Debug.Log("unimplemented");
        }

        return new Vector3Int(xyOffset.x, xyOffset.y, roomSize);
    }

    // Start is called before the first frame update
    void Start()
    {
        //init starting platform
        for (int i = -2; i < 2; i++)
        {
            Vector3Int pos = new Vector3Int(i, -2, 0);
            highlightMap.SetTile(pos, highlightTile);
        }
        //generate map border
        Vector3Int savedData = RoomGenerator(new Vector3Int(0,0,0), new Vector2Int(0,0), 5, 5);

        RoomGenerator(new Vector3Int(0,0, savedData.z), new Vector2Int(0, 1), 3, 5);
        RoomGenerator(new Vector3Int(0,0, savedData.z), new Vector2Int(-1, 0), 3, 5);
        for (int i = 0; i < 5; i++)
        {
            savedData = RoomGenerator(savedData, new Vector2Int(1, 0), 3, 5);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
