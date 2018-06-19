using UnityEngine;

namespace Test.Pickups
{
    //Each pickup belongs to a pool of similar objects 
    public abstract class Pickup : IPoolableObject
    {
        //Property used to see in which pickup slot has been spawned
        private int m_ID;
        public int ID { get { return m_ID; } set { m_ID = value; } }

        //Pickups need to respond to collision in order to be picked up
        protected virtual void OnTriggerEnter(Collider other)
        {
            this.Kill();
        }
    }
}