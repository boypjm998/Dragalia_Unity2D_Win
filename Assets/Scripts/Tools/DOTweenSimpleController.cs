using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class DOTweenSimpleController : MonoBehaviour
{
    [SerializeField] protected Tweener _tweener;

    [SerializeField] protected bool isLocal = true;

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
            _tweener = transform.DOLocalMove
                (
                 moveDirection,
                    duration);
            
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
        if(_tweener.active)
            _tweener.Kill();
    }
}
