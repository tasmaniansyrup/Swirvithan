using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleToPoint : MonoBehaviour
{

    public Transform point;

    public ParticleSystem particleSystem;

    private static ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1000];
    private static Vector3[] targetPosition = new Vector3[1000];

    public int count;
    public float speed;
    public float particleTimeToThrow;
    
    public bool flag;
    public bool flag2;
    // Start is called before the first frame update
    void Start()
    {
        // var main = particleSystem.main;
        // main.simulationSpace = ParticleSystemSimulationSpace.World;
    }

    void Awake() {
        flag = true;
        flag2 = true;
    }

    // Update is called once per frame
    void Update()
    {
        count = particleSystem.GetParticles(particles);
        
        for(int i = 0; i < count; i++) {
            speed = 0.05f;
            if(flag2) {
                targetPosition[i] = point.localPosition;
            }
            if(flag && i == 0) {
                particleTimeToThrow = particles[i].remainingLifetime;
                flag = false;
            }
            if(particles[i].remainingLifetime < particleTimeToThrow){
                ParticleSystem.Particle particle = particles[i];
                Vector3 targetPoint = targetPosition[i];

                Vector3 v1 = particle.position;
                Vector3 v2 = targetPoint;

                Vector3 targetPos = Vector3.MoveTowards(v1, v2, speed);
                particle.position = targetPos;
                
                particles[i] = particle;
            }
        }
        Debug.Log(targetPosition[0]);
        particleSystem.SetParticles(particles, count);
    }
}
