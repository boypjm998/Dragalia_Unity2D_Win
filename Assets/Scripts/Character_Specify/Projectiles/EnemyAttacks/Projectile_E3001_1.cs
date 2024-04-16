using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_E3001_1 : MonoBehaviour
    {
        private SpriteRenderer fill;

        private SpriteRenderer back;
        // Start is called before the first frame update
        public Color safeColor;
        public Color warningColor;
        public Color dangerColor;

        public float forcingTime = 10f;

        private void Awake()
        {
            fill = transform.GetChild(0).GetComponent<SpriteRenderer>();
            back = transform.GetChild(1).GetComponent<SpriteRenderer>();
            fill.gameObject.SetActive(false);
        }

        void Start()
        {
            
            fill.gameObject.SetActive(true);
            fill.size = new Vector2(0.1f, fill.size.y);
            
            //用Dotween.To 令fill的width从0到back的width
            
            var twc = DOTween.To(() => fill.size,
                x => fill.size = x,
                back.size, forcingTime);
            twc.OnComplete(() =>
            {
                Destroy(gameObject,0.1f);
            });
            
            
        }
        
        
        

        // Update is called once per frame
        void Update()
        {
            if(fill.size.x > back.size.x * 0.7f)
                fill.color = dangerColor;
            else if(fill.size.x > back.size.x * 0.4f)
                fill.color = warningColor;
            else
                fill.color = safeColor;
        }
    }
}


