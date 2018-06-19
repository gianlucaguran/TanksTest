using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{

    //Interface used to define some common behaviours between objects that should be pooled
    public abstract class IPoolableObject : MonoBehaviour
    {
        //Reference to the owning pool. 
        //It's used so the object "knows" in which pool it has to return
        private GenericPool m_poolOwner;

        //flag used to avoid an object to process more killing events at the same time 
        //and trying to return to the pool more times
        private bool m_IsKilled = true;

        public bool IsKilled
        {
            get
            {
                return m_IsKilled;
            }

            set
            {
                m_IsKilled = value;
            }
        }

        //sets the owner pool
        public void SetOwner(GenericPool poolParent)
        {
            m_poolOwner = poolParent;
        }

        //The object will deactivate itself and return to the owning pool.
        //To be used instead of "Destroy"
        public void Kill()
        {
            if (!IsKilled)  //don't return on the pool if you are already there
            {
                IsKilled = true;
                m_poolOwner.ReturnToPool(this);
            }
        }
    }

}