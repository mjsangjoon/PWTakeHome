using UnityEngine;
using System.Collections.Generic;
using System;

public class NavGrid : MonoBehaviour
{
    public int NumberOfCellsPerSide;
    public Vector3[] obstacleLocations;

    [ContextMenuItem("Add this many obstacles to grid", "AddObstacles")]
    public int numObstaclesToAdd;
    public Player player;
    public GameObject obstaclePrefab;
    private float planeYVal = 0;

    /// <summary>
    /// Given the current and desired location, return a path to the destination
    /// </summary>
    public NavGridPathNode[] GetPath(Vector3 origin, Vector3 destination)
    {
        planeYVal = GetComponent<MeshCollider>().transform.position. y;
        NavGridPathNode destinationNode = AStarSearch(origin, destination);

        // A* did not find a valid path so there is no parent for the destination node
        if(destinationNode.Parent == null){
            return new NavGridPathNode[]{
                new() { Position = origin }
            };
        }

        Stack<NavGridPathNode> path = new Stack<NavGridPathNode>();
        while(destinationNode.Position != GetNearestGridPosition(origin)){
            path.Push(destinationNode);
            destinationNode = destinationNode.Parent;
        }
        //last node pushed is the true destination, not grid destination
        path.Push(new NavGridPathNode(
            new Vector3(destination.x, destination.y, destination.z),
            destinationNode,
            destinationNode.fVal+1,
            destinationNode.gVal+1,
            destinationNode.hVal));
        return path.ToArray();
    }

    public NavGridPathNode AStarSearch(Vector3 origin, Vector3 destination){
        //sets to track visted nodes, potential nodes, and obstacle locations
        HashSet<NavGridPathNode> checkedNodes = new HashSet<NavGridPathNode>();
        SortedSet<NavGridPathNode> uncheckedNodes = new SortedSet<NavGridPathNode>(new FValComparer());
        HashSet<Vector3> obstacleSet = arrayToHashSet(obstacleLocations);
        Vector3 gridDestination = GetNearestGridPosition(destination);

        //seed with starting location
        uncheckedNodes.Add(new NavGridPathNode(){ Position = GetNearestGridPosition(origin), Parent = null, fVal=0, gVal=0, hVal=0});

        //do the algorithm
        while (uncheckedNodes.Count > 0){
            //remove element with lowest f in unchecked nodes and add to set of checked nodes
            NavGridPathNode n = uncheckedNodes.Min;
            uncheckedNodes.Remove(n);
            checkedNodes.Add(n);

            //g, h, and f values for successors of the node n
            float gNew, hNew, fNew;

            /** 
             * Check successors to north, south, east, west, northeast, northwest,
             * southeast, and southwest directions
             */
            // North successor
            NavGridPathNode north = new NavGridPathNode(new Vector3(n.Position.x-GetCellSize(), planeYVal, n.Position.z));
            if (IsValidNode(north.Position)){
                // found the destination, set parent and return
                if(north.Position == gridDestination){
                    // Set parent of the destination node
                    north.Parent = n;
                    return north;
                }
                // If successor is already checked or blocked, ignore it, otherwise add to unchecked
                // or update node with new f val
                else if (!checkedNodes.Contains(north) && !obstacleSet.Contains(north.Position)){
            Debug.Log("successor valid1");
                    gNew = n.gVal + GetCellSize();
                    hNew = n.hVal + Vector3.Distance(north.Position, gridDestination);
                    fNew = gNew + hNew;

                    north.fVal = fNew;
                    north.gVal = gNew;
                    north.hVal = hNew;
                    north.Parent = n;

                    //if not in open list, add to open list. Make node n the parent of this successor
                    if(!uncheckedNodes.Contains(north)){
                        uncheckedNodes.Add(north);
                    }
                    //if in open list but has as a smaller f value, remove and add smaller f value node
                    else{
                        NavGridPathNode old;
                        uncheckedNodes.TryGetValue(north, out old);
                        if(old.fVal > north.fVal){
                            uncheckedNodes.Remove(old);
                            uncheckedNodes.Add(north);
                        }
                    }
                }
            }

            // South successor
            NavGridPathNode south = new NavGridPathNode(new Vector3(n.Position.x+GetCellSize(), planeYVal, n.Position.z));
            if (IsValidNode(south.Position)){
                // found the destination, set parent and return
                if(south.Position == gridDestination){
                    // Set parent of the destination node
                    south.Parent = n;
                    return south;
                }
                // If successor is already checked or blocked, ignore it
                else if (!checkedNodes.Contains(south) && !obstacleSet.Contains(south.Position)){
            Debug.Log("successor valid2");
                    gNew = n.gVal + GetCellSize();
                    hNew = n.hVal + Vector3.Distance(south.Position, gridDestination);
                    fNew = gNew + hNew;

                    south.fVal = fNew;
                    south.gVal = gNew;
                    south.hVal = hNew;
                    south.Parent = n;

                    //if not in open list, add to open list. Make node n the parent of this successor
                    if(!uncheckedNodes.Contains(south)){
                        uncheckedNodes.Add(south);
                    }
                    //if in open list but has as a smaller f value, remove and add smaller f value node
                    else{
                        NavGridPathNode old;
                        uncheckedNodes.TryGetValue(south, out old);
                        if(old.fVal > south.fVal){
                            uncheckedNodes.Remove(old);
                            uncheckedNodes.Add(south);
                        }
                    }
                }
            }

            // East successor
            NavGridPathNode east = new NavGridPathNode(new Vector3(n.Position.x, planeYVal, n.Position.z+GetCellSize()));
            if (IsValidNode(east.Position)){
                // found the destination, set parent and return
                if(east.Position == gridDestination){
                    // Set parent of the destination node
                    east.Parent = n;
                    return east;
                }
                // If successor is already checked or blocked, ignore it
                else if (!checkedNodes.Contains(east) && !obstacleSet.Contains(east.Position)){
            Debug.Log("successor valid3");
                    gNew = n.gVal + GetCellSize();
                    hNew = n.hVal + Vector3.Distance(east.Position, gridDestination);
                    fNew = gNew + hNew;

                    east.fVal = fNew;
                    east.gVal = gNew;
                    east.hVal = hNew;
                    east.Parent = n;

                    //if not in open list, add to open list. Make node n the parent of this successor
                    if(!uncheckedNodes.Contains(east)){
                        uncheckedNodes.Add(east);
                    }
                    //if in open list but has as a smaller f value, remove and add smaller f value node
                    else{
                        NavGridPathNode old;
                        uncheckedNodes.TryGetValue(east, out old);
                        if(old.fVal > east.fVal){
                            uncheckedNodes.Remove(old);
                            uncheckedNodes.Add(east);
                        }
                    }
                }
            }

            // West successor
            NavGridPathNode west = new NavGridPathNode(new Vector3(n.Position.x, planeYVal, n.Position.z-GetCellSize()));
            if (IsValidNode(west.Position)){
                // found the destination, set parent and return
                if(west.Position == gridDestination){
                    // Set parent of the destination node
                    west.Parent = n;
                    return west;
                }
                // If successor is already checked or blocked, ignore it
                else if (!checkedNodes.Contains(west) && !obstacleSet.Contains(west.Position)){
            Debug.Log("successor valid4");
                    gNew = n.gVal + GetCellSize();
                    hNew = n.hVal + Vector3.Distance(west.Position, gridDestination);
                    fNew = gNew + hNew;

                    west.fVal = fNew;
                    west.gVal = gNew;
                    west.hVal = hNew;
                    west.Parent = n;

                    //if not in open list, add to open list. Make node n the parent of this successor
                    if(!uncheckedNodes.Contains(west)){
                        uncheckedNodes.Add(west);
                    }
                    //if in open list but has as a smaller f value, remove and add smaller f value node
                    else{
                        NavGridPathNode old;
                        uncheckedNodes.TryGetValue(west, out old);
                        if(old.fVal > west.fVal){
                            uncheckedNodes.Remove(old);
                            uncheckedNodes.Add(west);
                        }
                    }
                }
            }

            // Northeast successor
            NavGridPathNode northeast = new NavGridPathNode(new Vector3(n.Position.x-GetCellSize(), planeYVal, n.Position.z+GetCellSize()));
            if (IsValidNode(northeast.Position)){
                // found the destination, set parent and return
                if(northeast.Position == gridDestination){
                    // Set parent of the destination node
                    northeast.Parent = n;
                    return northeast;
                }
                // If successor is already checked or blocked, ignore it
                else if (!checkedNodes.Contains(northeast) && !obstacleSet.Contains(northeast.Position)){
            Debug.Log("successor valid5");
                    gNew = n.gVal + GetCellSize();
                    hNew = n.hVal + Vector3.Distance(northeast.Position, gridDestination);
                    fNew = gNew + hNew;

                    northeast.fVal = fNew;
                    northeast.gVal = gNew;
                    northeast.hVal = hNew;
                    northeast.Parent = n;

                    //if not in open list, add to open list. Make node n the parent of this successor
                    if(!uncheckedNodes.Contains(northeast)){
                        uncheckedNodes.Add(northeast);
                    }
                    //if in open list but has as a smaller f value, remove and add smaller f value node
                    else{
                        NavGridPathNode old;
                        uncheckedNodes.TryGetValue(northeast, out old);
                        if(old.fVal > northeast.fVal){
                            uncheckedNodes.Remove(old);
                            uncheckedNodes.Add(northeast);
                        }
                    }
                }
            }

            // Northwest successor
            NavGridPathNode northwest = new NavGridPathNode(new Vector3(n.Position.x-GetCellSize(), planeYVal, n.Position.z-GetCellSize()));
            if (IsValidNode(northwest.Position)){
                // found the destination, set parent and return
                if(northwest.Position == gridDestination){
                    // Set parent of the destination node
                    northwest.Parent = n;
                    return northwest;
                }
                // If successor is already checked or blocked, ignore it
                else if (!checkedNodes.Contains(northwest) && !obstacleSet.Contains(northwest.Position)){
            Debug.Log("successor valid6");
                    gNew = n.gVal + GetCellSize();
                    hNew = n.hVal + Vector3.Distance(northwest.Position, gridDestination);
                    fNew = gNew + hNew;

                    northwest.fVal = fNew;
                    northwest.gVal = gNew;
                    northwest.hVal = hNew;
                    northwest.Parent = n;

                    //if not in open list, add to open list. Make node n the parent of this successor
                    if(!uncheckedNodes.Contains(northwest)){
                        uncheckedNodes.Add(northwest);
                    }
                    //if in open list but has as a smaller f value, remove and add smaller f value node
                    else{
                        NavGridPathNode old;
                        uncheckedNodes.TryGetValue(northwest, out old);
                        if(old.fVal > northwest.fVal){
                            uncheckedNodes.Remove(old);
                            uncheckedNodes.Add(northwest);
                        }
                    }
                }
            }

            // Southeast successor
            NavGridPathNode southeast = new NavGridPathNode(new Vector3(n.Position.x+GetCellSize(), planeYVal, n.Position.z+GetCellSize()));
            if (IsValidNode(southeast.Position)){
                // found the destination, set parent and return
                if(southeast.Position == gridDestination){
                    // Set parent of the destination node
                    southeast.Parent = n;
                    return southeast;
                }
                // If successor is already checked or blocked, ignore it
                else if (!checkedNodes.Contains(southeast) && !obstacleSet.Contains(southeast.Position)){
            Debug.Log("successor valid7");
                    gNew = n.gVal + GetCellSize();
                    hNew = n.hVal + Vector3.Distance(southeast.Position, gridDestination);
                    fNew = gNew + hNew;

                    southeast.fVal = fNew;
                    southeast.gVal = gNew;
                    southeast.hVal = hNew;
                    southeast.Parent = n;

                    //if not in open list, add to open list. Make node n the parent of this successor
                    if(!uncheckedNodes.Contains(southeast)){
                        uncheckedNodes.Add(southeast);
                    }
                    //if in open list but has as a smaller f value, remove and add smaller f value node
                    else{
                        NavGridPathNode old;
                        uncheckedNodes.TryGetValue(southeast, out old);
                        if(old.fVal > southeast.fVal){
                            uncheckedNodes.Remove(old);
                            uncheckedNodes.Add(southeast);
                        }
                    }
                }
            }

            // Southwest successor
            NavGridPathNode southwest = new NavGridPathNode(new Vector3(n.Position.x+GetCellSize(), planeYVal, n.Position.z-GetCellSize()));
            if (IsValidNode(southwest.Position)){
                // found the destination, set parent and return
                if(southwest.Position == gridDestination){
                    // Set parent of the destination node
                    southwest.Parent = n;
                    return southwest;
                }
                // If successor is already checked or blocked, ignore it
                else if (!checkedNodes.Contains(southwest) && !obstacleSet.Contains(southwest.Position)){
            Debug.Log("successor valid8");
                    gNew = n.gVal + GetCellSize();
                    hNew = n.hVal + Vector3.Distance(southwest.Position, gridDestination);
                    fNew = gNew + hNew;

                    southwest.fVal = fNew;
                    southwest.gVal = gNew;
                    southwest.hVal = hNew;
                    southwest.Parent = n;

                    //if not in open list, add to open list. Make node n the parent of this successor
                    if(!uncheckedNodes.Contains(southwest)){
                        uncheckedNodes.Add(southwest);
                    }
                    //if in open list but has as a smaller f value, remove and add smaller f value node
                    else{
                        NavGridPathNode old;
                        uncheckedNodes.TryGetValue(southwest, out old);
                        if(old.fVal > southwest.fVal){
                            uncheckedNodes.Remove(old);
                            uncheckedNodes.Add(southwest);
                        }
                    }
                }
            }
        }

        //didn't find a path to destination so we return origin again
        Debug.Log("not found");
        return new NavGridPathNode(origin, null, 0, 0, 0);
    }


    //randomly add new obstacles in cells until numObstacles is reached
    private void AddObstacles(){
        planeYVal = GetComponent<MeshCollider>().transform.position. y;
        //determine how many obstacles to add, we don't want more than the total number of cells
        int numObstaclesToAddInMethod = numObstaclesToAdd;
        if (obstacleLocations.Length + numObstaclesToAddInMethod > NumberOfCellsPerSide*NumberOfCellsPerSide-1){
            numObstaclesToAddInMethod = (obstacleLocations.Length + numObstaclesToAddInMethod) % (NumberOfCellsPerSide*NumberOfCellsPerSide-1);
        }

        //Setup things needed in method
        HashSet<Vector3> nodes = arrayToHashSet(obstacleLocations);
        var minPoint = GetComponent<MeshCollider>().bounds.min;
        var colliderSize = GetComponent<MeshCollider>().bounds.size;
        Vector3[] newObstacles = new Vector3[numObstaclesToAddInMethod + obstacleLocations.Length];
        //add current obstacles
        for (int i = 0; i < obstacleLocations.Length; i++){
            nodes.Add(obstacleLocations[i]);
        }

        for (int i = 0; i < numObstaclesToAddInMethod; i++)
        {
            //scale to number of cells per side of plane to allow hashing, don't put something on player
            float x = UnityEngine.Random.Range(minPoint.x, minPoint.x + colliderSize.x);
            float z = UnityEngine.Random.Range(minPoint.z, minPoint.z + colliderSize.z);
            Vector3 obs = GetNearestGridPosition(new Vector3(x, planeYVal, z));
            if(nodes.Contains(obs) || obs == GetNearestGridPosition(player.transform.position)){
                i--;
                continue;
            }
            nodes.Add(obs);
        }

        nodes.CopyTo(newObstacles);
        obstacleLocations = newObstacles;
        GenerateObstacles();
    }

    public void GenerateObstacles(){
        for(int i = 0; i < obstacleLocations.Length; i++){
            var obs = Instantiate(obstaclePrefab, obstacleLocations[i], Quaternion.identity);
            obs.transform.localScale = new Vector3(GetCellSize(), 1, GetCellSize());
        }
    }

    /***
     * Utility functions
     */
    private bool IsValidNode(Vector3 position){
        Vector3 origin = new Vector3(position.x, 5, position.z);
        bool result = Physics.Raycast(origin, Vector3.down, out var hit, 10);
        return result;
    }

    private float GetCellSize(){
        var gridCollider = GetComponent<MeshCollider>();
        return gridCollider.bounds.size.x/NumberOfCellsPerSide;
    }

    private Vector3 GetNearestGridPosition(Vector3 position){
        Vector3[,] allGridNodePositions = new Vector3[NumberOfCellsPerSide,NumberOfCellsPerSide];
        var gridCollider = GetComponent<MeshCollider>();
        Vector3 start = new Vector3(gridCollider.bounds.min.x+GetCellSize()/2, planeYVal, gridCollider.bounds.min.z+GetCellSize()/2);
        Vector3 closest = start;

        for (int i = 0; i < NumberOfCellsPerSide; i++){
            for (int j = 0; j < NumberOfCellsPerSide; j++){
                Vector3 nodePos = new Vector3(start.x + (float)i * GetCellSize(), planeYVal, start.z + (float)j * GetCellSize());
                closest = Vector3.Distance(nodePos, position) < Vector3.Distance(closest, position) ? nodePos : closest;
            }
        }

        return closest;
    }

    private HashSet<T> arrayToHashSet<T>(T[] arr){
        HashSet<T> ret = new HashSet<T>();
        foreach (var i in arr){
            ret.Add(i);
        }
        return ret;
    }
}

//Comparer class for sorted set, sort by f value
public class FValComparer : IComparer<NavGridPathNode>
{
    float tolerance = .00000001f;
    int IComparer<NavGridPathNode>.Compare(NavGridPathNode a, NavGridPathNode b){
        if(Mathf.Abs(a.fVal - b.fVal) < tolerance)
            return 0;
        else if(a.fVal < b.fVal)
            return -1;
        else
            return 1;
    }
}
