/*
 *
 * Credits: https://github.com/UnityCommunity
 * 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
	
        #region Fields

        /// <summary>
        /// The instance.
        /// </summary>
        private static T _instance;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if ( _instance == null )
                {
                    _instance = FindObjectOfType<T> ();
                    if ( _instance == null )
                    {
                        GameObject obj = new GameObject ();
                        obj.name = typeof ( T ).Name;
                        _instance = obj.AddComponent<T> ();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Use this for initialization.
        /// </summary>
        protected virtual void Awake ()
        {
            if ( _instance == null )
            {
                _instance = this as T;
                DontDestroyOnLoad ( gameObject );
            }
            else
            {
                Destroy ( gameObject );
            }
        }

        #endregion
	
    }
}