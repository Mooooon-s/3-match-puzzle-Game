using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance = null;
    public enum BlockType
    {
        Normal,
        Active,
        Obstacle,
        End
    }

    [System.Serializable]
    public struct Block
    {
        public BlockType _type;
        public GameObject _block;

    }

    public Block[] blockPrefabs;

    private Dictionary<BlockType, GameObject> blockPrefabDict;
    public GameObject Grid;
    public int xSize,ySize;
    public GameObject[,] Blocks;


    public List<Sprite> list = new List<Sprite>();

    void Awake()
    {
        Instance = GetComponent<GridManager>();
        Vector2 offset = Grid.GetComponent<SpriteRenderer>().bounds.size;

        blockPrefabDict = new Dictionary<BlockType, GameObject>();
        for(int i = 0; i < blockPrefabs.Length; i++)
        {
            if (!blockPrefabDict.ContainsKey(blockPrefabs[i]._type))
            {
                blockPrefabDict.Add(blockPrefabs[i]._type, blockPrefabs[i]._block);
            }
        }
        CreateGrid(offset);
    }

    private void CreateGrid(Vector2 _offset)
    {
        Blocks = new GameObject[xSize, ySize];

        Vector3 StartPos = transform.position;

        for(int i = 0; i < xSize; i++)
        {
            for(int j = 0; j < ySize; j++)
            {
                //Create Grid
                GameObject NewGrid = 
                    Instantiate<GameObject>(Grid, GetPosition(i, j, _offset),Quaternion.identity);
            }
        }

        for(int i = 0;i < xSize; i++)
        {
            for(int j = 0;j<ySize; j++)
            {
                //Create Block
                GameObject NewBlock =
                    Instantiate<GameObject>(blockPrefabDict[BlockType.Normal], GetPosition(i, j, _offset), Quaternion.identity) ;
                NewBlock.name =NewBlock.name+ " (" + i + ", " + j+")";
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

    Vector2 GetPosition(float _x, float _y,Vector2 _offset)
    {
        return new Vector2(transform.position.x - xSize/2.0f +_x*(_offset.x), transform.position.y + ySize/2.0f -_y*(_offset.y));
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
