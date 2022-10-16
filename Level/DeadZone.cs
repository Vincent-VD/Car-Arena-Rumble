using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    const string PLAYERTAG = "Player";

    //When hit, the car will be reset/respawn
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != PLAYERTAG)
        {
            return;
        }

        Health temphp = other.GetComponent<Health>();
        if (temphp.IsAlive)
        {
            temphp.PlayDeathPartciles(other.attachedRigidbody.position);
            PlayerManager.Instance.CameraBeh.ShakeCamera();
            SoundManager.Instance.PlayerCarKillSound();
            temphp.IsAlive = false;
            other.GetComponent<BasicCarCharachter>().RestetIn(PlayerManager.Instance._carRespawnTime);
        }
    }

}
