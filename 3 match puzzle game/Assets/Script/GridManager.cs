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
    public float fillTime;
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
                NewGrid.gameObject.name = NewGrid.name + " (" + i + " , " + j + ") ";
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

        //test Code
        Destroy(Blocks[4, 4].gameObject);
        SpawnNewBlock(4,4, offSet, BlockType.Obstacle);

        Destroy(Blocks[1, 2].gameObject);
        SpawnNewBlock(1, 2, offSet, BlockType.Obstacle);

        StartCoroutine( Fill());
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
        GameObject newBlock = Instantiate<GameObject>(blockPrefabDict[_type], GetPosition(_x,_y,offSet),Quaternion.identity);

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

    public IEnumerator Fill()
    {
        while (FillStep()) {
            yield return new WaitForSeconds(fillTime);
        } ;
    }

    public bool FillStep()
    {
        bool movedPiece = false;


        //start from bottom
        for(int y = ySize-2; y >= 0; y--)
        {
            for(int x=0; x < xSize; x++)
            {
                GameBlock block = Blocks[x, y];
                //Check the Empty Block
                // if have Move Scrpt get true
                if (block.IsMoveable())
                {
                    //Check the below block
                    GameBlock belowBlock = Blocks[x, y+1];
                    if (belowBlock.Type == BlockType.Empty)
                    {
                        Destroy(belowBlock.gameObject);
                        //swap below block
                        block.MoveableComponent.Move(x, y+1,fillTime);
                        Blocks[x, y+1] = block;
                        SpawnNewBlock(x, y, offSet, BlockType.Empty);
                        movedPiece = true;
                    }
                    else //someting block in below
                    {
                        int side = CheckSide(x, y);
                        if(side != 0)
                        {
                            int DiagX = x + side;
                            if(DiagX>=0 && DiagX < xSize)
                            {
                                GameBlock DiagBlack = Blocks[DiagX, y+1];

                                if (DiagBlack.Type != BlockType.Empty) continue;
                                Destroy(DiagBlack.gameObject);
                                block.MoveableComponent.Move(DiagX, y+1,fillTime);
                                Blocks[DiagX, y + 1]=block;
                                SpawnNewBlock(x,y, offSet, BlockType.Empty);

                                movedPiece= true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        //re check top row
        for(int x=0; x < xSize;x++)
        {
            GameBlock belowBlock = Blocks[x, 0];
            if(belowBlock.Type == BlockType.Empty)
            {
                Destroy(belowBlock.gameObject);
                GameObject newBlock = Instantiate<GameObject>(blockPrefabDict[BlockType.Normal], GetPosition(x, -1,offSet), Quaternion.identity);
                newBlock.transform.parent = transform;

                Blocks[x,0] = newBlock.GetComponent<GameBlock>();
                Blocks[x, 0].Init(x, -1, this, BlockType.Normal);
                Blocks[x, 0].MoveableComponent.Move(x, 0,fillTime);
                Blocks[x, 0].AnimalComponent.SetAnimalType((AnimalBlock.Animaltype)Random.Range(0, (int)AnimalBlock.Animaltype.End));
                movedPiece = true;
            }
        }

        return movedPiece;
    }

    public int CheckSide(int x,int y)
    {
        if (x - 1 < 0 || x + 1 >= xSize) return 0;
        GameBlock leftBlock = Blocks[x - 1, y];
        GameBlock rightBlock = Blocks[x + 1, y];

        if (leftBlock.Type == BlockType.Obstacle)
        {
            return -1;
        }
        else if(rightBlock.Type == BlockType.Obstacle)
        {
            return 1;
        }
        return 0;
    }
}
