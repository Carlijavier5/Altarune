using UnityEngine.UI;
using UnityEngine;

public class BarTimer : MonoBehaviour
{
    public event System.Action OnTimerEnd;

    [SerializeField] private float fadeInTime;
    [SerializeField] private BaseObject attatchedObject;
    [SerializeField] private Image topLayer;
    [SerializeField] private Image backgroundLayer;
    [SerializeField] private CanvasGroup canvasGroup;

    private float maxTime;
    private float currentTime;
    private float currentFadeTime;
    private bool init = false;
    private float currentFadeOutTime;
    void Start(){
        init = false;
        canvasGroup.alpha = 0;
    }
    public void StartTimer(float inputTime){
        maxTime = inputTime;
        currentTime = maxTime;
        canvasGroup.alpha = 0.5f;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        topLayer.fillAmount = 1;
        init = true;
        currentFadeOutTime = 0;
        currentFadeTime = 0;
    }

    private void Fade(){
        canvasGroup.alpha = Mathf.Lerp(0.5f, 1.0f, currentFadeTime / fadeInTime);
        if(currentFadeTime / fadeInTime <= 0.5) {
            float canvasScale = Mathf.Lerp(0.9f, 1.1f,currentFadeTime / (fadeInTime * 0.5f));
            transform.localScale = canvasScale * new Vector3(1f, 1f, 1f);
        } else {
            float canvasScale = Mathf.Lerp(1.1f, 1f, (currentFadeTime / fadeInTime - 0.5f) / 0.5f);
            transform.localScale = canvasScale * new Vector3(1f, 1f, 1f);
        }
        currentFadeTime += attatchedObject.DeltaTime;
    }

    private void FadeOut(){
        canvasGroup.alpha = Mathf.Lerp(1.0f, 0, currentFadeOutTime / fadeInTime);
        if(currentFadeOutTime / fadeInTime <= 0.5) {
            float canvasScale = Mathf.Lerp(1.1f, 1f,currentFadeOutTime / (fadeInTime * 0.5f));
            transform.localScale = canvasScale * new Vector3(1f, 1f, 1f);
        } else {
            float canvasScale = Mathf.Lerp(1f, 0.5f, (currentFadeOutTime / fadeInTime - 0.5f) / 0.5f);
            transform.localScale = canvasScale * new Vector3(1f, 1f, 1f);
        }
        currentFadeOutTime += attatchedObject.DeltaTime;
    }

    void Update(){
        if(init){
            if(currentFadeTime < fadeInTime) {
                Fade();
            } else if (currentTime <= 0) {
                FadeOut();
            } else {
                UpdateTime();
                UpdateTopLayer();
            }
        }
    }

    private void UpdateTime(){
        if(currentTime > 0) {
            currentTime -= attatchedObject.DeltaTime;
            if(currentTime < 0) {
                currentTime = 0;
            }
        }
    }

    private void UpdateTopLayer(){
        topLayer.fillAmount = currentTime / maxTime;
        if(topLayer.fillAmount <= 0) {
            OnTimerEnd?.Invoke();
        }
    }
}
