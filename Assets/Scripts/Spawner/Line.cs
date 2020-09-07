using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    public List<Vector3> NodesPosition => _nodesPosition;

    private int _length;
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Vector3 _direction;
    private float _nodeSize;
    private List<Vector3> _nodesPosition;
    
    public Line(Vector3 startPosition, Vector3 direction, int length, float nodeSize = 1f)
    {
        _startPosition = startPosition;
        _direction = direction;
        _length = length;
        _nodeSize = nodeSize;
        
        _endPosition = startPosition + direction * (length * nodeSize);
        _nodesPosition = GetNodesPosition();
    }
    
    private List<Vector3> GetNodesPosition()
    {
        var nodesPosition = new List<Vector3>();
        
        for (int i = 0; i < _length; i++)
        {
            var nextPosition = Vector3.MoveTowards(_startPosition, _endPosition, i * _nodeSize);
            nodesPosition.Add(nextPosition); 
        }

        return nodesPosition;
    }
}
