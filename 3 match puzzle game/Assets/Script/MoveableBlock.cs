using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableBlock : MonoBehaviour
{
    private GameBlock block;
    private IEnumerator moveCoroutine;

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

    public void Move(float _mX,float _mY,float time)
    {
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = MoveCoroutine(_mX, _mY,time);
        StartCoroutine(moveCoroutine);
        //index??
    }

    private IEnumerator MoveCoroutine(float x, float y, float time)
    {
        block.X = x;
        block.Y = y;

        Vector3 startPos = transform.position;
        Vector3 endPos = block.GridRef.GetPosition(x, y, block.GridRef.offSet);

        for(float t = 0 ; t <= 1* time; t+=Time.deltaTime) {
            block.transform.position = Vector3.Lerp(startPos,endPos,t/time);
            yield return 0;
        }

        block.transform.position = endPos;
    }
}
