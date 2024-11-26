using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class Credit : MonoBehaviour
{
    [SerializeField] private CreditRoll creditData;
    [SerializeField] private IndividualCredit individualCredit;
    [SerializeField] private RectTransform content;
    public bool scroll;
    private float updateTime = 0.01f;
    private float time = 0f;
    // Start is called before the first frame update

    
    void Start()
    {
        scroll = true;
        foreach(CreditData i in creditData.Credits) {
            IndividualCredit k = Instantiate(individualCredit);
            k.transform.SetParent(content);
            k.createIndividualCredit(i);
        }
    }

    public void setScroll(bool activateScroll) {
        scroll = activateScroll;
    }

    // Update is called once per frame
    void Update()
    {
        if(scroll){
            time += Time.deltaTime;
            if (time > updateTime) {
                content.localPosition += new Vector3(0, 0.1f, 0);
            }
        }
    }
}
