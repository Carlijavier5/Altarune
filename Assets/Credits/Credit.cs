using UnityEngine;
using UnityEngine.SceneManagement;

public class Credit : MonoBehaviour
{
    [SerializeField] private CreditRoll creditData;
    [SerializeField] private IndividualCredit individualCredit;
    [SerializeField] private RectTransform content;
    [SerializeField] private bool autoScroll;
    [SerializeField] private float updateTime, scrollSpeed;
    [SerializeField] private SceneRef mainMenuScene;
    private float time;
    private bool done;
    
    void Awake() {
        foreach(CreditData data in creditData.Credits) {
            IndividualCredit credit = Instantiate(individualCredit);
            credit.transform.SetParent(content);
            credit.CreateIndividualCredit(data);
        }
    }

    void Update() {
        if (done) return;

        if (Input.mouseScrollDelta.magnitude > 0) {
            autoScroll = false;
        }

        if (Input.anyKeyDown) {
            SceneManager.LoadScene(mainMenuScene.BuildIndex);
            done = true;
        }

        if (autoScroll) {
            time += Time.deltaTime;
            if (time > updateTime) {
                content.localPosition += new Vector3(0, scrollSpeed, 0);
            }
        }
    }
}