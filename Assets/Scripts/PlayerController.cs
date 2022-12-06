using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    private float _horizontal, _vertical;
    private Vector3 _direction;
    [SerializeField] private float _speed = 0.3f, _currentVelocity, _turnTime = 0.2f;
    private Rigidbody _rb;
    [SerializeField] private FloatingJoystick _joystick;

    [Header("Crush Pumpkin")]
    [SerializeField] private List<ParticleSystem> _parts;
    [SerializeField] private List<GameObject> _pumpkins;
    
    [SerializeField] private Dictionary<GameObject, ParticleSystem> _pumpsAndParts;
      
    [Header("Detect & Collect")]
    [SerializeField] public Transform _holdTransform, _detectTransform;
    private float _detectionRange = 0.8f;
    [SerializeField] private LayerMask _collectedLayer;
    private int _itemCount = 0;
    private Collider[] _colliders;
    //[SerializeField] private float _distance = 1;
    //public List<Collider> Boxes;

    [Header("Drop")]

    float NextDropTime;
    [SerializeField] float DropRate = 3;
    [SerializeField] float DropSecond = 1;
    [SerializeField] List<GameObject> CollectedItems;
    [SerializeField] Transform DropArea;
    int dropCount = 0;
    //[SerializeField] float DropDistanceBetween = 1f;
    [SerializeField] private List<GameObject> _bakedItems;
    [SerializeField] private List<GameObject> _spawnedObjects;
    TimeControl _timeControl;
    [SerializeField] private GameObject _timeController;
    [SerializeField] private List<GameObject> _prefabs;
    [SerializeField] private GameObject _casper;
    private int _randomX;
    private int _randomY;
    private int _randomZ;
    private float _incremantalCount = 2;
    private HashSet<int> _xList;
    private HashSet<int> _zList;

    public List<Light> _homeLights;
    private float timeElapsed;

    [SerializeField] private List<GameObject> _potionParts;

    private Animator _anim;
    private CinemachineVirtualCamera _cinemachineVirtualCamera;

    [SerializeField] private GameObject _hat;
    [SerializeField] private GameObject _potion;
    [SerializeField] private GameObject _sebet;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _timeControl = _timeController.GetComponent<TimeControl>();
        _anim = GetComponent<Animator>();
        _cinemachineVirtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

        ////Instantiate etmeden sehnede olan pumkinleri liste yigmaq ve particllari sondurmek ucun
        //foreach (var p in _pumpkins)
        //{
        //    ParticleSystem part = p.GetComponentInChildren<ParticleSystem>();
        //    _parts.Add(part);
        //}
        //foreach (var i in _parts)
        //{
        //    i.Stop();
        //}

        //Dictionary not working
        //GameObject go = GameObject.FindWithTag("Pumpkin");
        //Debug.Log(go.name);
        //_pumpsAndParts.Add(go, null);
        //foreach (var item in _pumpsAndParts)
        //{
        //    Debug.Log(item);
        //    Debug.Log(item.Value);
        //}

    }
    void Update()
    {
        _horizontal = _joystick.Horizontal;
        _vertical = _joystick.Vertical;
        _direction = new Vector3(_horizontal, 0, _vertical);

        if (_direction.magnitude > 0.1f)
        {
            float _targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
            float _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _currentVelocity, _turnTime);
            transform.rotation = Quaternion.Euler(0, _angle, 0);
            _rb.MovePosition(transform.position + (_direction * _speed * Time.fixedDeltaTime));
        }
        _anim.SetFloat("speed", _direction.magnitude);

        if (_timeControl.currentTime.Hour == 7 && _timeControl.currentTime.Minute == 0)
        {
            //Debug.Log("iff");
            foreach (var item in _prefabs)
            {
                //Debug.Log("foreach prefabs");
                for (int i = 0; i < _incremantalCount; i++)
                {
                    //Debug.Log("for");

                   _randomX = Random.Range(-10, 2);
                   _randomZ = Random.Range(-10, 2);
        //            //_xList.Add(_randomX);

        //            //while (_xList.Contains(_randomX))
        //            //{
        //            //    continue;
        //            //}
        //            //Debug.Log(_randomX);

        //            //_randomZ = Random.Range(-10, 2);
        //            //_zList.Add(_randomZ);

        //            //while (_zList.Contains(_randomZ))
        //            //{
        //            //    //_randomZ = Random.Range(-10, 2);
        //            //    continue;
        //            //}
        //            //Debug.Log(_randomZ);
        //            ParticleSystem part = item.GetComponentInChildren<ParticleSystem>();
        //            //Debug.Log(part.name);
        //            part.Stop();
                   Instantiate(item, new Vector3(-7 + _randomX, -0.9f, -11 + _randomZ), Quaternion.identity);
                   _spawnedObjects.Add(item);

                }
           }
            _incremantalCount += 0.5f;
            //Debug.Log(_incremantalCount);

           foreach (var item in _potionParts)
           {
               item.SetActive(false);
           }
        }

        _colliders = Physics.OverlapSphere(_detectTransform.position, _detectionRange, _collectedLayer);
        foreach (var hit in _colliders)
        {
            if (hit.CompareTag("Collectable"))
            {
                hit.tag = "Collected";
                //Debug.Log(hit.name);
                hit.transform.parent = _holdTransform;
                var seq = DOTween.Sequence();
                hit.transform.localPosition = new Vector3(0, 0, 0);
                seq.Append(hit.transform.DOLocalJump(_holdTransform.localPosition, 2, 1, 0.3f))
                .Insert(0, hit.transform.DOScale(15, 0.1f))
                .Insert(0.1f, hit.transform.DOScale(5, 0.2f));
                //hit.transform.localPosition = new Vector3(0, 0, 0);
                CollectedItems.Add(hit.gameObject);

        //        //hit.gameObject.AddComponent<Rigidbody>();
                _itemCount++;
            }
        }


        if ((_timeControl.currentTime.Hour == 7))
        {
            foreach (var l in _homeLights)
            {
                l.intensity = Mathf.Lerp(5, 0, timeElapsed);
            }
        }
        if ((_timeControl.currentTime.Hour == 20))
        {
            foreach (var l in _homeLights)
            {
                l.intensity = Mathf.Lerp(0, 5, timeElapsed);
            }
            // if (CollectedItems.Count != _spawnedObjects.Count)
            // {
            //     Die();
            // }
        }
        timeElapsed += Time.deltaTime;

        if (_timeControl.currentTime.Hour == 20 && _timeControl.currentTime.Minute == 0)
        {
            for (int i = 0; i < _incremantalCount; i++)
            {
                _randomX = Random.Range(-8, 8);
                _randomY = Random.Range(-8, 8);
                _randomZ = Random.Range(-8, 8);
                Instantiate(_casper, new Vector3(-34 + _randomX, 1, -2.7f + _randomZ), Quaternion.identity);
            }
            _incremantalCount += 0.5f;
        }

    }

    public void Die()
    {
        Debug.Log("Game Over");

        Invoke("Restart", 3);
    }
    void Restart()
   {
       SceneManager.LoadScene(0);
        
   }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pumpkin") || collision.gameObject.CompareTag("Mushroom") || collision.gameObject.CompareTag("Yonca"))
        {
            Debug.Log(collision.gameObject.name);

            ParticleSystem p = collision.transform.parent.GetComponentInChildren<ParticleSystem>();

            Debug.Log(p.name);
            p.Play();
            //collision.gameObject.SetActive(false);
            Destroy(collision.transform.parent.gameObject);
        }

        // if (collision.gameObject.CompareTag("Casper") && _potion.transform.parent == this.gameObject)
        // {
        //     Debug.Log(collision.gameObject.name);
        //     Destroy(collision.gameObject);
        // }
    }


    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("FinishZone"))
        {

            if(_timeControl.currentTime.Hour >= 20 || _timeControl.currentTime.Hour <= 7)
            {
                if (_direction.magnitude < 0.01f)
                {
                    transform.position =  new Vector3(2.11999989f,-1.04999971f,-3.88400006f);
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    _hat.transform.parent = this.transform;
                    _hat.transform.position = new Vector3(0,0,0);
                    _hat.transform.localPosition =  new Vector3(-1.477f,-3.6329999f,-9.3739996f);
                    _sebet.transform.parent = null;
                    //_sebet.transform.position = new Vector3(0,0,0);
                    _sebet.transform.position =  new Vector3(4.1329999f,-0.879000008f,-3.97000003f);  


            if (Time.time >= NextDropTime)
            {
                if (CollectedItems.Count > 0)
                {
                    GameObject go = CollectedItems[CollectedItems.Count - 1];
                    Debug.Log(go);

                    go.transform.parent = null;
                    var Seq = DOTween.Sequence();
                    Seq.Append(go.transform.DOJump(DropArea.position + new Vector3(0, 1, 0), 2, 1, 0.3f))
                       .Join(go.transform.DOScale(15, 0.1f))
                       .Insert(0.1f, go.transform.DOScale(0, 0.2f));
                    dropCount++;
                    _itemCount--;
                    _bakedItems.Add(go);
                    CollectedItems.Remove(go);
                    NextDropTime = Time.time + DropSecond / DropRate;

                }
                else
                {
                    Debug.Log("else");
                                    _potion.transform.parent = this.transform;
                _potion.transform.position = new Vector3(0,0,0);
                _potion.transform.localPosition =  new Vector3(0.199000001f,0.795000017f,0.528999984f); 
                }
                Debug.Log("Test Drop");

            }
                            }
            foreach (var item in _potionParts)
            {
                item.SetActive(true);
            }
            }
         }

         if (other.CompareTag("Casper" )&& _potion.transform.parent != null)
         {
            Debug.Log(other.gameObject.name);
            Destroy(other.gameObject);
         }

        if (other.CompareTag("Casper" )&& _potion.transform.parent == null)
        {
            Debug.Log("No potion");
            Die();
        }

    }

}
