using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLight : MonoBehaviour
{
    [SerializeField]
    private Light _light;

    private float timeElapsed;

    [SerializeField] GameObject timeControl;
    TimeControl _timeControl;

    void Start()
    {
        _timeControl = timeControl.GetComponent<TimeControl>();
    }

    void Update()
    {

        if ((_timeControl.currentTime.Hour == 7))
        {
            //_light.enabled = false;
            _light.intensity = Mathf.Lerp(10, 0, timeElapsed);
        }
        timeElapsed += Time.deltaTime;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player") && ((_timeControl.currentTime.Hour < 20) && (_timeControl.currentTime.Hour > 15)))
        {
            _light.intensity = Mathf.Lerp(0, 5, 1);
        }
    }
}
