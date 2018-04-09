using Gameplay;
using SmartNet;
using UnityEngine;

namespace Gameplay
{
    public abstract class StateSyncObject : MonoBehaviour, IStateSynchronize
    {
        private Rigidbody _rb;
        [SerializeField] private float _priority = 0;
        [SerializeField] private ushort _uniqueId = 0;
        public SyncObject Prefab { protected set; get; }

        public Rigidbody Rigidbody => _rb;

        public float Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public ushort UniqueId
        {
            get { return _uniqueId; }
            set { _uniqueId = value; }
        }

        public abstract void CalculatePriority();

        protected virtual void Awake()
        {
            _uniqueId = StateSyncManager.GetUniqueId();
            _rb = GetComponent<Rigidbody>();
            Debug.Assert(Prefab!=SyncObject.None);
        }

        protected virtual void OnEnable()
        {
            StateSyncManager.Register(this);
        }

        protected virtual void OnDisable()
        {
            StateSyncManager.Unregister(this);
        }
    }

    public enum SyncObject : byte
    {
        None,
        PlayerCube,
        LittleCube,
    };
}