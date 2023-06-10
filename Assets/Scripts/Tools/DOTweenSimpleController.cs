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
    // Start is called before the first frame update
    IEnumerator Start()
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
                var rotatedVector = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * moveDirection;
                
                _tweener = transform.DOLocalMove
                (
                    rotatedVector + transform.localPosition,
                    duration);
                
            }
            
        }
        else
        {
            _tweener = transform.DOMove(targetObject.transform.position, duration);
        }
        _tweener.SetEase(EaseType);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if(_tweener.IsPlaying())
            _tweener.Kill();
    }
}
