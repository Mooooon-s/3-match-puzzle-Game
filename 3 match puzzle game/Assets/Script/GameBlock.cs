using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBlock : MonoBehaviour
{
    private float x;
    private float y;

    public float X
    {
        get { return x; }
        set { if(IsMoveable())
                x = value;
        }
    }

    public float Y
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
}
