using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public Node startingNode;
    public Node targetNode;
    public int state = 0; // 0:Neutral, 1:Calculating, 2:Transit, 3:Waiting
    public float topSpeed;
    public float acceleration;
    public float turnSpeed;
    public List<Node> path;
    public Node[] objective;

    private float _speed;

    // Agente Translation
    private Node currNode;
    private Node nextNode;
    // Agent Rotation
    private Quaternion _lookRotation;
    private Vector3 _direction;

    // Graph
    private Graph graph;
    private float objectiveTime;

    // To check if we need to disappear agent when he reaches his objective
    private bool leaving;

    // To check if agent is waiting to advance to next node
    public bool queue;
    private float qRefreshRate = 0.5f;
    private float qTimer;

    // Agent spawner calls this function to start up the agent
    public void StartUp(){
        qTimer = qRefreshRate;
        leaving = false;
        queue = false;
        graph = GameObject.FindObjectOfType<Graph>();
        state = 1;
        // Calculate path to nearest available objective
        path = findPath(startingNode);
        nextNode = path[0];
    }

    // Update is called once per frame
    void Update(){
        if(state == 2){
            if(!queue){
                if(_speed < topSpeed){
                    _speed += acceleration * Time.deltaTime;
                }else if(_speed > topSpeed){
                    _speed = topSpeed;
                }
                float step =  _speed * Time.deltaTime;
                Vector3 moveTo = Vector3.MoveTowards(transform.position, nextNode.transform.position, step);
                transform.position = new Vector3(moveTo.x, transform.position.y, moveTo.z);
                
                Vector3 nextNodePos = new Vector3(nextNode.transform.position.x, transform.position.y, nextNode.transform.position.z);
                _direction = (nextNodePos - transform.position).normalized;
                _lookRotation = Quaternion.LookRotation(_direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * turnSpeed);
            }else{
                if(nextNode.free){
                    queue = false;
                    entersNode(currNode.GetComponent<Collider>(), false);
                }
                // Refreshes path every 0.5s while waiting
                if(qTimer <= 0){
                    qTimer = qRefreshRate;
                    path = findPath(currNode);
                    path.RemoveAt(0);
                    nextNode = path[0];
                }else{
                    qTimer -= Time.deltaTime;
                }
            }
        }
        if(state == 3){
            if(objectiveTime <= 0){
                state = 1;
                leaving = true;
                objective = graph.exitNodes;
                path = findPath(currNode);
                path.RemoveAt(0);
                nextNode = path[0];
            }else{
                objectiveTime -= Time.deltaTime;
            }
        }
    }
    
    // A* Pathfinding algorithm
    List<Node> findPath(Node start){
        targetNode = getNearest(objective);
        List<Node> openList = new List<Node>();
        List<Node>  closedList = new List<Node>();
        Dictionary<Node, int> costs = new Dictionary<Node, int>();
        Dictionary<Node, Node> parents = new Dictionary<Node, Node>();

        openList.Add(start);
        costs[start] = 0;
        parents[start] = start;

        while(openList.Count > 0){
            Node n = null;
            Node v = null;
            for(int i = 0; i < openList.Count; i++){
                v = openList[i];
                if(n == null || costs[v] + heuristic(v, objective) < costs[n] + heuristic(n, objective)){
                    n = v;
                }
            }

            if(n == null){
                Debug.Log("Path does not exist!");
                return null;
            }

            if(n == targetNode){
                List<Node> path = new List<Node>();

                while(parents[n] != n){
                    path.Add(n);
                    n = parents[n];
                }

                currNode = start;
                path.Add(start);
                path.Reverse();

                state = 2;

                return path;
            }

            int nNeighbors = n.adjList.Length;
            for(int i = 0; i < nNeighbors; i++){
                Node currNeighbor = n.adjList[i];
                if(!openList.Contains(currNeighbor) && !closedList.Contains(currNeighbor)){
                    openList.Add(currNeighbor);
                    parents[currNeighbor] = n;
                    costs[currNeighbor] = costs[n] + currNeighbor.getCost();
                }else{
                    if(costs[currNeighbor] > (costs[n] + currNeighbor.getCost())){
                        costs[currNeighbor] = costs[n] + currNeighbor.getCost();
                        parents[currNeighbor] = n;

                        if(closedList.Contains(currNeighbor)){
                            closedList.Remove(currNeighbor);
                            openList.Add(currNeighbor);
                        }
                    }
                }
            }

            openList.Remove(n);
            closedList.Add(n);

        }
        Debug.Log("Path does not exist!");
        return null;
    }

    // Heuristic calculated using euclidean distance between node a and node group b
    int heuristic(Node a, Node[] bG){
        Vector3 b = graph.heuristicPosition(bG);
        int cost = (int)Mathf.Sqrt(Mathf.Pow(a.gameObject.transform.position.x - b.x, 2)+Mathf.Pow(a.gameObject.transform.position.z - b.z, 2));
        return cost;
    }

    // This function returns the nearest available node in objective
    Node getNearest(Node[] nodes){
        if(leaving){
            return(nodes[Random.Range(0, nodes.Length)]);
        }
        List<Node> cleanNodes = new List<Node>();
        for(int i = 0; i < nodes.Length; i++){
            if(nodes[i].free){
                cleanNodes.Add(nodes[i]);
            }
        }
        Node nearest; 
        if(cleanNodes.Count > 0){
            nearest = cleanNodes[0];
            for(int i = 0; i < cleanNodes.Count; i++){
                if(euclidean(nearest) > euclidean(cleanNodes[i])){
                    nearest = cleanNodes[i];
                }
            }
        }else{
            nearest = nodes[0];
            for(int i = 0; i < nodes.Length; i++){
                if(euclidean(nearest) > euclidean(nodes[i])){
                    nearest = nodes[i];
                }
            }
        }
        return nearest;
    }

    // Euclidean distance btwn agent and node
    float euclidean(Node a){
        float d = Mathf.Sqrt(Mathf.Pow(a.gameObject.transform.position.x - transform.position.x, 2)+Mathf.Pow(a.gameObject.transform.position.z - transform.position.z, 2));
        return d;
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Node")){
            entersNode(other, true);
        }
    }

    void entersNode(Collider other, bool natural){
        if(natural){
            currNode.setFree();
        }
        Node collidedNode = other.gameObject.GetComponent<Node>();
        currNode = collidedNode;
        if(currNode == targetNode){
            if(!leaving){
                state = 3;
            }
        }else if(other.gameObject.GetComponent<Node>() == path[0]){
            currNode.setOccupied();
            path.RemoveAt(0);
            nextNode = path[0];
            if(nextNode.free){
                // TESTING
                path = findPath(currNode);
                path.RemoveAt(0);
                nextNode = path[0];
                nextNode.setOccupied();
            }else{
                _speed = 0;
                queue = true;
            }
        }
    }

    public void SetObjectiveTime(float time){
        objectiveTime = time;
    }
}
