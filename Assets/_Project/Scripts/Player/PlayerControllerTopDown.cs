using UnityEngine;
using KBCore.Refs;
using Utilities;


public class PlayerControllerTopDown : MonoBehaviour
{
    #region refs
    [SerializeField, Self] Rigidbody _rb;
    [SerializeField, Self] Collider2D _collider;

    #endregion



    #region privates
    StateMachine _stateMachine;

    #endregion


    private void Awake()
    {


        _stateMachine = new StateMachine();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
