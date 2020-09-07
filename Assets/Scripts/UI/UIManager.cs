using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ProgressBar _progress;
    [SerializeField] private Button _playButton;
    [SerializeField] private Image _whiteScreen;
    [SerializeField] private TextMeshProUGUI _levelAnnouncement;

    public Action OnPlay;
    public Action OnFadeInComplete;

    private int _currentLevel;

    public void UpdateProgressBar(float progress) => _progress.SetPercentage(progress);

    public void Play()
    {
        _playButton.gameObject.SetActive(false);
        OnPlay?.Invoke();
    }

    public void StartGame()
    {
        _progress.gameObject.SetActive(true);
    }

    public void SetCurrentLevel(int level)
    {
        _progress.SetLevel(level);
        _levelAnnouncement.text = "Level " + level;
    }

    public void Reset()
    {
        UpdateProgressBar(1f);
    }

    public void FadeIn(float duration, float delay)
    {
        _progress.gameObject.SetActive(false);
        LeanTween.value(gameObject, 0f, 1f, duration)
            .setOnUpdate(ChangeNewLevelScreenAlpha)
            .setOnComplete(() => OnFadeInComplete?.Invoke())
            .setDelay(delay);
    }

    public void FadeOut(float duration, float delay)
    {
        LeanTween.value(gameObject, 1f, 0f, duration)
            .setOnUpdate(ChangeNewLevelScreenAlpha)
            .setOnComplete(() => _playButton.gameObject.SetActive(true))
            .setDelay(delay);
    }

    public void PlayNextLevelScreen(int level)
    {
        float fadeInTime = 0.25f;
        float delayTime = 2f;
        float fadeOutTime = 0.25f;
        
        _levelAnnouncement.text = "Level " + level;

        LeanTween.value(gameObject, 0f, 1f, fadeInTime)
            .setOnUpdate(ChangeNewLevelScreenAlpha)
            .setOnComplete(() =>
                LeanTween.value(gameObject, 1f, 0f, fadeOutTime)
                    .setOnUpdate(ChangeNewLevelScreenAlpha)
                    .setDelay(delayTime)
                    .setOnComplete(() => _playButton.gameObject.SetActive(true))
            );
    }

    private void ChangeNewLevelScreenAlpha(float alpha)
    {
        _whiteScreen.color = 
            new Color(_whiteScreen.color.r, _whiteScreen.color.g, _whiteScreen.color.b, alpha);
        _levelAnnouncement.color = 
            new Color(_levelAnnouncement.color.r, _levelAnnouncement.color.g, _levelAnnouncement.color.b, alpha);
    }
}
