using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private ColorConfig _colors;
    [SerializeField] private List<LevelConfig> _levels;
    
    //HERESY!
    private static GameSettings _instance;
    public static GameSettings Instance => _instance;
    
    public ColorConfig Colors => _colors;
    public List<LevelConfig> Levels => _levels;

    private void Awake()
    {
        InitializeSingleton();
        InitializeSettings();
    }

    private void InitializeSingleton()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private void InitializeSettings()
    {
        _colors.Initialize();
    }
}
