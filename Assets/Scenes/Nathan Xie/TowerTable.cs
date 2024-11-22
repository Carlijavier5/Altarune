using System.Collections.Generic;
using UnityEngine;

public class TowerTable : MonoBehaviour
{
    [SerializeField] private TowerSelectionMenu towerSelectionMenu;
    [SerializeField] private CanvasGroup towerMenu;
    private bool activateInput = false;

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    private void CheckInput(){
        if(activateInput) {
            if(towerMenu.interactable && (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Escape))) {
                HashSet<TowerData> towerHash = towerSelectionMenu.getTowerSelection();
                if(towerMenu.interactable && towerHash.Count != 0) {
                    towerMenu.interactable = false;
                    towerMenu.alpha = 0;
                }
            } else if(Input.GetKeyDown(KeyCode.M)){
                    towerMenu.interactable = true;
                    towerMenu.alpha = 1;
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        activateInput = true;
    }

    void OnTriggerExit(Collider other) {
        activateInput = false;   
    }
}
