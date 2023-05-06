using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C005_1 : MonoBehaviour
    {
        [SerializeField] private GameObject slash1;
        [SerializeField] private GameObject slash2;
        private Tweener[] _tweener;

        private Coroutine animRoutine;
        private GameObject fx1;
        private GameObject fx2;
        private GameObject fx3;
        private GameObject fx4;

        // Start is called before the first frame update
        private void Start()
        {
            fx1 = transform.Find("1").gameObject;
            //fx2 = transform.Find("2").gameObject;
            fx3 = transform.Find("3").gameObject;
            //fx4 = transform.Find("4").gameObject;
            fx1.GetComponent<Animator>().speed = 0;
            fx3.GetComponent<Animator>().speed = 0;
            animRoutine = StartCoroutine("PlayAnimations");
            _tweener = new Tweener[2];
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void OnDestroy()
        {
            CancelInvoke();
            _tweener[0].Kill();
            _tweener[1].Kill();
        }

        private IEnumerator PlayAnimations()
        {
            yield return new WaitForSeconds(0.2f);
            fx1.transform.position = new Vector3(fx1.transform.position.x + Random.Range(-1f, 1f),
                fx1.transform.position.y + Random.Range(-1f, 1f));
            fx1.SetActive(true);
            fx1.GetComponent<Animator>().speed = 1;

            slash1.SetActive(true);
            slash2.SetActive(true);
            _tweener[0] = fx1.transform.DOMove(
                    new Vector3(fx1.transform.position.x - 5f, fx1.transform.position.y - 2.5f), 0.2f)
                .SetEase(Ease.OutSine);


            yield return new WaitForSeconds(0.35f);

            fx3.transform.position = new Vector3(fx3.transform.position.x + Random.Range(-1f, 1f),
                fx3.transform.position.y + Random.Range(-1f, 1f));
            fx3.SetActive(true);
            fx3.GetComponent<Animator>().speed = 1;
            _tweener[1] = fx3.transform.DOMove(
                    new Vector3(fx3.transform.position.x + 5f, fx3.transform.position.y - 2.5f), 0.2f)
                .SetEase(Ease.OutSine);

            yield return new WaitForSeconds(0.4f);
            fx3.SetActive(false);
            fx1.SetActive(false);


            //yield return new WaitForSeconds(0.25f);
            //fx3.transform.position = new Vector3(fx3.transform.position.x + Random.Range(-1f, 1f),
            //    fx3.transform.position.y + Random.Range(-1f, 1f));
            //fx3.SetActive(true);
            //
            //yield return new WaitForSeconds(0.25f);
            //fx4.transform.position = new Vector3(fx4.transform.position.x + Random.Range(-1f, 1f),
            //    fx4.transform.position.y + Random.Range(-1f, 1f));
            //fx4.SetActive(true);
        }
    }
}