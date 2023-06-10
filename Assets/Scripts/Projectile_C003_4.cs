using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSpecificProjectiles
{
    public class Projectile_C003_4 : MonoBehaviour
    {
        ParticleSystem ps;
        private float maxDepth;
        private float minDepth;
        [SerializeField] private float radius = 8f;
        [SerializeField] private float maxSize = 1.4f;
        public ParticleSystem.Particle[] particleArray;
        public ParticleSystem.MainModule psmain;
        
        // Start is called before the first frame update
        void Start()
        {
            ps = GetComponent<ParticleSystem>();
            //获取ps中的所有粒子
            particleArray = new ParticleSystem.Particle[ps.main.maxParticles];
            
            
            maxDepth = 8f;
            minDepth = -8f;
            psmain = ps.main;
            //print(transform.position.z);
    
        }
    
        // Update is called once per frame
        void Update()
        { 
            ps.GetParticles(particleArray);
            // print(particleArray.Length);
            // print((particleArray[0].position.z));
            for(int i = 0; i < particleArray.Length;i++)
            {
                particleArray[i].size = maxSize * 0.55f + 0.5f*maxSize*(-particleArray[i].position.z+radius)/(radius*2);
            }
            if(particleArray.Length>0)
                ps.SetParticles(particleArray,particleArray.Length);
            
        }
    
    }
}

