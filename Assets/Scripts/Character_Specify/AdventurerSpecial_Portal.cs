using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerSpecial_Portal : MonoBehaviour
{
    [SerializeField] private GameObject transportationEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Transport(Transform origin)
    {
        var eff = Instantiate(transportationEffect, origin.position, Quaternion.identity);
        var eff2 = Instantiate(transportationEffect, transform.position, Quaternion.identity);
        eff2.GetComponent<AdventurerSpecial_PortalTrail>().InitAnim(origin);
        
        origin.transform.position = this.transform.position;
        
        eff.GetComponent<AdventurerSpecial_PortalTrail>().InitAnim(transform);
        
        
        Destroy(gameObject);
    }


}
