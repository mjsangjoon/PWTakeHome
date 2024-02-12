using UnityEngine;
using System.Collections.Generic;

public class NavGrid : MonoBehaviour
{
    public int NumberOfCellsPerSide;
    public Vector3[] obstacleLocations;

    /// <summary>
    /// Given the current and desired location, return a path to the destination
    /// </summary>
    public NavGridPathNode[] GetPath(Vector3 origin, Vector3 destination)
    {
        //Use A* algorithm
        return new NavGridPathNode[]
        {
            new() { Position = origin },
            new() { Position = destination }
        };
    }

    public void AddObstacles(int numObstacles){
        //randomly add new obstacles in cells until numObstacles is reached
        HashSet<Vector3> nodes = new HashSet<Vector3>(numObstacles);

        while (nodes.Count < numObstacles-1)
        {
            //scale to number of cells per side of plane to allow hashing
            float xVal = Random.Range(0, 1) * NumberOfCellsPerSide;
            float zVal = Random.Range(0, 1) * NumberOfCellsPerSide;

            nodes.Add(new Vector3(xVal, 0, zVal));
        }

        nodes.CopyTo(obstacleLocations);
    }
}
