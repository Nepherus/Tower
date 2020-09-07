using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TowerManager _towerManager;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private Transform _platform;
    [SerializeField] private int _winPercentage = 8;

    private int _currentLevelIndex = 0;
    private LevelConfig _currentLevel;

    private int _totalCylinderCount;
    private int _currentCylinderCount;

    private Coroutine _startSequenceRoutine;
    private Coroutine _cameraHeightRoutine;
    private Coroutine _endRoutine;

    private void Start()
    {
        _uiManager.OnPlay += StartSequence;
        _uiManager.OnFadeInComplete += GoToNextLevel;
        _towerManager.OnCylinderRemoved += RemoveCylinder;
        _playerController.OnNoBullets += RestartLevel;
        StartLevel();
    }

    private void StartLevel()
    {
        _currentLevel = GameSettings.Instance.Levels[_currentLevelIndex % GameSettings.Instance.Levels.Count];
        _currentCylinderCount = _totalCylinderCount = _currentLevel.GetCylinderCount();
        AdjustPlatformSize();
        _towerManager.SpawnTower(_currentLevel);
    }

    private void AdjustPlatformSize()
    {
        var lowestShape = _currentLevel.GetShapeConfig(0);
        float shapeDiameter = lowestShape.SideLength / Mathf.Sin(Mathf.Deg2Rad * (180f / lowestShape.Sides));
        _platform.localScale = new Vector3(shapeDiameter, _platform.localScale.y, shapeDiameter);
    }

    private void StartSequence()
    {
        _startSequenceRoutine = StartCoroutine(StartSequenceRoutine());
    }

    private IEnumerator StartSequenceRoutine()
    {
        float rotationSpeed = 1f;
        float speed = 10f;
        float yOffset = -5f;
        
        while (_playerController.transform.position.y < _towerManager.TopFloor + yOffset)
        {
            _playerController.RotateAroundCenter(rotationSpeed * Time.deltaTime);
            _playerController.transform.position += Vector3.up * (speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        
        _towerManager.EnableGameLoop();
        _playerController.StartGame();
        _uiManager.StartGame();
        _startSequenceRoutine = null;
        _cameraHeightRoutine = StartCoroutine(CameraHeightRoutine());
    }

    private IEnumerator CameraHeightRoutine()
    {
        float yOffset = -5f;
        float minHeight = 5f;
        float speed = 10f;

        while (true)
        {
            if (_playerController.transform.position.y > _towerManager.TopFloor + yOffset && _playerController.transform.position.y > minHeight)
            {
                _playerController.transform.position += Vector3.down * (speed * Time.deltaTime);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void RemoveCylinder()
    {
        _currentCylinderCount--;
        float progress = (float) _currentCylinderCount / (float) _totalCylinderCount;
        
        if (progress * 100f < _winPercentage && _endRoutine == null)
            _endRoutine = StartCoroutine(EndSequenceRoutine());
        else
            _uiManager.UpdateProgressBar(Mathf.InverseLerp(_winPercentage * 0.01f, 1f, progress));
    }

    private IEnumerator EndSequenceRoutine()
    {
        yield return new WaitForEndOfFrame();
        _playerController.Disable();
        _uiManager.SetCurrentLevel(_currentLevelIndex+2);
        _uiManager.FadeIn(2f, 2f);
    }
    
    private void GoToNextLevel()
    {
        _currentLevelIndex++;
        CleanupScene();
        StartLevel();
        _uiManager.SetCurrentLevel(_currentLevelIndex+1);
        _uiManager.FadeOut(0.25f, 2f);
    }
    
    private void RestartLevel()
    {
        CleanupScene();
        _uiManager.PlayNextLevelScreen(_currentLevelIndex+1);
        StartLevel();
    }

    private void CleanupScene()
    {
        _endRoutine = null;
        StopCoroutine(_cameraHeightRoutine);
        _towerManager.Reset();
        _playerController.Reset();
        _uiManager.Reset();
    }
}
