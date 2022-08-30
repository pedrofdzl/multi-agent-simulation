using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public GameObject[] nodes;
    public Node[] exitNodes;
    public int occupiedPenalty = 2;
    public int freewayCost = 2;
    public int freewayFastCostReduction = 1;
    public int cost = 3;
    public int targetCost = 5;

    public Material blockedMat;
    public Material busyMat;
    public Material freeMat;
    public Material occMat;

    public Vector3 heuristicPosition(Node[] nodeGroup){
        float x = 0;
        float z = 0;
        for(int i = 0; i < nodeGroup.Length; i++){
            x += nodeGroup[i].gameObject.transform.position.x;
            z += nodeGroup[i].gameObject.transform.position.z;
        }
        x /= nodeGroup.Length;
        z /= nodeGroup.Length;
        
        Vector3 ans = new Vector3(x, 0.5f, z);
        return(ans);
    }
}
