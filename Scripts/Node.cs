using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Graph graph;
    public Node[] adjList;
    public bool free = true;
    public bool blocked = false;
    public bool isFreeway = false;
    public bool isFast = false;
    public bool isTarget = false;
    public bool isEnd = false;
    
    public int cost;
    private Material[] busyMaterial = new Material[1];
    private Material[] freeMaterial = new Material[1];
    private Material[] blockedMaterial = new Material[1];

    public void Start(){
        if(isTarget){
            cost = graph.targetCost;
        }else if(isFreeway){
            cost = graph.freewayCost;
        }else{
            cost = graph.cost;
        }
        if(isFast){
            cost -= graph.freewayFastCostReduction;
        }
        blockedMaterial[0] = graph.blockedMat;
        busyMaterial[0] = graph.busyMat;
        freeMaterial[0] = gameObject.GetComponent<MeshRenderer>().materials[0];
        gameObject.GetComponent<MeshRenderer>().materials = freeMaterial;
    }

    public void setOccupied(){
        if(!isEnd && !blocked){
            gameObject.GetComponent<MeshRenderer>().materials = busyMaterial;
            free = false;
        }
    }

    public void setFree(){
        if(!blocked){
            //Invoke("_freeUp", 0.2f);
            gameObject.GetComponent<MeshRenderer>().materials = freeMaterial;
            free = true;
        }
    }
    void _freeUp(){
            gameObject.GetComponent<MeshRenderer>().materials = freeMaterial;
            free = true;
    }

    public void setBlocked(){
        free = false;
        gameObject.GetComponent<MeshRenderer>().materials = blockedMaterial;
        blocked = true;
    }
    public void setUnblocked(){
        blocked = false;
        setFree();
    }

    public int getCost(){
        if(free == true || isFreeway){
            return(cost);
        }else{
            return(cost+graph.occupiedPenalty);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player") && isEnd){
            Destroy(other.gameObject);
        }
    }

}
