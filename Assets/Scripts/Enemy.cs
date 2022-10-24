using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Stats of Enemy
    private Animator anim;
    private int enemyid;
    private bool isBoss;
    private int baseHp;
    private int kbResistance;
    private int baseAtk;
    private int baseDef;

    //Hurt Effect
    [SerializeField]
    private GameObject TargetObject;
    private SpriteRenderer spriteRenderer;
    private Material originMaterial;
    private Coroutine hurtEffectCoroutine;
    [SerializeField]
    private Material hurtEffectMaterial;
    [SerializeField]
    private float hurtEffectDuration;



    // Start is called before the first frame update
    private void Start()
    {
        spriteRenderer = TargetObject.GetComponent<SpriteRenderer>();
        originMaterial = spriteRenderer.material;
    }
    void Awake()
    {
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //anim = GetComponent<Animator>();
        
    }

    public virtual void TakeDamage() 
    {
        //anim = GetComponent<Animator>();
        //Debug.Log(anim.name);
        //anim.SetTrigger("hurt");
        Flash();
    }

    public virtual void TakeDamage(int damage)
    {
        Flash();

    }

    private IEnumerator HurtEffectCoroutine()
    {
        spriteRenderer.material = hurtEffectMaterial;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b,100);
        yield return new WaitForSeconds(hurtEffectDuration);
        spriteRenderer.material = originMaterial;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        hurtEffectCoroutine = null;
    }

    public virtual void Flash()
    {
        if (hurtEffectCoroutine != null)
        {
            StopCoroutine(hurtEffectCoroutine);

        }
        hurtEffectCoroutine = StartCoroutine(HurtEffectCoroutine());
    }
}
