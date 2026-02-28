using System.Collections.Generic;
using UnityEngine;

public class LineTrapsBase : MonoBehaviour
{
    [SerializeField] protected float _damage = 20f;
    

    public int Row { get; set; }
    public int Column { get; set; }

    private void Start()
    {



    }
}
