using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceBlocks : MonoBehaviour
{
    public Tile highlightTile;
    public Tile highlightTile2;
    public Tilemap highlightMap;
    // Start is called before the first frame update
    void Start()
    {
        //init starting platform
        for (int i = -2; i < 2; i++)
        {
            Vector3Int pos = new Vector3Int(i, -2, 0);
            highlightMap.SetTile(pos, highlightTile);
        }
        //generate map
        for(int i = -10; i < 9; i++)
        {
            Vector3Int pos = new Vector3Int(i, -2, 0);
            if(highlightMap.GetTile(pos) == null)
            {
                highlightMap.SetTile(pos, highlightTile2);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
