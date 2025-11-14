using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SavageDeathCutscene : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera focusCamera;
    [SerializeField] private Player player;
    [SerializeField] private GolemSavage savageGolem;
    [SerializeField] private Animator savageAnimator;
    [SerializeField] private AnimationClip perishClip;
    [SerializeField] private AnimationCurve timeFreezeCurve;
    [SerializeField] private AnimationCurve timeResumeCurve;
    [SerializeField] private float timeFreezeTimer;
    [SerializeField] private float triggerWait, endWait;

    private TimeScaleCore timeScaleCore;

    void Awake() {
        savageGolem.OnPerish += SavageGolem_OnPerish;
    }

    private void SavageGolem_OnPerish(BaseObject _) {
        GM.AudioManager.FadeMusic(0.5f);
        timeScaleCore = GM.TimeScaleManager.AddTimeScaleMultiplier(0, timeFreezeTimer, timeFreezeCurve, true);
        StartCoroutine(IDoCutscene());
    }

    private IEnumerator IDoCutscene() {
        yield return new WaitForSecondsRealtime(triggerWait);
        player.ToggleUI(false);
        player.InputSource.DeactivateInput();
        player.InputSource.DeactivateSummons();

        savageAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        focusCamera.Priority = 20;

        yield return new WaitForSecondsRealtime(perishClip.length + endWait);

        player.ToggleUI(true);
        player.TriggerManaCollapse(false);
        player.InputSource.ActivateInput();
        player.InputSource.ActivateSummons();

        savageAnimator.gameObject.layer = LayerUtils.EnvironmentLayer;
        savageAnimator.updateMode = AnimatorUpdateMode.Normal;
        focusCamera.Priority = 0;

        timeScaleCore?.Kill();
        GM.TimeScaleManager.AddTimeScaleMultiplier(0, timeFreezeTimer, timeResumeCurve);
    }
}