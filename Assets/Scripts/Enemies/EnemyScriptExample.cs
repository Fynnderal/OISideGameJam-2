using KBCore.Refs;
using UnityEngine;

public class EnemyScriptExample : ValidatedMonoBehaviour, IEnemy
{
    [SerializeField, Self] private Animator _animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool TakeDamage(int damage, Suit suit, GameObject player)
    {
        Debug.Log($"Enemy took {damage} damage from a {suit} attack!");
        _animator.Play("DAMAGED", 0, 0);
        return true;
        
    }
}
