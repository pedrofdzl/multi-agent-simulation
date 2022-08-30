using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Objective
{
    public string name;
    public float minTime;
    public float maxTime;
    public Node[] nodes;
}

public class AgentSpawner : MonoBehaviour
{
    public GameObject agentPrefab;
    public int agentsPerMinute;
    public float randomness;
    public Node[] startingNodes;

    public Objective[] objectives;

    private float rate;
    private float timer;

    void Start(){
        rate = 60 / agentsPerMinute;
        setTimer();
    }

    void Update(){
        if(timer <= 0){
            spawnAgent();
            setTimer();
        }else{
            timer -= Time.deltaTime;
        }
    }

    void setTimer(){
        timer = Random.Range(rate-randomness, rate+randomness);
    }

    void spawnAgent(){
        var a = Instantiate(agentPrefab, transform.position, Quaternion.identity);
        int r = Random.Range(0, objectives.Length);
        a.GetComponent<Agent>().startingNode = startingNodes[Random.Range(0, 2)];
        a.GetComponent<Agent>().SetObjectiveTime(Random.Range(objectives[r].minTime, objectives[r].maxTime+1));
        a.GetComponent<Agent>().objective = objectives[r].nodes;
        // TESTING
        //a.GetComponent<Agent>().objective = objectives[2].nodes;
        a.GetComponent<Agent>().StartUp();
    }
}
