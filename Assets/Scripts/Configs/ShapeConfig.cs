using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shape", menuName = "Spawner/Shape")]
public class ShapeConfig : ScriptableObject
{
    [SerializeField] private int _sides;
    [SerializeField] private int _sideLength;

    public int Sides => _sides;
    public int SideLength => _sideLength;
}
