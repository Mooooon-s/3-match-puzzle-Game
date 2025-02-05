using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager instance=null;
    public GridManager grid = GridManager.Instance;

    List<Sprite> sprites = new List<Sprite>();

    public GameObject block;
    GameObject[,] blocks;


    Vector2Int getSize()
    {
        Vector2Int gridSize = new Vector2Int(grid.xSize, grid.ySize);

        return gridSize;
    }

    private void Awake()
    {
        instance = GetComponent<TileManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Vector3 startPos = grid.GetComponent<Transform>().position;

        UnityEngine.Vector3 offset = grid.Grid.GetComponent<SpriteRenderer>().bounds.size;


        blocks = new GameObject[grid.xSize,grid.ySize];

        for(int i = 0; i < getSize().x; i++)
        {
            for(int j = 0 ; j < getSize().y; j++)
            {
                GameObject NewBlock 
                    = Instantiate(block, new UnityEngine.Vector3(startPos.x+(i* offset.x), startPos.y+(j* offset.y)),block.transform.rotation);
                blocks[i,j] = NewBlock;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
