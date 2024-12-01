using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class IndividualCredit : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image YanImage;

    private string name, role, quote;
    private Sprite image;

    public void createIndividualCredit(CreditData creditData){
        name = creditData.name;
        role = creditData.role;
        text.text = name + "\n" + role + "\n";
        if(creditData.quote != null) {
            quote = creditData.quote;
            text.text += quote;
        }
        if(creditData.sprite != null) {
            image = creditData.sprite;
            YanImage.sprite = image;
            text.alignment = TMPro.TextAlignmentOptions.Left;
            this.GetComponent<HorizontalLayoutGroup>().childAlignment = UnityEngine.TextAnchor.UpperLeft;
        } else {
            Destroy(YanImage.gameObject);
            text.alignment = TMPro.TextAlignmentOptions.Center;
            Vector2 originalSize = GetComponent<RectTransform>().sizeDelta;
            originalSize.y = 200;
            GetComponent<RectTransform>().sizeDelta = originalSize;
            this.GetComponent<HorizontalLayoutGroup>().enabled = true;
            this.GetComponent<HorizontalLayoutGroup>().childAlignment = UnityEngine.TextAnchor.MiddleCenter;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
