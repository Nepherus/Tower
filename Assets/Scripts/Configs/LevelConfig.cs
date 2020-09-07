using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Configs/Level")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private List<ShapeConfig> _levelPattern;
    [SerializeField] private int _levelHeight;
    [SerializeField] private float _angle;

    public int LevelHeight => _levelHeight;

    public ShapeConfig GetShapeConfig(int level) => _levelPattern[level % _levelPattern.Count];
    public float GetAngle(int level) => _angle * level;

    public int GetCylinderCount()
    {
        int cylinderCount = 0;
        for (int i = 0; i < _levelHeight; i++)
        {
            var shape = GetShapeConfig(i);
            cylinderCount += shape.Sides * (shape.SideLength - 1);
        }

        return cylinderCount;
    }
}
