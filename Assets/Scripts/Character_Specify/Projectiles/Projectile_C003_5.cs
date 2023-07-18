using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C003_5 : MonoBehaviour
    {
        ParticleSystem ps;
        [SerializeField] private float radius = 8f;
        public ParticleSystem.Particle[] particleArray;
        private float startSize = 2f;
        
        
        // Start is called before the first frame update
        void Start()
        {
            ps = GetComponent<ParticleSystem>();
            //获取ps中的所有粒子
            particleArray = new ParticleSystem.Particle[ps.main.maxParticles];
    
    
        }
    
        // Update is called once per frame
        void Update()
        {
            ps.GetParticles(particleArray);
            //print(ps.GetParticles(particleArray));
            // print(particleArray.Length);
            // print((particleArray[0].position.z));
            for(int i = 0; i < particleArray.Length;i++)
            {
                particleArray[i].size = startSize * 0.55f + 0.5f*startSize*(-particleArray[i].position.z+radius)/(2*radius);
                
            }
            if(particleArray.Length>0)
                ps.SetParticles(particleArray,particleArray.Length);
            
        }
    }
}

