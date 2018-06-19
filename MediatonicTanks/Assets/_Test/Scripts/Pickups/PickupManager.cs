using UnityEngine;
using UnityEngine.Assertions;

namespace Test.Pickups
{
    //Manager for pickups
    public class PickupManager : MonoBehaviour
    {
        ///editor variables
        // Pickup pools references
        [SerializeField]
        private GenericPool m_HealthPickupPool;
        [SerializeField]
        private GenericPool m_DamagePickupPool;
        [SerializeField]
        private GenericPool m_SpeedPickupPool;
        //

        //Array of pickup spawn points
        [SerializeField]
        private Transform[] m_SpawnPoints;
        //Array of flags to keep track of which spawn point is occupied
        [SerializeField]
        private bool[] m_SpawnPointStatus;
        
        void Awake()
        {
            //Gets any children of this object as spawnpoint
            m_SpawnPoints = transform.GetComponentsInChildren<Transform>();
            //Sets spawn point flags
            m_SpawnPointStatus = new bool[m_SpawnPoints.Length];
            for (int i = 0; i < m_SpawnPointStatus.Length; i++)
            {
                m_SpawnPointStatus[i] = false;
            }
        }

        private void Start()
        { 
            Assert.IsNotNull(m_DamagePickupPool, "m_DamagePickupPool is null");
            Assert.IsNotNull(m_HealthPickupPool, "m_HealthPickupPool is null");
            Assert.IsNotNull(m_SpeedPickupPool, "m_SpeedPickupPool is null");


        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}