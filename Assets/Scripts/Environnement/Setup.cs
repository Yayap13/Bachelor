using SmartNet;
using UnityEngine;

public class Setup : MonoBehaviour
{
    public GameObject ground;
    public SmartNetIdentity inputManager;
    public int areaWidth;
    public int cubePerWidth;
    public int moveInputIntensity = 50;
    public SyncMode syncMode = SyncMode.None;
    
    [Header("Snapshot Interpolation")]
    public GameObject littleCubePrefabSI;
    public GameObject playerCubePrefabSI;
    public SmartNetIdentity snapshotInterpolationManagerPrefab;
    
    [Header("State Syncronization")]
    public GameObject littleCubePrefabSS;
    public GameObject playerCubePrefabSS;
    public SmartNetIdentity stateSyncronizationManagerPrefab;

    // Use this for initialization
    void Start()
    {
        if (NetworkServer.Active)
        {
            NetworkScene.Spawn(inputManager, Vector3.zero, Quaternion.identity);
            Debug.Log("Spawned Inputmanager");
            switch (syncMode)
            {
                case SyncMode.None:
                    SpawnLittleCubes();
                    break;

                case SyncMode.SnapshotInterpolation:
                    NetworkScene.Spawn(snapshotInterpolationManagerPrefab, Vector3.zero, Quaternion.identity);
                    break;

                case SyncMode.StateSyncronization:
                    NetworkScene.Spawn(stateSyncronizationManagerPrefab, Vector3.zero, Quaternion.identity);
                    break;

                default:
                    break;
            }
        }

    Instantiate(ground);
    }

    private void SpawnLittleCubes()
    {
        Transform entityHolder = new GameObject("Entities").transform;

        for (int x = -areaWidth / 2; x < areaWidth / 2; x += areaWidth / cubePerWidth)
        {
            for (int z = -areaWidth / 2; z < areaWidth / 2; z += areaWidth / cubePerWidth)
            {
                GameObject gm = Instantiate(littleCubePrefabSI, Vector3.zero, Quaternion.identity);
                gm.transform.SetParent(entityHolder);
            }
        }
    }
}

public enum SyncMode
{
    None,
    SnapshotInterpolation,
    StateSyncronization
}