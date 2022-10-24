using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SlashTest : MonoBehaviour
{
    public Animator anim;
    public List<Slash> slashes;

    private bool attacking;

    void Start()
    {
        DisableSlashes();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !attacking)
        {
            attacking = true;
            anim.SetTrigger("attack");
            StartCoroutine(SlashAttack());
        }
    }

    IEnumerator SlashAttack()
    {
        for(int i=0; i<slashes.Count; i++)
        { 
            yield return new WaitForSeconds(slashes[i].delay);
            slashes[i].slashObj.SetActive(true);
        }

        yield return new WaitForSeconds(1);
        DisableSlashes();
        attacking = false;
    }
    void DisableSlashes()
    {
        for (int i = 0; i < slashes.Count; i++)
            slashes[i].slashObj.SetActive(false);
    }
}

[System.Serializable]
public class Slash 
{
    public GameObject slashObj;
    public float delay;
}
