using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public GameObject redLight;
    public GameObject yellowLight;
    public GameObject greenLight;
    public Node[] nodes;

    public void SetRed(){
        yellowLight.SetActive(false);
        greenLight.SetActive(false);
        redLight.SetActive(true);
    }

    public void SetYellow(){
        redLight.SetActive(false);
        greenLight.SetActive(false);
        yellowLight.SetActive(true);
        for(int i = 0; i < nodes.Length; i++){
            nodes[i].setBlocked();
        }
        Invoke("SetRed", 2f);
    }

    public void SetGreen(float time){
        yellowLight.SetActive(false);
        redLight.SetActive(false);
        greenLight.SetActive(true);
        Invoke("FreeNodes", time);
    }

    private void FreeNodes(){
        for(int i = 0; i < nodes.Length; i++){
            nodes[i].setUnblocked();
        }
    }
}
