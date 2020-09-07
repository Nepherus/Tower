using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Image _progressImage;

    [SerializeField] private Gradient _progressGradient;

    public void SetLevel(int level)
    {
        _levelText.text = "Level " + level;
    }
    
    public void SetPercentage(float progress)
    {
        float actualProgress = 1f - progress;
        int percentage = (int)((actualProgress) * 100f);
        _progressImage.fillAmount = actualProgress;
        _progressImage.color = _progressGradient.Evaluate(actualProgress);
        _progressText.text = percentage.ToString() + "%";
    }
}
