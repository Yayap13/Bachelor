using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGameObject : MonoBehaviour {

    private GameObject _objectToFollow;

	void LateUpdate () {
	    if(_objectToFollow!=null)
            transform.LookAt(_objectToFollow.transform);
	}

    public void SetObject(GameObject ObjectToFollow)
    {
        _objectToFollow = ObjectToFollow;
    }
}
