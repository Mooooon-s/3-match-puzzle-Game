using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance = null;
    
    public GameObject tile;
    public int xSize,ySize;
    public GameObject[,] tiles;

    void Awake()
    {
        Instance = GetComponent<GridManager>();
        Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
        Debug.Log(offset);
        CreateGrid(offset.x,offset.y);
    }

    private void CreateGrid(float offsetX, float offsetY)
    {
        tiles = new GameObject[xSize, ySize];

        Vector3 StartPos = transform.position;

        for(int i = 0; i < xSize; i++)
        {
            for(int j = 0; j < ySize; j++)
            {
                GameObject NewTile = 
                    Instantiate<GameObject>(tile, new Vector3(StartPos.x+(i*offsetX), StartPos.y+(j*offsetY)),tile.transform.rotation);
                tiles[i, j]=NewTile;
            }
        }

        /*for(int i = 0;i < xSize; i++)
        {
            for(int j= 0; j < ySize; j++)
            {
                Debug.Log(tiles[i,j].name);
            }
        }*/
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
