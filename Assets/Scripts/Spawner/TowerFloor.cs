using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerFloor
{
    public List<Cylinder> Cylinders => _cylinders;

    private Shape _shape;
    private Transform _parent;
    private int _level;
    private float _rotationAngle;
    
    private Transform _transform;
    private List<Cylinder> _cylinders;

    private float _cylinderRemovedThreshold = 0.5f;

    public Action OnCylinderRemoved;

    public TowerFloor(ShapeConfig shapeConfig, Transform parent, int level, float rotationAngle)
    {
        _parent = parent;
        _level = level;
        _rotationAngle = rotationAngle;
        _shape = CreateShape(shapeConfig);
    }

    public void InstantiateFloor(Cylinder cylinderPrefab)
    {
        _transform = InstantiateTransform();
        _cylinders = InstantiateCylinders(cylinderPrefab);
        
        _transform.Rotate(Vector3.up, _rotationAngle);
    }

    public void Activate()
    {
        for (int i = 0; i < _cylinders.Count; i++)
        {
            _cylinders[i].Activate();
        }
    }

    public void Deactivate()
    {
        for (int i = 0; i < _cylinders.Count; i++)
        {
            _cylinders[i].Deactivate();
        }
    }

    public bool UpdateActiveCylinders()
    {
        for (int i = _cylinders.Count - 1; i >= 0; i--)
        {
            if (_cylinders[i].transform.position.y < _transform.position.y - _cylinderRemovedThreshold)
            {
                _cylinders[i].StartFloating();
                OnCylinderRemoved?.Invoke();
                _cylinders.Remove(_cylinders[i]);
            }
        }

        return _cylinders.Count != 0;
    }
    
    private void DestroyCylinder(Cylinder cylinder)
    {
        _cylinders.Remove(cylinder);
        cylinder.OnDestroy -= DestroyCylinder;
        //Wish I had time to pool all of this
        GameObject.Destroy(cylinder.gameObject);
        OnCylinderRemoved?.Invoke();
    }

    #region Helpers
    
    private Shape CreateShape(ShapeConfig shapeConfig)
    {
        var centerPoint = _parent.position + Vector3.up * _level;
        var shape = new Shape(centerPoint, shapeConfig.Sides, shapeConfig.SideLength);
        return shape;
    }

    private Transform InstantiateTransform()
    {
        var go = new GameObject("TowerFloor_" + _level);
        var transform = go.transform;
        transform.parent = _parent;
        transform.position = Vector3.up * _level;
        return transform;
    }

    private List<Cylinder> InstantiateCylinders(Cylinder cylinderPrefab)
    {
        var cylinders = new List<Cylinder>();
        for (int i = 0; i < _shape.NodesPosition.Count; i++)
        {
            var cylinder = GameObject.Instantiate(cylinderPrefab, _transform);
            cylinder.transform.position = _shape.NodesPosition[i];
            cylinder.SetRandomColor();
            cylinders.Add(cylinder);
            cylinder.OnDestroy += DestroyCylinder;
        }

        return cylinders;
    }

    #endregion
}