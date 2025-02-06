using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    public enum BlockType
    {
        Empty,
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
    public GameBlock[,] Blocks;
    public Vector2 offSet;


    public List<Sprite> list = new List<Sprite>();

    void Awake()
    {
        offSet = Grid.GetComponent<SpriteRenderer>().bounds.size;

        blockPrefabDict = new Dictionary<BlockType, GameObject>();
        for(int i = 0; i < blockPrefabs.Length; i++)
        {
            if (!blockPrefabDict.ContainsKey(blockPrefabs[i]._type))
            {
                blockPrefabDict.Add(blockPrefabs[i]._type, blockPrefabs[i]._block);
            }
        }
        CreateGrid(offSet);
    }

    private void CreateGrid(Vector2 _offset)
    {
        Blocks = new GameBlock[xSize, ySize];

        Vector3 StartPos = transform.position;

        for(int i = 0; i < xSize; i++)
        {
            for(int j = 0; j < ySize; j++)
            {
                //Create Grid
                GameObject NewGrid = 
                    Instantiate<GameObject>(Grid, GetPositionVec3(i,j,_offset), Quaternion.identity);
            }
        }

        for(int i = 0;i < xSize; i++)
        {
            for(int j = 0;j<ySize; j++)
            {
                //Create Block
                SpawnNewBlock(i, j, offSet, BlockType.Empty);
            }
        }
    }

    public Vector2 GetPosition(float _x, float _y,Vector2 _offset)
    {
        return new Vector2(transform.position.x - xSize/2.0f +_x*(_offset.x), transform.position.y + ySize/2.0f -_y*(_offset.y));
    }

    public Vector3 GetPositionVec3(float _x, float _y, Vector2 _offset)
    {
        return new Vector3(transform.position.x - xSize / 2.0f + _x * (_offset.x), transform.position.y + ySize / 2.0f - _y * (_offset.y),100);
    }

    public GameBlock SpawnNewBlock(int _x, int _y, Vector2 _offset,BlockType _type)
    {
        GameObject newBlock = Instantiate<GameObject>(blockPrefabDict[_type],GetPositionVec3(_x,_y,offSet),Quaternion.identity);

        newBlock.transform.parent = transform;

        Blocks[_x, _y] = newBlock.GetComponent<GameBlock>();
        Blocks[_x, _y].Init(_x, _y, this, _type);

        return Blocks[_x, _y];
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
