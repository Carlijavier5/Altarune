using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigGatlingArea : MonoBehaviour {

    private GatlingGun gatlingGun;

    public void Init(float duration, float size, GatlingGun gatlingGun) {
        this.gatlingGun = gatlingGun;
        transform.localScale = new Vector3(size, 0.1f, size);
        Invoke("Expire", duration);
    }

    private void Expire() {
        gatlingGun.StopAggro();
        Destroy(gameObject);
    }
}
