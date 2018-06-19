using UnityEngine;
using UnityEngine.Assertions;

namespace Test
{


    //Class that represent a generic pool of recyclable objects.
    //Uses 2 arrays to store the object - the deadList and the live list.
    //Dead list is made by deactivated object that can be retrieved. 
    //At the beginning all objects are in the dead list. 
    //When they are in use they go to the live list. 
    //Live list is used when it's necessary to reset the scene
    public class GenericPool : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Size of the pool")]
        private int m_Size = 5;

        [SerializeField]
        private IPoolableObject[] m_aObjDeadList;
        [SerializeField]
        private IPoolableObject[] m_aObjLiveList;
        [SerializeField]
        private IPoolableObject m_objPrefab;  //type of object that will be pooled

        [SerializeField]
        [Tooltip("Flag to set behaviour to handle requests when empty. If true will return null, else will assert")]
        private bool m_CanReturnNull;

        private int m_FreeCount;


        // Use this for initialization
        void Start()
        {
            Init();
        }

        //inits the array
        private void Init()
        {
            Assert.raiseExceptions = true;
            Assert.IsTrue(0 < m_Size);


            m_FreeCount = m_Size;

            m_aObjDeadList = new IPoolableObject[m_Size];
            m_aObjLiveList = new IPoolableObject[m_Size];
            for (int iLoop = 0; iLoop < m_Size; iLoop++)
            {
                m_aObjLiveList[iLoop] = null;

                m_aObjDeadList[iLoop] = CreateObject();
            }
        }

        //Creates an object of the ObjPrefab type. Used to populate the arrays
        private IPoolableObject CreateObject()
        {
            GameObject objResult = null;
            objResult = GameObject.Instantiate(m_objPrefab.gameObject, this.transform, false);
            objResult.SetActive(false);

            IPoolableObject objComponent = objResult.GetComponent<IPoolableObject>();
            objComponent.SetOwner(this);
            return objComponent;
        }

        //Gets an object from the pool, taking it from the dead list
        public GameObject GetObject()
        {
            IPoolableObject objResult = null;


            if (0 == m_FreeCount)
            {
                if (m_CanReturnNull)
                {
                    return null;
                }

                else
                {
                    Assert.IsTrue(false, "Requesting object from a depleted pool. Probably replanning usage is required, and  resizing accordingly");
                }

            }

            for (int iLoop = 0; iLoop < m_Size; iLoop++)
            {
                //first non null object , saves it 
                if (null != m_aObjDeadList[iLoop])
                {
                    objResult = m_aObjDeadList[iLoop];
                    m_aObjDeadList[iLoop] = null;

                    for (int jLoop = 0; jLoop < m_Size; jLoop++)
                    {
                        //first null object , saves there 
                        if (null == m_aObjLiveList[jLoop])
                        {
                            m_aObjLiveList[jLoop] = objResult;
                            break;
                        }
                    }

                    break;
                }
            }

            Assert.IsTrue(null != objResult, "Didn't find any available object from pool");

            if (objResult)
            {
                objResult.gameObject.SetActive(true);
                objResult.transform.parent = null;
                objResult.IsKilled = false;
            }

            m_FreeCount--;
            return objResult.gameObject;
        }


        //Returns an object to pool
        public void ReturnToPool(IPoolableObject objParam)
        {
            IPoolableObject objResult = null;

            //look if it's in the live list, end eventually remove it
            for (int iLoop = 0; iLoop < m_Size; iLoop++)
            {
                if (objParam == m_aObjLiveList[iLoop])
                {
                    objResult = m_aObjLiveList[iLoop];
                    m_aObjLiveList[iLoop] = null;
                    break;
                }
            }

            Assert.IsNotNull(objResult, "Object not belonging to pool returned!");

            //deactivate
            objResult.gameObject.SetActive(false);



            //put back in deadlist
            for (int iLoop = 0; iLoop < m_Size; iLoop++)
            {
                if (null == m_aObjDeadList[iLoop])
                {
                    m_aObjDeadList[iLoop] = objResult;
                    break;
                }
            }

            m_FreeCount++;
            objResult.transform.parent = this.transform;

        }


        //this is used if a reset is needed and some objects from the pool are alive in the scene
        public void ResetState()
        {

            //loop in the live list
            for (int iLoop = 0; iLoop < m_Size; iLoop++)
            {
                //if there is something, return it to pool forcefully
                if (null != m_aObjLiveList[iLoop])
                {
                    ReturnToPool(m_aObjLiveList[iLoop]);
                }
            }


            m_FreeCount = m_Size;
        }
    }
}