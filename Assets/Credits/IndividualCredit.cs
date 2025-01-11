using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class IndividualCredit : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI tName,
                                             tRole,
                                             tQuote;
    [SerializeField] private Transform imageAnchor;
    [SerializeField] private Image imageFrame;

    public void CreateIndividualCredit(CreditData creditData){
        string name = creditData.name;
        tName.text = name;
        tName.fontSize += creditData.fontSizeIncrease;

        if (!string.IsNullOrWhiteSpace(creditData.role)) {
            string role = creditData.role;
            tRole.text = role;
        } else tRole.gameObject.SetActive(false);

        if (!string.IsNullOrWhiteSpace(creditData.quote)) {
            string quote = creditData.quote;
            tQuote.text = quote;
        } else tQuote.gameObject.SetActive(false);

        if (creditData.sprite != null) {
            Sprite image = creditData.sprite;
            imageFrame.sprite = image;
        } else imageAnchor.gameObject.SetActive(false);
    }
}