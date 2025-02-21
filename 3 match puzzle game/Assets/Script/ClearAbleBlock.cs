using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearAbleBlock : MonoBehaviour
{
    public AnimationClip clearAnimation;

    private bool isBeingCleared = false;

    public bool IsBeingCleared
    {
        get { return isBeingCleared; }
    }

    protected GameBlock block;

    private void Awake()
    {
        block = GetComponent<GameBlock>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clear()
    {
        isBeingCleared = true;
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();

        if (animator)
        {
            animator.Play(clearAnimation.name);
            yield return new WaitForSeconds(clearAnimation.length);

            Destroy(gameObject);
        }

    }
}
