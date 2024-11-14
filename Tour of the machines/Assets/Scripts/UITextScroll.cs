using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITextScroll : MonoBehaviour
{
    [SerializeField] private float _speed = 1.5f;

    private float _time;

    private float _timeReset = 55f;

    private Vector3 pos;

    private void Start()
    {
        pos = transform.position;
    }

    private void Update()
    {
        _time = _time + Time.deltaTime;

        transform.position += _speed * Time.deltaTime * Vector3.down;

        if (_time >= _timeReset)
        {
            Debug.Log("End Text Scroll!!");
            StartPos();
        }
    }
    public void StartPos()
    {
        transform.position = pos;
        _time = 0;
    }
    
}


