using System.Collections;
using UnityEngine;
using Cinemachine;

public class SovereignEndCutscene : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera bossCamera;
    [SerializeField] private CinemachineVirtualCamera portalCamera;
    [SerializeField] private SovereignEndPortal endPortal;
    [SerializeField] private float initialWait, animEndWait;

    public void DoAnimation(float animationLength) {
        GM.AudioManager.FadeMusic(1f);
        ToggleInputs(false);
        StartCoroutine(IDoAnimation(animationLength));
    }

    private IEnumerator IDoAnimation(float animationLength) {
        GM.TimeScaleManager.GlobalTimeScale = 0.25f;
        bossCamera.Priority = 100;
        yield return new WaitForSecondsRealtime(initialWait);
        GM.TimeScaleManager.GlobalTimeScale = 1;
        yield return new WaitForSeconds(animationLength + animEndWait);
        bossCamera.Priority = 0;
        portalCamera.Priority = 100;
        endPortal.Show();
        endPortal.OnPortalShown += EndPortal_OnPortalShown;
    }

    private void EndPortal_OnPortalShown() {
        endPortal.OnPortalShown -= EndPortal_OnPortalShown;
        portalCamera.Priority = 0;
        ToggleInputs(true);
    }

    private void ToggleInputs(bool on) {
        if (on) {
            GM.Player.InputSource.ActivateInput();
            GM.Player.InputSource.ActivateSummons();
        } else {
            GM.Player.InputSource.DeactivateInput();
            GM.Player.InputSource.DeactivateSummons();
        }
    }
}
