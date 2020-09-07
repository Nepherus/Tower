using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private Cylinder _cylinderPrefab;
    [SerializeField] private int _activeFloorCount = 8;
    
    private Transform _towerParent;
    private List<TowerFloor> _towerFloors = new List<TowerFloor>();
    private List<TowerFloor> _activeFloors = new List<TowerFloor>();
    private Stack<TowerFloor> _inactiveFloors = new Stack<TowerFloor>();
    private LevelConfig _currentLevel;
    
    private bool _gameLoopEnabled = false;

    public int TopFloor => _towerFloors.Count;
    public Action OnCylinderRemoved;

    public void SpawnTower(LevelConfig levelConfig)
    {
        _currentLevel = levelConfig;
        _towerParent = new GameObject("Tower").transform;
        _towerParent.parent = transform;
        for (int i = 0; i < _currentLevel.LevelHeight; i++)
        {
            SpawnFloor(i);
        }
    }
    
    private void SpawnFloor(int level)
    {
        var floor = new TowerFloor(_currentLevel.GetShapeConfig(level), _towerParent, level, _currentLevel.GetAngle(level));
        floor.InstantiateFloor(_cylinderPrefab);
        floor.OnCylinderRemoved += RemoveCylinder;
        _towerFloors.Add(floor);
    }
    
    public void EnableGameLoop()
    {
        for (int i = 0; i < _towerFloors.Count; i++)
        {
            if (i < _towerFloors.Count - _activeFloorCount)
            {
                _towerFloors[i].Deactivate();
                _inactiveFloors.Push(_towerFloors[i]);
            }
            else
            {
                _towerFloors[i].Activate();
                _activeFloors.Add(_towerFloors[i]);
            }
        }
        
        _gameLoopEnabled = true;
    }
    
    private void LateUpdate()
    {
        UpdateActiveFloors();
    }

    private void UpdateActiveFloors()
    {
        for (int i = _activeFloors.Count - 1; i >= 0 && _gameLoopEnabled; i--)
        {
            var floor = _activeFloors[i];
            bool floorIsActive = floor.UpdateActiveCylinders();

            if (!floorIsActive)
            {
                floor.OnCylinderRemoved -= RemoveCylinder;
                _towerFloors.Remove(floor);
                _activeFloors.Remove(floor);

                if (_inactiveFloors.Count > 0)
                {
                    var lastInactiveFloor = _inactiveFloors.Pop();
                    lastInactiveFloor.Activate();
                    _activeFloors.Add(lastInactiveFloor);
                    Vibration.Vibrate(100);
                }
            }
        }
    }
    

    private void RemoveCylinder()
    {
        OnCylinderRemoved?.Invoke();
    }

    public void Reset()
    {
        _gameLoopEnabled = false;
        _towerFloors.Clear();
        _activeFloors.Clear();
        _inactiveFloors.Clear();
        Destroy(_towerParent.gameObject);
        _towerParent = null;
    }
}