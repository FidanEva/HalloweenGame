using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Casper : MonoBehaviour
{
    [SerializeField] private Transform _home;
    private Rigidbody _rb;
    [SerializeField] private float _speed = 0.5f;
    [SerializeField] private GameObject _player;
    private PlayerController _playerController;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        transform.DOMoveY(1.4f,1).SetLoops(-1, LoopType.Yoyo);
        transform.LookAt(_home);

        _playerController = _player.GetComponent<PlayerController>();
    }


    void Update()
    {

    }

    private void FixedUpdate()
    {
        _rb.velocity = transform.forward * _speed;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("House"))
        {
            _playerController.Die();
        }
    }    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("House"))
        {
            _playerController.Die();
        }
    }
}
