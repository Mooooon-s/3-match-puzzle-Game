using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableBlock : MonoBehaviour
{
    private GameBlock block;

    // Start is called before the first frame update
    void Awake()
    {
        block = GetComponent<GameBlock>();   
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(float _mX,float _mY)
    {
        //index??
        block.X = _mX;
        block.Y = _mY;

        Vector2 offset = block.GridRef.offSet;
        block.transform.position = block.GridRef.GetPosition(_mX, _mY, offset);
    }
}
