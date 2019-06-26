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
    public GameObject spawnGuy;
    public Transform playerTf;
    public int maxEnemies;
    public int totalEnemies;
    public int totalRooms;

    //to-do
    //  make doors variable size
    List<Vector2Int> RoomGenerator(Vector2Int xyOffset, Vector2Int xyCorner, Vector2Int sizeBoundary, Vector2Int xySizes)
    {
        //init basic variables
        xyOffset.x += (xySizes.x * xyCorner.x) + (1 * xyCorner.x);
        xyOffset.y += (xySizes.y * xyCorner.y) + (1 * xyCorner.y);
        Vector2Int xyRoomSize = new Vector2Int(Random.Range(sizeBoundary.x, sizeBoundary.y), Random.Range(sizeBoundary.x, sizeBoundary.y));
        if ((xyRoomSize.x < 0) || (xyRoomSize.y < 0))
            return new List<Vector2Int> { new Vector2Int(0, 0), new Vector2Int(0, 0) };

        //set up noise
        int noiseDirection = Random.Range(-10, 10); //the noiseX variables add variation to room placement
        if (noiseDirection < 0)
            noiseDirection = -1;
        if (noiseDirection > 0)
            noiseDirection = 1;
        int noiseOffset = 0;
        if(xyCorner.x != 0) // if horiz room
            {noiseOffset = Random.Range(1, xySizes.y - xyRoomSize.y) * noiseDirection;}
        else if (xyCorner.y != 0) // if vert room
            {noiseOffset = Random.Range(1, xySizes.x - xyRoomSize.x) * noiseDirection;}
        xyOffset.x += (xyRoomSize.x * xyCorner.x) + (noiseOffset * xyCorner.y);
        xyOffset.y += (xyRoomSize.y * xyCorner.y) + (noiseOffset * xyCorner.x);

        //spawn enemies
        if (totalEnemies < maxEnemies)
        {
            Instantiate(spawnGuy, new Vector3(xyOffset.x, xyOffset.y, 0), Quaternion.identity);
            totalEnemies++;
        }

        //make wall outline
        for (int j = -1; j < 2; j += 2)
        {
            for (int i = -xyRoomSize.x; i < xyRoomSize.x; i++)
            {
                Vector3Int pos;
                //top/bottom tiles
                pos = new Vector3Int(i + xyOffset.x, j * xyRoomSize.y + (j == -1 ? -1 : 0) + xyOffset.y, 0);
                if ((wallMap.GetTile(pos) == null) && (emptyMap.GetTile(pos) == null))
                    { wallMap.SetTile(pos, wallTile); }
            }
            for(int i = -xyRoomSize.y; i < xyRoomSize.y; i++)
            {
                Vector3Int pos;
                //side tiles
                pos = new Vector3Int(j * xyRoomSize.x + (j == -1 ? -1 : 0) + xyOffset.x, i + xyOffset.y, 0);
                if ((wallMap.GetTile(pos) == null) && (emptyMap.GetTile(pos) == null))
                { wallMap.SetTile(pos, wallTile); }
            }
            
            //corners
            for (int i = -1; i < 2; i += 2)
            {
                Vector3Int cornerVec = new Vector3Int(xyOffset.x + (xyRoomSize.x * j) - (j == -1 ? 1 : 0),
                                                    xyOffset.y + (xyRoomSize.y * i) - (i == -1 ? 1 : 0), 0);
                if((wallMap.GetTile(cornerVec) == null) && (emptyMap.GetTile(cornerVec) == null))
                    wallMap.SetTile(cornerVec, wallTile2);
            }
        }
        for(int i = -xyRoomSize.x; i < xyRoomSize.x; i++)
        {
            for(int j = -xyRoomSize.y; j < xyRoomSize.y; j++)
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
            doorLoc = Random.Range(yLower, yUpper);
            Vector3Int doorVec = new Vector3Int(xOffset, doorLoc, 0);
            wallMap.SetTile(doorVec, null);
            //make doors bigger than 1 sometimes
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
            doorLoc = Random.Range(xLower, xUpper);
            wallMap.SetTile(new Vector3Int(doorLoc, yOffset, 0), null);
        }
        return new List<Vector2Int> {xyOffset, xyRoomSize};
    }

    void worldGenerator(Vector2Int xyOffset, Vector2Int sizeBoundary, Vector2Int xySize, int totalRooms)
    {
        Vector2Int lastDir = new Vector2Int(0, 0);
        Vector2Int randomDir = new Vector2Int(0, 0);
        int maxAttempts = 3;
        while (totalRooms > 0)
        {
            int currAttempts = 0;
            while ((lastDir == randomDir) && (currAttempts < maxAttempts))
            {
                randomDir = new Vector2Int(0, 0);
                randomDir.x = Random.Range(-1, 1);
                if (randomDir.x == 0)
                {
                    randomDir.y = Random.Range(-1, 1);
                    if (randomDir.y == 0)
                        randomDir.y = 1;
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
        worldGenerator(savedData[0], new Vector2Int(2, 12), savedData[1], totalRooms);
    }

    // Start is called before the first frame update
    void Start()
    {
        totalEnemies = 0;
        maxEnemies = 10;
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

            //var EnemyObj = GameObject.FindGameObjectsWithTag("BasicGuy");
            //foreach (GameObject item in EnemyObj)
            //    {Destroy(item);}
            //totalEnemies = 0;

            worldGeneratorHelper(playerTileCoord.x, playerTileCoord.y);
        }
    }
}