using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HyperNinjaController : MonoBehaviour {

    [SerializeField]
    float _minForce = 2.0f;

    [SerializeField]
    float _maxSpeed = 300.0f;

    [SerializeField]
    float _dashDuration = 0.1f;

    [SerializeField]
    Vector2 _startPosition;

    //[SerializeField]
    //float _minMagnitudeToLaunch = 2.0f;

    const int GARNISH_BACKGROUND_LAYER_MASK = 8;

    struct LaunchVectorInfo {
        float _time;
        public float Time
        {
            get { return _time; }
        }

        Vector3 _vector;
        public Vector3 Vector
        {
            get { return _vector;  }
        }

        public LaunchVectorInfo(float time, Vector3 vector)
        {
            _time = time;
            _vector = vector;
        }
    }

	bool _launching;
    Vector3 _mouseLaunchPosition;
    List<LaunchVectorInfo> _launchVelocity = new List<LaunchVectorInfo>();

    void Awake() {
        _startPosition = transform.position;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            MouseDown();
        } else if (Input.GetMouseButtonUp(0)) {
            MouseUp();
        } else if (Input.GetMouseButton(0)) {
            MouseDrag();
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == "Hazard") {
            transform.position = _startPosition;
        }
    }

    void MouseDown()
    {
        if (!_launching) {
            _launching = true;
            _launchVelocity.Add(new LaunchVectorInfo(Time.time, Input.mousePosition));
        }
    }

    void MouseDrag() {
        if (_launching)
        {
            if (_launchVelocity.Count > 10) {
                _launchVelocity.RemoveAt(0);
            }

            _launchVelocity.Add(new LaunchVectorInfo(Time.time, Input.mousePosition));
        }
        
    }

    void MouseUp()
    {
        if (_launching)
        {
            Launch();
        }
    }

    void Launch()
    {
        float time = _launchVelocity[_launchVelocity.Count - 1].Time - _launchVelocity[0].Time;
        Vector3 vector = _launchVelocity[_launchVelocity.Count - 1].Vector - _launchVelocity[0].Vector;

        Vector3 velocity = (vector / time) * 0.01f;

        /*if (velocity.y <= 0)
        {
            CancelLaunch();
            return;
        }
        else if (velocity.y < _minForce)
        {
            velocity.y = _minForce;
        }*/

        //velocity =  Vector3.ClampMagnitude(velocity, _maxSpeed);
        velocity = velocity.normalized * _maxSpeed;

        GetComponent<Rigidbody2D> ().velocity = velocity;

        StartCoroutine("DoStop");
        
        //GetComponent<Rigidbody2D>().AddForce(velocity, ForceMode2D.Force);

        //GarnishManager.Register (_launch);

        //AudioController.Instance.PlayLaunchGarnishSound();

        _launching = false;
        _launchVelocity.Clear();
    }

    IEnumerator DoStop() {
        yield return new WaitForSeconds(_dashDuration);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    void CancelLaunch()
    {
        _launching = false;
        _launchVelocity.Clear();
    }
}
