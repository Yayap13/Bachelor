using SmartNet;
using UnityEngine;

namespace Network
{
	public class NetSpawner : MonoBehaviour
	{
		public SmartNetIdentity[] PrefabsToSpawn;
	
		// Use this for initialization
		void Start ()
		{
			foreach (var prefab in PrefabsToSpawn)
			{				
				NetworkScene.Spawn(prefab, prefab.transform.position, prefab.transform.rotation);
			}
		}
	}
}
