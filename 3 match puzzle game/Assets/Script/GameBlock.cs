using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBlock : MonoBehaviour
{
    private int x;
    private int y;

    public int X
    {
        get { return x; }
        set { if(IsMoveable())
                x = value;
        }
    }

    public int Y
    {
        get { return y; }
        set
        {
            if (IsMoveable())
                y = value;
        }
    }

    private GridManager.BlockType type;

    public GridManager.BlockType Type
    {
        get { return type; }
    }


    private GridManager grid;

    public GridManager GridRef
    {
        get { return grid; }
    }

    private MoveableBlock moveableComponent;

    public MoveableBlock MoveableComponent
    {
        get { return moveableComponent; }
    }

    private AnimalBlock animalComponent;

    public AnimalBlock AnimalComponent
    {
        get { return animalComponent; }
    }

    private ClearAbleBlock clearAbleComponent;

    public ClearAbleBlock ClearAbleComponent
    {
        get { return clearAbleComponent; }
    }

    public void Init(int _x, int _y,GridManager _grid,GridManager.BlockType _type)
    {
        x = _x;
        y = _y;
        type = _type;
        grid = _grid;
    }

    void Awake()
    {
        moveableComponent=GetComponent<MoveableBlock>();
        animalComponent=GetComponent<AnimalBlock>();
        clearAbleComponent = GetComponent<ClearAbleBlock>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsMoveable()
    {
        return moveableComponent != null;
    }

    public bool IsAnimalType()
    {
        return animalComponent != null;
    }

    public bool IsClearAble()
    {
        return clearAbleComponent != null;
    }

    private void OnMouseEnter()
    {
        grid.EnterBlock(this);
    }

    private void OnMouseDown()
    {
        grid.PressedBlock(this);
    }

    private void OnMouseUp()
    {
        grid.ReleaseBlock();
    }
}
