using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager instance=null;
    public GridManager grid = GridManager.Instance;

    List<Sprite> sprites = new List<Sprite>();

    GameObject block;
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

        Debug.Log(startPos);

        for(int i = 0; i < getSize().x; i++)
        {
            for(int j = 0 ; j < getSize().y; j++)
            {
                Debug.Log("DebugGetGridSize");
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
