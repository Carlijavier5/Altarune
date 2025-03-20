using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    private SummonType activeSummon;
    private int activeSummonIndex;
    private static AchievementManager instance;
    public static AchievementManager Instance => instance;

    private HashSet<SiftlingType> _siftlings = new () { SiftlingType.Fire , SiftlingType.Normal, SiftlingType.Water, SiftlingType.Wind };

    [Header("UI Params")] [SerializeField] private Transform UI;
    [SerializeField] private float entranceDelay;
    [SerializeField] private float hangDelay;
    [SerializeField] private float offset = -500f;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI text;

    private Vector3 finalLocation;
    private Vector3 exitLocation;
    
    private void Awake() {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() {
        finalLocation = UI.transform.position;
        UI.transform.position = new Vector3(finalLocation.x, finalLocation.y + offset, finalLocation.z);
        exitLocation = UI.transform.position;
        RegisterEvent();
    }
    
    private void DelayRegister() {
        GM.Player.InputSource.OnSummonSelect += CheckTower;
        GM.Player.InputSource.OnSummonPerformed += WindTowerPlaceEvent;
    }
    
    private void RegisterEvent() {
        GM.Instance.OnPlayerInit += DelayRegister;
        GM.TransitionManager.OnFadeEnd += Level1Check;
    }
    
    private void PromptUI() { 
        UI.transform.DOMove(finalLocation, entranceDelay);
        StartCoroutine(HidePrompt());
    }

    private IEnumerator HidePrompt() {
        yield return new WaitForSeconds(hangDelay);
        ExitUI();
    }

    private void ExitUI() {
        UI.transform.DOMove(exitLocation, entranceDelay);
    }

    public bool _level1Reached = false;
    public bool _windTowerPlaced = false;
    public bool _sentintelGolemInterrupted = false;
    public bool _scarabQueenSpawnCheck = false;
    public bool _siftlingCheck;
    public bool _savageGolemCheck;
    public bool _savageGolemWithSniperCheck;
    public bool _lightningOnlyCheck;
    public bool _sovereignGolemDefeatCheck;
    public bool _perfectRunCheck;
    
    //LMAO I'm sorry Carlos
    private void Level1Check() {
        if (!_level1Reached) {
            if (!GM.RoomManager.CurrentRoom) return;
            if ((int) GM.RoomManager.CurrentRoom.RoomTag > 1) {
                _level1Reached = true;
                title.SetText("Achievement: Entering the Catacombs...");
                text.SetText("Clear Floor 1.");
                PromptUI();
            }
        }
    }
    
    private void CheckTower(SummonType summon, int index) {
        activeSummon = summon;
        activeSummonIndex = index;
    }
    public void WindTowerPlaceEvent() {
        if (activeSummon == SummonType.Tower) {
            if (activeSummonIndex == 8) {
                _windTowerPlaced = true;
                title.SetText("Achievement: Second Wind");
                text.SetText("Construct a wind tower.");
                PromptUI();
            }
        }
    }
    
    //TODO: Sentinel golem interrupt event
    public void InterruptSentinelCheck() {
        if (!_sentintelGolemInterrupted) {
            _sentintelGolemInterrupted = true;
            title.SetText("Achievement: Discombobulate");
            text.SetText("Interrupt a sentinel or slither golem during its charge attack.");
            PromptUI();
        }
    }
    
    //TODO: Scarab Queen spawn event
    public void ScarabQueenSpawnCheck() {
        if (!_scarabQueenSpawnCheck) {
            _scarabQueenSpawnCheck = true;
            title.SetText("Achievement: Scaramite Breeder");
            text.SetText("Let the Scarab Queen spawn 5 scaramites.");
            PromptUI();
        }
    }

    public void SiftingKillsCheck(SiftlingType type) {
        if (!_siftlingCheck) {
            _siftlings.Remove(type);
            if (_siftlings.Count <= 0) {
                _siftlingCheck = true;
                title.SetText("Achievement: The Four Nations");
                text.SetText("Destroy a siftling golem of every element.");
                PromptUI();
            }
        }
    }

    public void SavageGolemCheck(bool killedWithSniperTowers) {
        if (!_savageGolemCheck) {
            _savageGolemCheck = true;
            if (killedWithSniperTowers) {
                _savageGolemWithSniperCheck = true;
            }

            if (killedWithSniperTowers) {
                title.SetText("Achievement: I Need More Guns");
                text.SetText("Vanquish the Savage Golem with only Sniper and Railgun Towers.");
                PromptUI();
            } else {
                title.SetText("Achievement: Myth of Al' Turrone");
                text.SetText("Vanquish the Savage Golem.");
                PromptUI();
            }
        }
    }

    public void ChainLightningOnlyCheck() {
        if (!_lightningOnlyCheck) {
            _lightningOnlyCheck = true;
            title.SetText("Achievement: Trapmaster");
            text.SetText("Clear a floor with only the chain-lightning tower.");
            PromptUI();
        }
    }

    public void SovereignGolemCheck() {
        if (!_sovereignGolemDefeatCheck) {
            _sovereignGolemDefeatCheck = true;

            if (!fellInWater) {
                _perfectRunCheck = true;
            }

            if (_perfectRunCheck) {
                title.SetText("Achievement: Flawless Excursion");
                text.SetText("Beat the game without falling off the map.");
                PromptUI();
            }
            else {
                title.SetText("Achievement: The Legend of Yan");
                text.SetText("Vanquish the Sovereign Golem and complete the game.");
                PromptUI();
            }
        }
    }

    private bool fellInWater = false;
    public void FellInWater() {
        if (!fellInWater) fellInWater = true;
    }
}
