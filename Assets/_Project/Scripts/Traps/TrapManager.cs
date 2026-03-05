using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;
using static UnityEngine.Rendering.DebugUI.Table;
public enum TrapType
{
    Fire,
    Spike,
}
public class TrapManager : MonoBehaviour
{
    [SerializeField] GameObject _fireTrap;
    [SerializeField] GameObject _spikes;

    [SerializeField] protected Transform _gridOrigin;
    [SerializeField] protected int cols;
    [SerializeField] protected int rows;

    [SerializeField] protected int maxTrapsAtOnce;

    [SerializeField] GameObject[] _enemies;

    int[,] _matrix;

    //Queue<GameObject> _traps = new Queue<GameObject>();

    private void Start()
    {
        _matrix = new int[rows, cols];
        //InstantiateTraps(TrapType.Fire);
        //InstantiateTrap(TrapType.Spike); 

    }
    private void Update()
    {
        
            //if (_traps.Count > 0)
            //{   
            //    _matrix[_traps.Peek().GetComponent<LineTrapsBase>().Row, _traps.Peek().GetComponent<LineTrapsBase>().Column] = 0;
            //    Destroy(_traps.Dequeue());
               
            //}

            //_traps.Enqueue(InstantiateTrap(_fireTrap));

    }

    public void InstantiateTraps(TrapType trap)
    {
        int iterations = Random.Range(1, maxTrapsAtOnce + 1);
        
        for (int i = 0; i < iterations; i++)
        {
            InstantiateTrap(trap);
        }
    }

    public GameObject InstantiateTrap(TrapType trap)
    {
        switch (trap)
        {
            case TrapType.Fire:
                return InstantiateTrap(_fireTrap);
            case TrapType.Spike:
                return InstantiateTrap(_spikes);
            default:
                return null;
        }
    }
    public GameObject InstantiateTrap(GameObject trap)
    {

        
        int col = Random.Range(0, cols);
        int row = Random.Range(0, rows);

        if (_matrix[row, col] == 1)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (_matrix[i, j] == 0)
                    {
                        row = i;
                        col = j;
                        break;
                    }
                }
            }
        }

        _matrix[row, col] = 1; 



        Vector2 position = new Vector2(_gridOrigin.position.x + col, _gridOrigin.position.y - row);


        GameObject newTrap = Instantiate(trap, position, Quaternion.identity);
        newTrap.GetComponent<LineTrapsBase>().Row = row;
        newTrap.GetComponent<LineTrapsBase>().Column = col;

        return newTrap;
    }


    private void InstantiateEnemy(GameObject enemy, Transform waveparent)
    {
        int col = Random.Range(3, cols - 3);
        int row = Random.Range(3, rows - 3);

        Vector2 position = new Vector2(_gridOrigin.position.x + col, _gridOrigin.position.y - row);


        Instantiate(enemy, position, Quaternion.identity, waveparent);
    }
    public void InstantiateEnemies(int waveNumber, Transform waveparent) 
    {
        int min_number = Mathf.CeilToInt(waveNumber * 0.3f);
        int max_number = waveNumber > 8 ? Mathf.CeilToInt(waveNumber * 0.5f) : 3;

        for (int i = 0; i < _enemies.Length; i++)
        {
            int numberToSpawn = Random.Range(min_number, max_number + 1);   

            for (int j = 0; j < numberToSpawn; j++)
            {
                InstantiateEnemy(_enemies[i], waveparent);
            }
        }
    }
}
