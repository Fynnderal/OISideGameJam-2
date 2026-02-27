using Unity.VisualScripting;
using UnityEngine;
using KBCore.Refs;
using Utilities;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// Controls the behavior of a moving platform. 
/// </summary>
public class MovingPlatform : ValidatedMonoBehaviour
{


    [SerializeField, Self] Rigidbody2D _rb;
    [SerializeField] AnimationCurve _speedCurve;
    [SerializeField] MoveDirection _moveDirection;
    [SerializeField] float _duration;
    [SerializeField] float _multiplier = 1f;
    [SerializeField] bool _touchReacts;
    



    private float _currentTime;
    private StopwatchTimer _timer;

    private float _currentPosition;
    private Vector2 _origin;

    private Rigidbody2D _player;
    private PlayerController _playerController;
    private float _previousPosition;

    private float _speed;


    public float Speed => _speed;
    public MoveDirection Direction => _moveDirection;
    public Rigidbody2D Player => _player;
    public PlayerController PlayerController => _playerController;

    private bool _blocked = false;

    private void Awake()
    {

        _player = null;

        _timer = new StopwatchTimer();


        _origin = _rb.position;
        if (_touchReacts)
        {
            _blocked = true;
        }
    }
    void Start()
    {
        _timer.Start();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (_player != null)
        {
            _blocked = false;
        }


        if (_blocked)
            return;

        _timer.Tick(Time.fixedDeltaTime);
        _currentTime = _timer.GetTime();

        // Reached the end of the movement
        if (_currentTime > _duration)
        {
            _currentTime = _duration;
            _timer.Reset();
            if (_touchReacts && Player == null)
            {
                _blocked = true;
                return;
            }
        }

        _previousPosition = _currentPosition;
        _currentPosition = _speedCurve.Evaluate(_currentTime) * _multiplier;
        
        _speed = (_currentPosition - _previousPosition) / Time.fixedDeltaTime;


        if (_moveDirection == MoveDirection.HORIZONTAL || _moveDirection == MoveDirection.BOTH)
        {
            _rb.MovePosition(_origin + Vector2.right * _currentPosition);
            if (_player != null)
            {
                // Add platform speed to player velocity so they move together
                _player.linearVelocityX += _speed;
            }

        }


        if (_moveDirection == MoveDirection.VERTICAL || _moveDirection == MoveDirection.BOTH)
        {
            _rb.MovePosition(_origin + Vector2.up * _currentPosition);
            if (_player != null)
            {
                _player.linearVelocityY += _speed;
            }


        }


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _player = collision.gameObject.GetComponent<Rigidbody2D>();
            _playerController = collision.gameObject.GetComponent<PlayerController>();  
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _player = null;
            _playerController = null;
        }
    }
}

