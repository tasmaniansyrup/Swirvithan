using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControl : MonoBehaviour
{
    public Material slMaterial;
    public WeaponController wc;
    public int shinyNumber;
    public float lerpNum;
    public ParticleSystem system;
    private static ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1000];
    public int count;
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        shinyNumber = Shader.PropertyToID("_Shiny_Number");
        slMaterial.SetFloat(shinyNumber, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(wc.canUseSpecialAttack) {
            if(lerpNum < 1) {
                lerpNum += 0.005f;
            }

            count = system.GetParticles(particles);

            for(int i = 0; i < count; i++) {
                ParticleSystem.Particle particle = particles[i];

                Vector3 noVel = new Vector3(0f,0f,0f);
                Vector3 v3 = Vector3.Lerp(particle.velocity, noVel, lerpNum);

                particle.velocity = v3;

                //Quaternion targetRot = Quaternion.LookRotation(particle.position - target.position);
                // particle.axisOfRotation = Vector3.up;
                // particle.rotation3D = Vector3.RotateTowards(target.position, particle.rotation3D, 10, 0f);

                Vector3 direction = new Vector3(1,-80,46);
                particle.rotation3D = Vector3.Lerp(particle.rotation3D,direction, lerpNum);

                particles[i] = particle;
            }
            system.SetParticles(particles, count);
            slMaterial.SetFloat(shinyNumber, lerpNum);
        }
    }
}
