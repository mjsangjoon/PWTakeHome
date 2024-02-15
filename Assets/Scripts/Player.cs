using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private NavGridPathNode[] _currentPath = Array.Empty<NavGridPathNode>();
    private List<PathSection> pathSections = new List<PathSection>();
    
    [SerializeField]
    private NavGrid _grid;
    [SerializeField]
    private float _speed = 10.0f;

    private float pathPosition = 0.0f;

    void Update()
    {
        // Check Input
        if (Input.GetMouseButtonUp(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo))
            {
                pathSections.Clear();
                _currentPath = _grid.GetPath(transform.position, hitInfo.point);
                for(int i = 0; i < _currentPath.Length; i++)
                {
                    Debug.Log("Node " + i +": " + _currentPath[i].ToString());
                }
                CreateSmoothPath();
                pathPosition = 0.0f;
            }
        }

        // Traverse
        /*if (_currentPathIndex < _currentPath.Length)
        {
            var currentNode = _currentPath[_currentPathIndex];
            
            var maxDistance = _speed * Time.deltaTime;
            var vectorToDestination = currentNode.Position - transform.position;
            var moveDistance = Mathf.Min(vectorToDestination.magnitude, maxDistance);

            var moveVector = vectorToDestination.normalized * moveDistance;
            moveVector.y = 0f; // Ignore Y
            transform.position += moveVector;

            if (transform.position == currentNode.Position)
                _currentPathIndex++;
        }*/
        if (CanMoveOnPath())
        {
            pathPosition += Time.deltaTime * _speed;
            UpdateTransform();
        }
    }

    private void CreateSmoothPath()
    {
        if (_currentPath.Length <= 0)
            return;
        if (_currentPath.Length == 2)
        {
            pathSections.Add(new StraightPathSection(transform.position, _currentPath[1].Position, transform.position.y));
        }
        else
        {
            bool isStraight = !(_currentPath[0].Position.x != _currentPath[2].Position.x && _currentPath[0].Position.z != _currentPath[2].Position.z);
            Vector3 start = _currentPath[0].Position;
            for (int i = 2; i < _currentPath.Length; i++)
            {
                bool xInLine = _currentPath[i].Position.x == _currentPath[i - 1].Position.x && _currentPath[i - 1].Position.x == _currentPath[i - 2].Position.x;
                bool zInLine = _currentPath[i].Position.z == _currentPath[i - 1].Position.z && _currentPath[i - 1].Position.z == _currentPath[i - 2].Position.z;
                if (isStraight && xInLine || zInLine)
                {
                    continue;
                }
                else if (isStraight && !(xInLine || zInLine))
                {
                    if(start != _currentPath[i - 1].Position)
                    {
                        pathSections.Add(new StraightPathSection(start, _currentPath[i - 1].Position, transform.position.y));
                        start = Vector3.zero;
                    }
                }
                else
                {
                    pathSections.Add(new CurvedPathSection(_currentPath[i].Position, _currentPath[i - 1].Position, _currentPath[i - 2].Position, transform.position.y));
                    start = _currentPath[i].Position;
                }

                isStraight = xInLine || zInLine;
            }
            if(start != Vector3.zero)
            {
                pathSections.Add(new StraightPathSection(start, _currentPath[_currentPath.Length - 1].Position, transform.position.y));
            }
        }
        Debug.Log("test");
    }

    public PathSection GetCurrentSection()
    {
        if (pathSections.Count == 0 || Mathf.FloorToInt(pathPosition) >= pathSections.Count)
            return null;
        return pathSections[Mathf.FloorToInt(pathPosition)];
    }

    void UpdateTransform()
    {
        //last movement on path causes null reference sometimes, add if statement to fix
        if (CanMoveOnPath())
            transform.position = GetCurrentSection().GetPositionOnPath(pathPosition);
    }

    bool CanMoveOnPath()
    {
        return GetCurrentSection() != null;

    }
}
