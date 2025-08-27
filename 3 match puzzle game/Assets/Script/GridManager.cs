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
using TMPro;

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
        bool needsRefill = true;

        while(needsRefill)
        {
            yield return new WaitForSeconds(fillTime);
            while (FillStep()) {
                yield return new WaitForSeconds(fillTime);
            }

            needsRefill = clearAllValidMatches();
        }
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
            if (IsMatching(_block1, _block2.X,_block2.Y)!=null || IsMatching(_block2,_block1.X,_block1.Y)!=null)
            {
                Blocks[_block1.X, _block1.Y] = _block2;
                Blocks[_block2.X, _block2.Y] = _block1;

                Vector2Int tmpPos = new Vector2Int(_block1.X,_block1.Y);
                _block1.MoveableComponent.Move(_block2.X, _block2.Y, fillTime);
                _block2.MoveableComponent.Move(tmpPos.x, tmpPos.y, fillTime);

                clearAllValidMatches();

                StartCoroutine(Fill());
            }
            else
            {
                Blocks[_block1.X, _block1.Y] = _block1;
                Blocks[_block2.X, _block2.Y] = _block2;
            }
        }
    }
    public List<GameBlock> IsMatching(GameBlock _block,int newX, int newY)
    {
        horizontalList.Clear();
        verticalList.Clear();
        if(_block.IsAnimalType())
        {
            MakeHorizontalList(_block, newX, newY);

            List < GameBlock > matchedBlocks = new List<GameBlock>();
            if (horizontalList.Count >= 3)
            {
                foreach (var s in horizontalList)
                {
                    matchedBlocks.Add(s);
                }
            }

            if (horizontalList.Count >= 3)
            {
                for (int i = 0; i < horizontalList.Count; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        for (int offsetY = 1; offsetY < ySize; offsetY++)
                        {
                            int y = newY;
                            if (j == 0)
                            {
                                y = newY - offsetY;
                            }
                            else if (j == 1)
                            {
                                y = newY + offsetY;
                            }

                            if (y < 0 || y >= ySize)
                            {
                                break;
                            }

                            if (Blocks[horizontalList[i].X,y].IsAnimalType() && Blocks[horizontalList[i].X, y].AnimalComponent.animalType == _block.AnimalComponent.animalType)
                            {
                                verticalList.Add(Blocks[horizontalList[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (verticalList.Count < 2)
                    {
                        verticalList.Clear();
                    }
                    else
                    {
                        foreach (var s in verticalList)
                        {
                            matchedBlocks.Add(s);
                        }

                        break;
                    }
                }
            }

            if (matchedBlocks.Count >= 3)
            {
                return matchedBlocks;
            }

            verticalList.Clear();
            horizontalList.Clear();

            MakeVerticalList(_block, newX, newY);

            //List<GameBlock> matchedBlocks = new List<GameBlock>();
            if (verticalList.Count >= 3)
            {
                foreach (var s in verticalList)
                {
                    matchedBlocks.Add(s);
                }

            }

            if (verticalList.Count >= 3)
            {
                for (int i = 0; i < verticalList.Count; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        for (int offsetX = 1; offsetX < ySize; offsetX++)
                        {
                            int x = newX;
                            if (j == 0)
                            {
                                x = newX - offsetX;
                            }
                            else if (j == 1)
                            {
                                x = newX + offsetX;
                            }

                            if (x < 0 || x >= xSize)
                            {
                                break;
                            }

                            if (Blocks[x,verticalList[i].Y].IsAnimalType() && Blocks[x, verticalList[i].Y].AnimalComponent.animalType == _block.AnimalComponent.animalType)
                            {
                                horizontalList.Add(Blocks[x, verticalList[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (horizontalList.Count < 2)
                    {
                        horizontalList.Clear();
                    }
                    else
                    {
                        foreach (var s in horizontalList)
                        {
                            matchedBlocks.Add(s);
                        }

                        break;
                    }
                }
            }

            if (matchedBlocks.Count >= 3)
            {
                return matchedBlocks;
            }
        }
        return null;
    }

    void MakeHorizontalList(GameBlock _block,int newX, int newY)
    {
        horizontalList.Add(_block);
        for (int i = 0; i < 2; i++)
        {
            for (int offsetX = 1; offsetX < xSize; offsetX++)
            {
                int x = newX;

                if (i == 0)
                {
                    x = newX - offsetX;
                }
                else if (i == 1)
                {
                    x = newX + offsetX;
                }

                if (x < 0 || x >= xSize)
                {
                    break;
                }
                if (Blocks[x, newY].IsAnimalType() && Blocks[x, newY].AnimalComponent.animalType == _block.AnimalComponent.animalType)
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

    void MakeVerticalList(GameBlock _block, int newX, int newY)
    {
        verticalList.Add(_block);
        for (int i = 0; i < 2; i++)
        {
            for (int offsetY = 1; offsetY < ySize; offsetY++)
            {
                int y = newY;
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
                if (Blocks[newX, y].IsAnimalType() && Blocks[newX, y].AnimalComponent.animalType == _block.AnimalComponent.animalType)
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

    public List<GameBlock> FindMatch()
    {
        List<tii>[,] adjL = new List<tii>[xSize,ySize];

        for(int i=0;i<xSize; i++)
        {
            for(int j=0;j<ySize; j++)
            {
                adjL[i, j] = new List<tii>();
            }
        }

        List<GameBlock> horizonL = new List<GameBlock>();
        List<GameBlock> verticalL = new List<GameBlock>();
        List<GameBlock> matchL = new List<GameBlock>();

        makeADJ(adjL);

        //bfs
        Queue<tii> q = new Queue<tii>();
        bool[,] Hvisited = new bool[xSize, ySize];
        bool[,] Vvisited = new bool[xSize, ySize];

        for(int i = 0; i < xSize; i++)
        {
            for(int j=0;j < ySize; j++)
            {
                //try horizontal bfs
                if (Hvisited[i, j]==false)
                {
                    q.Enqueue(new tii(i, j));
                    Hvisited[i, j] = true;

                    while(q.Count > 0)
                    {
                        var s = q.Dequeue();
                        foreach( var v in adjL[s.Item1,s.Item2])
                        {
                            int x = v.Item1;
                            int y = v.Item2;
                            if (Hvisited[x,y] == true) continue;

                            //행이 같은가?
                            if (s.Item2 != y) continue;
                            Hvisited[x, y] = true;
                            horizonL.Add(Blocks[x,y]);
                            q.Enqueue(new tii(x,y));
                        }
                    }
                    //end while


                }

                if(horizonL.Count >= 3)
                {
                    return horizonL;
                }

                q.Clear();
                //try vertical bfs
                if (Vvisited[i, j] == false)
                {
                    q.Enqueue(new tii(i, j));
                    Vvisited[i, j] = true;

                    while (q.Count > 0)
                    {
                        var s = q.Dequeue();
                        foreach (var v in adjL[s.Item1, s.Item2])
                        {
                            int x = v.Item1;
                            int y = v.Item2;
                            if (Vvisited[x, y] == true) continue;

                            //열이 같은가?
                            if (s.Item1 != x) continue;
                            Vvisited[x, y] = true;
                            verticalL.Add(Blocks[x, y]);
                            q.Enqueue(new tii(x, y));
                        }
                    }
                    //end while


                }

                if (verticalL.Count >= 3)
                {
                    return verticalL;
                }
            }
        }
        return null;
    }


    void makeADJ(List<tii>[,] adj)
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if (i - 1 < 0) continue;
                {
                    if (Blocks[i - 1, j].IsAnimalType() == false || Blocks[i, j].IsAnimalType() == false) continue;
                    else if (Blocks[i - 1, j].IsAnimalType() && Blocks[i - 1, j].AnimalComponent.animalType == Blocks[i, j].AnimalComponent.animalType)
                    {
                        adj[i, j].Add(new tii(i - 1, j));
                        adj[i - 1, j].Add(new tii(i, j));
                    }

                    if (j - 1 < 0) continue;
                    else if (Blocks[i, j - 1].IsAnimalType() && Blocks[i, j - 1].AnimalComponent.animalType == Blocks[i, j].AnimalComponent.animalType)
                    {
                        adj[i, j].Add(new tii(i, j - 1));
                        adj[i, j - 1].Add(new tii(i, j));
                    }
                }
            }
        }
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

    public bool clearAllValidMatches()
    {
        bool needsRefill = false;
        var s = FindMatch();

        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                IsMatching(Blocks[i, j], i, j);
            }
        }


        /*        if (s != null)
                {
                    for(int i=0;i<s.Count;i++)
                    {
                        if(ClearBlock(s[i].X, s[i].Y))
                        {
                            needsRefill = true;
                        }
                    }
                }*/


        return needsRefill;
    }

    public bool ClearBlock(int x,int y)
    {
        if (Blocks[x,y].IsClearAble() && !Blocks[x, y].ClearAbleComponent.IsBeingCleared)
        {
            Blocks[x, y].ClearAbleComponent.Clear();
            SpawnNewBlock(x, y, offSet, BlockType.Empty);
            return true;
        }
        return false;
    }
}
