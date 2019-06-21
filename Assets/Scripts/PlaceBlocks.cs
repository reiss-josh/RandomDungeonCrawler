using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceBlocks : MonoBehaviour
{
    public Tile wallTile;
    public Tile wallTile2;
    public Tile emptyTile;
    public Tilemap emptyMap;
    public Tilemap wallMap;

    //NEED TO ADD A CHECK TO DECREASE ROOM SIZE UNTIL FITS
    int chooseRoomSize(Vector2Int Offset, int lowerBound, int upperBound)
    {
        //Debug.Log(Offset);
        Vector3Int pos;
        int savedMax = 0;
        for(int currSize = 2; currSize < upperBound; currSize++)
        {
            pos = new Vector3Int(Offset.x - currSize, Offset.y + (currSize - 1), 0);
            for (int i = 0; i < (currSize * 2); i++)
            {
                for(int j = 0; j < (currSize * 2); j++)
                {                   
                    //Debug.Log(new Vector2Int(pos.x + i, pos.y-j));
                    if(wallMap.GetTile(new Vector3Int(pos.x + i, pos.y - j, 0)) != null)
                    {
                        savedMax = currSize;
                        i = (upperBound * 2);
                        j = (upperBound * 2);
                    }
                }
            }
        }
        upperBound = savedMax;
        if (upperBound < 0)
            return -1;
        if (upperBound < lowerBound)
            return upperBound;
        return Random.Range(lowerBound, upperBound);
    }

    Vector3Int RoomGenerator(Vector3Int xySize, Vector2Int xyCorner, int lowerBound, int upperBound)
    {
        Vector2Int xyOffset = new Vector2Int(xySize.x, xySize.y); //this variable acts as a holder for our room's origin
        int passedSize = xySize.z;
        xyOffset.x += (passedSize * xyCorner.x) + (1 * xyCorner.x);
        xyOffset.y += (passedSize * xyCorner.y) + (1 * xyCorner.y);
        int roomSize = chooseRoomSize(xyOffset, lowerBound, upperBound); //here, we randomly select our room size.
        if (roomSize < 0)
            return new Vector3Int(0, 0, -1);
        int noiseDirection = Random.Range(-10, 10); //the noiseX variables add variation to room placement
        if (noiseDirection < 0)
            noiseDirection = -1;
        if (noiseDirection > 0)
            noiseDirection = 1;
        int noiseOffset = Random.Range(0, passedSize - roomSize) * noiseDirection;
        xyOffset.x += (roomSize * xyCorner.x) + (noiseOffset * xyCorner.y);
        xyOffset.y += (roomSize * xyCorner.y) + (noiseOffset * xyCorner.x);

        //make wall outline
        for (int j = -1; j < 2; j += 2)
        {
            for (int i = -roomSize; i < roomSize; i++)
            {
                Vector3Int pos;
                //top/bottom tiles
                pos = new Vector3Int(i + xyOffset.x, j * roomSize + (j == -1 ? -1 : 0) + xyOffset.y, 0);
                if (wallMap.GetTile(pos) == null)
                    {wallMap.SetTile(pos, wallTile);}
                pos.y = pos.y - j;
                emptyMap.SetTile(pos, emptyTile);

                //side tiles
                pos = new Vector3Int(j * roomSize + (j == -1 ? -1 : 0) + xyOffset.x, i + xyOffset.y, 0);
                if (wallMap.GetTile(pos) == null)
                    {wallMap.SetTile(pos, wallTile);}
                pos.x = pos.x - j;
                emptyMap.SetTile(pos, emptyTile);
            }
            //corners
            for (int i = -1; i < 2; i += 2)
            {
                wallMap.SetTile(new Vector3Int(xyOffset.x + (roomSize*j) - (j == -1 ? 1:0),
                                                    xyOffset.y + (roomSize*i) - (i == -1 ? 1:0), 0),
                                                    wallTile2);
            }
        }
        //make Door
        int doorLoc;
        if (xyCorner.x != 0)
        {
            int xOffset = xyOffset.x - (xyCorner.x * roomSize) - ((xyCorner.x == 1) ? 1:0);
            int yLower = 0;
            int yUpper = 0;
            Vector3Int savedVec = new Vector3Int(xOffset - xyCorner.x, 0, 0);
            for(int i = xyOffset.y - (roomSize - 1); i < xyOffset.y + (roomSize); i++)
            {
                savedVec.y = i;
                if (yLower == 0)
                {
                    if (emptyMap.GetTile(savedVec) != null)
                        yLower = i;
                    else
                        continue;
                }
                else
                {
                    if (emptyMap.GetTile(savedVec) != null)
                        yUpper = i;
                    else
                        i = xyOffset.y + roomSize;
                }
            }
            doorLoc = Random.Range(yLower, yUpper);
            wallMap.SetTile(new Vector3Int(xOffset, doorLoc, 0), null);
        }
        else if (xyCorner.y != 0)
        {
            int yOffset = xyOffset.y - (xyCorner.y * roomSize) - ((xyCorner.y == 1) ? 1:0);
            int xLower = 0;
            int xUpper = 0;
            Vector3Int savedVec = new Vector3Int(0, yOffset - xyCorner.y, 0);
            for (int i = xyOffset.x - (roomSize - 1); i < xyOffset.x + (roomSize); i++)
            {
                savedVec.x = i;
                if (xLower == 0)
                {
                    if (emptyMap.GetTile(savedVec) != null)
                        xLower = i;
                    else
                        continue;
                }
                else
                {
                    if (emptyMap.GetTile(savedVec) != null)
                        xUpper = i;
                    else
                        i = xyOffset.x + roomSize;
                }
            }
            doorLoc = Random.Range(xLower, xUpper);
            wallMap.SetTile(new Vector3Int(doorLoc, yOffset, 0), null);
        }

        return new Vector3Int(xyOffset.x, xyOffset.y, roomSize);
    }

    void worldGenerator(Vector3Int xySize, int lowerBound, int UpperBound, int totalRooms)
    {
        Vector2Int lastDir = new Vector2Int(0, 0);
        Vector2Int randomDir = new Vector2Int(0, 0);
        while (totalRooms > 0)
        {
            while(lastDir == randomDir)
            {
                randomDir = new Vector2Int(0, 0);
                randomDir.x = Random.Range(-1, 1);
                if (randomDir.x == 0)
                {
                    randomDir.y = Random.Range(-1, 1);
                    if (randomDir.y == 0)
                        randomDir.y = 1;
                }
            }
            xySize = RoomGenerator(xySize, randomDir, lowerBound, UpperBound);
            randomDir *= -1;
            lastDir = randomDir;
            totalRooms--;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //init starting platform
        for (int i = -2; i < 2; i++)
        {
            Vector3Int pos = new Vector3Int(i, -2, 0);
            wallMap.SetTile(pos, wallTile);
        }
        //generate map border
        Vector3Int savedData = RoomGenerator(new Vector3Int(0,0,0), new Vector2Int(0,0), 5, 5);

        worldGenerator(savedData, 2, 12, 50);
        /*
        Vector3Int tempSave = RoomGenerator(new Vector3Int(0,0, savedData.z), new Vector2Int(0, 1), 3, 5);
        RoomGenerator(tempSave, new Vector2Int(-1, 0), 3, 5);
        tempSave = RoomGenerator(new Vector3Int(0,0, savedData.z), new Vector2Int(-1, 0), 3, 5);
        //ughhhhhhhhhhhhh
        //Debug.Log(RoomGenerator(tempSave, new Vector2Int(0, 1), 3, 5));
        for (int i = 0; i < 25; i++)
            {savedData = RoomGenerator(savedData, new Vector2Int(1, 0), 2, Mathf.Min(savedData.z + 4, 12));}
        for (int i = 0; i < 25; i++)
            {savedData = RoomGenerator(savedData, new Vector2Int(0,-1), 2, Mathf.Min(savedData.z + 4, 12));}
        */
    }
}
