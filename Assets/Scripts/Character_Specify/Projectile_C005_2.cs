using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Projectile_C005_2 : MonoBehaviour
{
    private GameObject fx1;
    private GameObject fx2;
    private GameObject fx3;
    private GameObject fx4;
    private Tweener[] _tweener;

    [SerializeField] private GameObject slash1;
    [SerializeField] private GameObject slash2;
    
    private Coroutine animRoutine; 
    void Start()
    {
        fx1 = transform.Find("1").gameObject;
        fx2 = transform.Find("2").gameObject;
        fx3 = transform.Find("3").gameObject;
        fx4 = transform.Find("4").gameObject;
        fx1.GetComponent<Animator>().speed = 0;
        fx3.GetComponent<Animator>().speed = 0;
        fx2.GetComponent<Animator>().speed = 0;
        fx4.GetComponent<Animator>().speed = 0;
        animRoutine = StartCoroutine("PlayAnimations");
        _tweener = new Tweener[4];
    }

    // Update is called once per frame
    IEnumerator PlayAnimations()
    {
        yield return new WaitForSeconds(0.2f);
        fx1.transform.position = new Vector3(fx1.transform.position.x + Random.Range(-1f, 1f),
            fx1.transform.position.y + Random.Range(-1f, 1f));
        fx1.SetActive(true);
        fx1.GetComponent<Animator>().speed = 1;
        
        slash1.SetActive(true);
        slash2.SetActive(true);
        _tweener[0] = fx1.transform.DOMove(
                new Vector3(fx1.transform.position.x - 9f, fx1.transform.position.y - 4f), 0.2f)
            .SetEase(Ease.OutSine);
        
        yield return new WaitForSeconds(0.15f);
        
        fx2.transform.position = new Vector3(fx2.transform.position.x + Random.Range(-1f, 1f),
            fx2.transform.position.y + Random.Range(-1f, 1f));
        fx2.SetActive(true);
        fx2.GetComponent<Animator>().speed = 1;
        _tweener[1] = fx2.transform.DOMove(
                new Vector3(fx2.transform.position.x - 4f, fx2.transform.position.y - 9f), 0.2f)
            .SetEase(Ease.OutSine);
        
        
        yield return new WaitForSeconds(0.15f);
        
        
        
        fx3.transform.position = new Vector3(fx3.transform.position.x + Random.Range(-1f, 1f),
            fx3.transform.position.y + Random.Range(-1f, 1f));
        fx3.SetActive(true);
        fx3.GetComponent<Animator>().speed = 1;
        _tweener[2] = fx3.transform.DOMove(
                new Vector3(fx3.transform.position.x + 9f, fx3.transform.position.y - 4f), 0.2f)
            .SetEase(Ease.OutSine);
        
        yield return new WaitForSeconds(0.15f);
        
        fx4.transform.position = new Vector3(fx4.transform.position.x + Random.Range(-1f, 1f),
            fx4.transform.position.y + Random.Range(-1f, 1f));
        fx4.SetActive(true);
        fx4.GetComponent<Animator>().speed = 1;
        _tweener[3] = fx4.transform.DOMove(
                new Vector3(fx4.transform.position.x + 4f, fx4.transform.position.y - 9f), 0.2f)
            .SetEase(Ease.OutSine);
        
        
        yield return new WaitForSeconds(0.15f);
        fx3.SetActive(false);
        fx1.SetActive(false);
        fx2.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        fx4.SetActive(false);
        
        
    }
}
