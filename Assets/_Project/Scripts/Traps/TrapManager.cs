using System.Collections.Generic;
using UnityEngine;
using Utilities;
using static UnityEngine.Rendering.DebugUI.Table;

public class TrapManager : MonoBehaviour
{
    [SerializeField] GameObject _fireTrap;

    [SerializeField] protected Transform _gridOrigin;
    [SerializeField] protected int cols;
    [SerializeField] protected int rows;

    int[,] _matrix;

    CountdownTimer _testTimer = new CountdownTimer(5f);
    Queue<GameObject> _traps = new Queue<GameObject>();

    private void Start()
    {
        _testTimer.Start();
        _matrix = new int[rows, cols];

    }
    private void Update()
    {
        if (!_testTimer.isRunning)
        {
            //if (_traps.Count > 0)
            //{   
            //    _matrix[_traps.Peek().GetComponent<LineTrapsBase>().Row, _traps.Peek().GetComponent<LineTrapsBase>().Column] = 0;
            //    Destroy(_traps.Dequeue());
               
            //}

            _traps.Enqueue(InstantiateTrap(_fireTrap));
            _testTimer.Start();

        }


        _testTimer.Tick(Time.deltaTime);
    }

    GameObject InstantiateTrap(GameObject trap)
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
}
