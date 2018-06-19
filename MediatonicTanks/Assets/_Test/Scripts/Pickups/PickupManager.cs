using System.Collections;
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

        //Maximum amount of active Pickups
        [SerializeField]
        [Tooltip("Maximum number of active pickups")]
        private int m_MaximumActivePickups = 5;

        [SerializeField]
        [Tooltip("Minimum wait time for pickup spawn")]
        private float m_SpawnMinimumWaitTime = 5.0f;

        [SerializeField]
        [Tooltip("Maximum wait time for pickup spawn")]
        private float m_SpawnMaximumWaitTime = 10.0f;
        //editor variables - end

        //Array of pickup spawn points
        [SerializeField]
        private Transform[] m_SpawnPoints;
        
        //Array of flags to keep track of which spawn point is occupied 
        //true = used, false = free
        [SerializeField]
        private bool[] m_UsedSpawnPoints;

        //Keeps track of the how many active pickups are present
        private int m_ActivePickupsCount = 0;
  
        void Awake()
        {
            //Gets any children of this object as spawnpoint
            m_SpawnPoints = transform.GetComponentsInChildren<Transform>();
            //Sets spawn point flags
            m_UsedSpawnPoints = new bool[m_SpawnPoints.Length];
            for (int i = 0; i < m_UsedSpawnPoints.Length; i++)
            {
                m_UsedSpawnPoints[i] = false;
            }
        }

        private void Start()
        { 
            Assert.IsNotNull(m_DamagePickupPool, "m_DamagePickupPool is null");
            Assert.IsNotNull(m_HealthPickupPool, "m_HealthPickupPool is null");
            Assert.IsNotNull(m_SpeedPickupPool, "m_SpeedPickupPool is null");

            int InitialPickupsAmount = (int)Random.Range(0, m_MaximumActivePickups+1);
            for (int i = 0; i < InitialPickupsAmount; i++)
            {
                SpawnRandomPickup();
            }

            StartCoroutine(SpawningCycle());
        }

      

        private void SpawnRandomPickup()
        {
            //Get a random free position amongst the spawn points
            int SpawnPointsArraySize = m_SpawnPoints.Length;
            int GridIndex = (int) Random.Range(0, SpawnPointsArraySize);
            //increases the found index until a freeposition is found
            while ( m_UsedSpawnPoints[GridIndex] )
            {
                GridIndex = (GridIndex++) % SpawnPointsArraySize;
            }

            //Gets a pickup of random type from the pools
            GameObject PickupObj = null;
            float TypeValue = Random.value;
            if ( 0.33f > TypeValue )
            {
                PickupObj = m_HealthPickupPool.GetObject();
            }
            else if ( 0.66f > TypeValue )
            {
                PickupObj = m_DamagePickupPool.GetObject();
            }
            else
            {
                PickupObj = m_SpeedPickupPool.GetObject();
            }

            PickupObj.transform.position = m_SpawnPoints[GridIndex].position;
            Pickup Scriptcomponent = PickupObj.GetComponent<Pickup>();
            Scriptcomponent.ID = GridIndex;
            m_ActivePickupsCount++;
            m_UsedSpawnPoints[GridIndex] = true;
        }

        //Corouting that spawns new pickups every random time
        IEnumerator SpawningCycle()
        {
            while(true)
            {
                float IntervalTime = Random.Range( m_SpawnMinimumWaitTime, m_SpawnMaximumWaitTime);
                yield return new WaitForSeconds(IntervalTime);
                if (m_ActivePickupsCount < m_MaximumActivePickups)
                {
                    SpawnRandomPickup();
                }
            }
        }

        private void OnDestroy()
        {
            StopCoroutine(SpawningCycle());
        }


    }
}