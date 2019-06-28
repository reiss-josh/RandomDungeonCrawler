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
    public Transform playerTf;
    public long totalRooms;

    //to-do
    //  make rooms actually have things in them
    List<Vector2Int> RoomGenerator(Vector2Int xyOffset, Vector2Int xyCorner, Vector2Int sizeBoundary, Vector2Int xySizes)
    {
        //init basic variables
        xyOffset.x += (xySizes.x * xyCorner.x) + (1 * xyCorner.x);
        xyOffset.y += (xySizes.y * xyCorner.y) + (1 * xyCorner.y);
        Vector2Int xyRoomSize = new Vector2Int(Random.Range(sizeBoundary.x, sizeBoundary.y + 1), Random.Range(sizeBoundary.x, sizeBoundary.y + 1));
        if ((xyRoomSize.x < 0) || (xyRoomSize.y < 0))
            return new List<Vector2Int> { new Vector2Int(0, 0), new Vector2Int(0, 0) };

        //set up noise
        int noiseDirection = Random.Range(-10, 10 + 1); //the noiseX variables add variation to room placement
        if (noiseDirection < 0)
            noiseDirection = -1;
        if (noiseDirection > 0)
            noiseDirection = 1;
        int noiseOffset = 0;
        if (xyCorner.x != 0) // if horiz room
        { noiseOffset = Random.Range(1, xySizes.y - xyRoomSize.y + 1) * noiseDirection; }
        else if (xyCorner.y != 0) // if vert room
        { noiseOffset = Random.Range(1, xySizes.x - xyRoomSize.x + 1) * noiseDirection; }
        xyOffset.x += (xyRoomSize.x * xyCorner.x) + (noiseOffset * xyCorner.y);
        xyOffset.y += (xyRoomSize.y * xyCorner.y) + (noiseOffset * xyCorner.x);

        //make wall outline
        for (int j = -1; j < 2; j += 2)
        {
            for (int i = -xyRoomSize.x; i < xyRoomSize.x; i++)
            {
                Vector3Int pos;
                //top/bottom tiles
                pos = new Vector3Int(i + xyOffset.x, j * xyRoomSize.y + (j == -1 ? -1 : 0) + xyOffset.y, 0);
                PlaceTileIfEmpty(pos, wallTile);
            }
            for (int i = -xyRoomSize.y; i < xyRoomSize.y; i++)
            {
                Vector3Int pos;
                //side tiles
                pos = new Vector3Int(j * xyRoomSize.x + (j == -1 ? -1 : 0) + xyOffset.x, i + xyOffset.y, 0);
                PlaceTileIfEmpty(pos, wallTile);
            }

            //corners
            for (int i = -1; i < 2; i += 2)
            {
                Vector3Int cornerVec = new Vector3Int(xyOffset.x + (xyRoomSize.x * j) - (j == -1 ? 1 : 0),
                                                    xyOffset.y + (xyRoomSize.y * i) - (i == -1 ? 1 : 0), 0);
                PlaceTileIfEmpty(cornerVec, wallTile2);
            }
        }
        for (int i = -xyRoomSize.x; i < xyRoomSize.x; i++)
        {
            for (int j = -xyRoomSize.y; j < xyRoomSize.y; j++)
            {
                Vector3Int pos = new Vector3Int(xyOffset.x + i, xyOffset.y + j, 0);
                emptyMap.SetTile(pos, emptyTile);
                if (wallMap.GetTile(pos) != null)
                    wallMap.SetTile(pos, null);
            }
        }
        //make Door
        int doorLoc;
        if (xyCorner.x != 0) //if we have a horizontally placed room
        {
            int xOffset = xyOffset.x - (xyCorner.x * xyRoomSize.x) - ((xyCorner.x == 1) ? 1 : 0);
            int yLower = 0;
            int yUpper = 0;
            Vector3Int savedVec = new Vector3Int(xOffset - xyCorner.x, 0, 0);
            for (int i = xyOffset.y - (xyRoomSize.y - 1); i < xyOffset.y + (xyRoomSize.y); i++)
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
                        i = xyOffset.y + xyRoomSize.y;
                }
            }
            doorLoc = Random.Range(yLower, yUpper + 1);
            Vector3Int doorVec = new Vector3Int(xOffset, doorLoc, 0);
            wallMap.SetTile(doorVec, null);
            emptyMap.SetTile(doorVec, emptyTile);
            int spareRoomDown = doorLoc - yLower;
            int spareRoomUp = yUpper - doorLoc;
            if ((spareRoomDown > 1) && (spareRoomUp > 1))
            {
                int doorOffset = 0;
                while (doorOffset == 0)
                {
                    doorOffset = Random.Range(-1, 2);
                    doorVec.y += doorOffset;
                }
            }
            else if (spareRoomUp > 1)
                doorVec.y += 1;
            else if (spareRoomDown > 1)
                doorVec.y -= 1;
            wallMap.SetTile(doorVec, null);
            emptyMap.SetTile(doorVec, emptyTile);
        }
        else if (xyCorner.y != 0) //if we have a vertically placed room
        {
            int yOffset = xyOffset.y - (xyCorner.y * xyRoomSize.y) - ((xyCorner.y == 1) ? 1 : 0);
            int xLower = 0;
            int xUpper = 0;
            Vector3Int savedVec = new Vector3Int(0, yOffset - xyCorner.y, 0);
            for (int i = xyOffset.x - (xyRoomSize.x - 1); i < xyOffset.x + (xyRoomSize.x); i++)
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
                        i = xyOffset.x + xyRoomSize.x;
                }
            }
            doorLoc = Random.Range(xLower, xUpper + 1);
            Vector3Int doorVec = new Vector3Int(doorLoc, yOffset, 0);
            wallMap.SetTile(doorVec, null);
            emptyMap.SetTile(doorVec, emptyTile);
            int spareRoomLeft = doorLoc - xLower;
            int spareRoomRight = xUpper - doorLoc;
            if ((spareRoomLeft > 1) && (spareRoomRight > 1))
            {
                int doorOffset = 0;
                while (doorOffset == 0)
                {
                    doorOffset = Random.Range(-1, 2);
                    doorVec.x += doorOffset;
                }
            }
            else if (spareRoomRight > 1)
                doorVec.x += 1;
            else if (spareRoomLeft > 1)
                doorVec.x -= 1;
            wallMap.SetTile(doorVec, null);
            emptyMap.SetTile(doorVec, emptyTile);
        }
        return new List<Vector2Int> { xyOffset, xyRoomSize };
    }

    void worldGenerator(Vector2Int xyOffset, Vector2Int sizeBoundary, Vector2Int xySize, long totalRooms)
    {
        Vector2Int lastDir = new Vector2Int(0, 0);
        Vector2Int randomDir = new Vector2Int(0, 0);
        int maxAttempts = 5;
        while (totalRooms > 0)
        {
            int currAttempts = 0;
            while ((lastDir == randomDir) && (currAttempts < maxAttempts))
            {
                randomDir = new Vector2Int(0, 0);
                while ((randomDir.x == 0) && (randomDir.y == 0))
                {
                    int xORy = Random.Range(0, 2);
                    if (xORy == 0)
                        randomDir.x = Random.Range(-1, 2);
                    else
                        randomDir.y = Random.Range(-1, 2);
                }
                currAttempts++;
            }
            List<Vector2Int> funcOutput = RoomGenerator(xyOffset, randomDir, sizeBoundary, xySize);
            xyOffset = funcOutput[0];
            xySize = funcOutput[1];
            randomDir *= -1;
            lastDir = randomDir;
            totalRooms--;
        }
    }

    void worldGeneratorHelper(int xOffset, int yOffset)
    {
        //generate starting room
        List<Vector2Int> savedData = RoomGenerator(new Vector2Int(xOffset, yOffset), new Vector2Int(0, 0), new Vector2Int(5, 5), new Vector2Int(0, 0));
        //start world generation
        worldGenerator(savedData[0], new Vector2Int(2, 6), savedData[1], totalRooms);
    }

    // Start is called before the first frame update
    void Start()
    {
        totalRooms = 50;
        worldGeneratorHelper(0, 0);
    }

    void Update()
    {
        bool keyReset = Input.GetButtonDown("Fire2");
        if (keyReset)
        {
            Vector3Int playerTileCoord = emptyMap.WorldToCell(playerTf.position);
            emptyMap.ClearAllTiles();
            wallMap.ClearAllTiles();

            worldGeneratorHelper(playerTileCoord.x, playerTileCoord.y);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPos = wallMap.WorldToCell(clickPos);
            RemoveBlock(gridPos);
        }
    }

    public void receiveRoomCount(string roomCount)
    {
        totalRooms = long.Parse(roomCount);
    }

    public bool PlaceTileIfEmpty(Vector3Int chosenPos, Tile chosenTile)
    {
        if ((wallMap.GetTile(chosenPos) == null) && (emptyMap.GetTile(chosenPos) == null))
        {
            wallMap.SetTile(chosenPos, chosenTile);
            return true;
        }
        else
            return false;
    }

    public void RemoveBlock(Vector3Int blockPos)
    {
        if (wallMap.GetTile(blockPos) != null)
        {
            wallMap.SetTile(blockPos, null);
            emptyMap.SetTile(blockPos, emptyTile);
            blockPos.x -= 1;
            blockPos.y -= 1;
            for (int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    PlaceTileIfEmpty(blockPos, wallTile);
                    blockPos.x++;
                }
                blockPos.x -= 3;
                blockPos.y++;
            }
        }
    }
}