using System.Collections;
using System.Collections.Generic;
using SmartNet;
using UnityEngine;
using Network = UnityEngine.Network;

public class CubeController : MonoBehaviour
{
    void OnEnable() {
        if(NetworkClient.Active)
            Camera.main.GetComponent<FollowGameObject>().SetObject(gameObject);
    }
}
