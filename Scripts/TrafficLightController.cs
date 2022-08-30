using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public TrafficLight[] lights;
    public float timer;
    public float reactionTime;
    
    private int activeLight = 0;
    private float t;

    void Update(){
        if(t <= 0){
            if(activeLight != 2){
                lights[activeLight].SetYellow();
                lights[activeLight+1].SetGreen(reactionTime);
                activeLight++;
            }else{
                lights[activeLight].SetYellow();
                lights[0].SetGreen(reactionTime);
                activeLight = 0;
            }
            t = timer;
        }else{
            t -= Time.deltaTime;
        }
    }
}
