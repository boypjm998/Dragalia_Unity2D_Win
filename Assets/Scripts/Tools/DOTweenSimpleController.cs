using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class DOTweenSimpleController : MonoBehaviour
{
    [SerializeField] protected Tweener _tweener;

    [SerializeField] protected bool isLocal = true;

    [SerializeField] protected bool absolutePosition = true;

    [SerializeField] public Vector2 moveDirection;

    [SerializeField] public float duration = 1;

    [SerializeField] private GameObject targetObject;

    [SerializeField] private Ease EaseType = Ease.Linear;

    [SerializeField] private float waitTime = 0;

    [SerializeField] private bool useRigid = false;

    private Rigidbody2D _rigid;
    // Start is called before the first frame update
    public bool isCalled = false;


    public void SetWaitTime(float waitTime)
    {
        this.waitTime = waitTime;
    }
    
    public bool IsAbsolutePosition { get => absolutePosition; set => absolutePosition = value; }

    public void SetEaseType(Ease ease)
    {
        EaseType = ease;
    }


    IEnumerator Start()
    {
        isCalled = true;
        
        yield return new WaitForSeconds(waitTime);

        if (isLocal)
        {
            if (absolutePosition)
            {
                if (!useRigid)
                {
                    _tweener = transform.DOLocalMove
                    (
                        moveDirection,
                        duration);
                }
                else
                {
                    _rigid = GetComponent<Rigidbody2D>();
                    _tweener = _rigid.DOMove
                    (
                        moveDirection + _rigid.position,
                        duration);
                }
                
            }
            else
            {
                //将moveDirection沿着transform.rotation的方向进行旋转。
                var rotatedVector = (Vector3)moveDirection;

                if (useRigid)
                {
                    _rigid = GetComponent<Rigidbody2D>();
                    _tweener = _rigid.DOMove
                    (
                        new Vector3(rotatedVector.x * transform.localScale.x,
                            rotatedVector.y * transform.localScale.y, rotatedVector.z * transform.localScale.z)+transform.position,
                        duration);
                }
                else
                {
                    _tweener = transform.DOLocalMove
                    (
                        rotatedVector+transform.localPosition,
                        duration);
                }
                
                
                
            }
            
        }
        else
        {
            _tweener = transform.DOMove(targetObject.transform.position, duration);
        }
        _tweener.SetEase(EaseType);

    }

    // Update is called once per frame
    public IEnumerator Restart()
    {
        yield return new WaitForSeconds(waitTime);

        if (isLocal)
        {
            if (absolutePosition)
            {
                _tweener = transform.DOLocalMove
                (
                    moveDirection,
                    duration);
            }
            else
            {
                //将moveDirection沿着transform.rotation的方向进行旋转。
                var rotatedVector = (Vector3)moveDirection;
                
                _tweener = transform.DOLocalMove
                (
                    rotatedVector+transform.localPosition,
                    duration);
                
                
            }
            
        }
        else
        {
            _tweener = transform.DOMove(targetObject.transform.position, duration);
        }
        _tweener.SetEase(EaseType);
    }

    private void OnDestroy()
    {
        try
        {
            if(_tweener.IsPlaying())
                _tweener.Kill();
        }
        catch 
        {
        }
        
    }
}
