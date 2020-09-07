using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape
{
    public List<Vector3> NodesPosition => _nodesPosition;

    private Vector3 _center;
    private int _sides;
    private int _sideLength;
    private float _nodeSize;

    private float _sideAngle;
    private List<Vector3> _nodesPosition;
    

    public Shape(Vector3 center, int sides, int sideLength, float nodeSize = 1f)
    {
        _center = center;
        _sides = sides;
        _sideLength = sideLength;
        _nodeSize = nodeSize;

        _sideAngle = GetSideAngle();
        _nodesPosition = GetNodesPosition();
    }

    private List<Vector3> GetNodesPosition()
    {
        var nodesPosition = new List<Vector3>();
        
        var nextStartposition = GetFirstNodePosition();
        var nextDirection = Vector3.right;
        var nextSideLength = _sideLength;

        for (int i = 0; i < _sides; i++)
        {
            var line = new Line(nextStartposition, nextDirection, nextSideLength, _nodeSize);
            nodesPosition.AddRange(line.NodesPosition);

            nextDirection = GetNextDirection(nextDirection);
            nextStartposition = nodesPosition[nodesPosition.Count-1] + nextDirection * _nodeSize;
            if (IsSecondOrLastSide(i + 1)) nextSideLength--;
        }
        
        return nodesPosition;
    }
    
    #region Helpers

    private Vector3 GetFirstNodePosition()
    {
        var directionToFirstNode = Quaternion.AngleAxis(_sideAngle * -0.5f, Vector3.up) * Vector3.left;
        var distanceToFirstNode = GetRadius();
        return _center + directionToFirstNode * distanceToFirstNode;
    }
    
    private float GetSideAngle() => ((_sides - 2f) * 180f) / _sides;

    private float GetRadius()
    {
        float radius = ((_sideLength-1) * _nodeSize) / (2f * Mathf.Sin(Mathf.Deg2Rad * (180f / _sides)));
        // if (_sideLength == 2) radius *= 0.5f;
        return radius;
    }

    private Vector3 GetNextDirection(Vector3 currentDirection) => Quaternion.AngleAxis(-360f / _sides, Vector3.up) * currentDirection;
    
    private bool IsSecondOrLastSide(int side) => side == 1 || side == _sides - 1;
    
    #endregion
}
