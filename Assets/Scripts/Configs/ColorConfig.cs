using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Color Config", menuName = "Configs/Colors")]
public class ColorConfig : ScriptableObject
{
    [SerializeField] private List<Color> _colors;
    [SerializeField] private Color _disabledColor;

    public int ColorCount => _colors.Count;

    private List<MaterialPropertyBlock> _colorBlocks;
    private MaterialPropertyBlock _disabledBlock;
    
    public void Initialize()
    {
        InitializeColorBlocks();
        InitializeDisabledBlock();
    }

    private void InitializeColorBlocks()
    {
        _colorBlocks = new List<MaterialPropertyBlock>();
        
        for (int i = 0; i < _colors.Count; i++)
        {
            var block = new MaterialPropertyBlock();
            block.SetColor("_Color", _colors[i]);
            _colorBlocks.Add(block);
        }
    }

    private void InitializeDisabledBlock()
    {
        _disabledBlock = new MaterialPropertyBlock();
        _disabledBlock.SetColor("_Color", _disabledColor);
    }

    public MaterialPropertyBlock GetColorBlock(int colorId) => _colorBlocks[colorId];
    public MaterialPropertyBlock GetDisabledColorBlock() => _disabledBlock;
}
