using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;



using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using tii = System.Tuple<int, int>;
using System;
using UnityEngine.XR.WSA.Input;

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

    public GameBlock mousePickedBlock;
    public GameBlock mouseEnterBlock;

    private List<tii>[,] adj;
    private List<tii> connetList = new List<tii>();



    private List<tii>[,] verticalAdj;
    private List<tii>[,] horizontalAadj;


    private List<GameBlock> verticalList = new List<GameBlock>();
    private List<GameBlock> horizontalList = new List<GameBlock>();

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
                AnimalBlock.Animaltype animalType = (AnimalBlock.Animaltype)UnityEngine.Random.Range(0, (int)AnimalBlock.Animaltype.End);
                Blocks[x, 0].AnimalComponent.SetAnimalType(animalType);
                Blocks[x, 0].name = animalType.ToString();
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

    public bool IsAdjBlock(GameBlock _block1,GameBlock _block2)
    {
        // 한칸만 확인 십자가로만 이동이 가능하기 때문에 두가지 만 확인하면 됨
        if(_block1.X == _block2.X && Mathf.Abs(_block1.Y-_block2.Y)==1)
            return true;
        else if (_block1.Y == _block2.Y && Mathf.Abs(_block1.X - _block2.X) == 1)
            return true;
        return false;
    }

    public void SwapBlock(GameBlock _block1,GameBlock _block2)
    {
        if(_block1.IsMoveable() && _block2.IsMoveable())
        {
            Blocks[_block1.X, _block1.Y] = _block2;
            Blocks[_block2.X, _block2.Y] = _block1;

            Vector2Int tmpPos = new Vector2Int(_block1.X,_block1.Y);
            _block1.MoveableComponent.Move(_block2.X, _block2.Y, fillTime);
            _block2.MoveableComponent.Move(tmpPos.x, tmpPos.y, fillTime);

            if (IsMatched(_block1, _block2))
            {
                int a = 0;
                Debug.Log("True");
            }
            else
            {
                Blocks[_block1.X, _block1.Y] = _block2;
                Blocks[_block2.X, _block2.Y] = _block1;

                tmpPos = new Vector2Int(_block1.X, _block1.Y);
                _block1.MoveableComponent.Move(_block2.X, _block2.Y, fillTime);
                _block2.MoveableComponent.Move(tmpPos.x, tmpPos.y, fillTime);
                Debug.Log("False");
            }
        }
    }
    public void MakeAdj(GameBlock _block,int newX, int newY)
    {
        horizontalList.Clear();
        verticalList.Clear();

        horizontalList.Add(_block);
        int x = newX;
        for(int i = 0; i < 2; i++)
        {
            for(int offsetX = 1; offsetX <= xSize; offsetX++)
            {
                if (i == 0)
                {
                    x = newX - offsetX;
                }
                else if (i == 1)
                {
                    x = newX + offsetX;
                }
                if(x < 0 || x >= xSize)
                {
                    break;
                }


                if (Blocks[x, newY].IsAnimalType() && x != newX)
                {
                    if (Blocks[x, newY].AnimalComponent.animalType == _block.AnimalComponent.animalType)
                    {
                        horizontalList.Add(Blocks[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        verticalList.Add(_block);
        int y = newY;
        for (int i = 0; i < 2; i++)
        {
            for (int offsetY = 1; offsetY <= ySize; offsetY++)
            {
                if (i == 0)
                {
                    y = newY - offsetY;
                }
                else if (i == 1)
                {
                    y = newY + offsetY;
                }
                if (y < 0 || y >= ySize)
                {
                    break;
                }


                if (Blocks[newX, y].IsAnimalType() && y != newY)
                {
                    if (Blocks[newX, y].AnimalComponent.animalType == _block.AnimalComponent.animalType)
                    {
                        verticalList.Add(Blocks[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    public bool IsMatched(GameBlock _Pickblock, GameBlock _otherblock)
    {
        //board
        bool[,] hVisited = new bool[xSize,ySize];
        bool[,] vVisited = new bool[xSize,ySize];
        Queue<tii> horizontalQueue = new Queue<tii>();
        Queue<tii> verticalQueue = new Queue<tii>();

        //인접리스트 생성
        MakeAdj(_Pickblock,_otherblock.X,_otherblock.Y);

        //큐에 새로운 위치 넣고 BFS돌리기

        //세로 방향
/*        tii newPos = new tii(_otherblock.X, _otherblock.Y);

        horizontalQueue.Enqueue(newPos);
        hVisited[newPos.Item1, newPos.Item2] = true;
        int _horizontalCount = 1;

        while(horizontalQueue.Count != 0)
        {
            tii _tii = horizontalQueue.Peek(); horizontalQueue.Dequeue();
            foreach (var s in horizontalAadj[_tii.Item1,_tii.Item2])
            {
                tii nextpos= s;
                if (hVisited[nextpos.Item1, nextpos.Item2]) continue;
                hVisited[nextpos.Item1, nextpos.Item2] = true;
                horizontalQueue.Enqueue(nextpos);
                _horizontalCount++;
            }
        }


        //가로 방향
        verticalQueue.Enqueue(newPos);
        vVisited[newPos.Item1, newPos.Item2] = true;
        int _verticalCount = 1;
        while (verticalQueue.Count != 0)
        {
            tii _tii = verticalQueue.Peek(); verticalQueue.Dequeue();
            foreach (var s in verticalAdj[_tii.Item1, _tii.Item2])
            {
                tii nextpos = s;
                if (vVisited[nextpos.Item1, nextpos.Item2]) continue;
                vVisited[nextpos.Item1, nextpos.Item2] = true;
                verticalQueue.Enqueue(nextpos);
                _verticalCount++;
            }
        }*/

        //3개 이상이면 참
        if (verticalList.Count >=3 || horizontalList.Count >= 3)
            return true;
        return false;
    }

    public void PressedBlock(GameBlock _gameBlock)
    {
        mousePickedBlock = _gameBlock;
    }

    public void EnterBlock(GameBlock _gameBlock)
    {
        mouseEnterBlock = _gameBlock;
    }

    public void ReleaseBlock()
    {
        if (IsAdjBlock(mousePickedBlock, mouseEnterBlock))
        {
            SwapBlock(mousePickedBlock, mouseEnterBlock);
        }
    }
}
