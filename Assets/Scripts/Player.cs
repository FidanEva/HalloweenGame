//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using DG.Tweening;
//using UnityEngine.SceneManagement;
//using Sirenix.OdinInspector;
//using Dreamteck;
//using Dreamteck.Splines;

//public class Player : SerializedMonoBehaviour
//{

//    bool isFinished = false;
//    [Header("Direction")]
//    private float _horizontal;

//    [Header("Detect & Collect")]
//    [SerializeField] public Transform _holdTransform, _detectTransform;
//    private float _detectionRange = 1;
//    [SerializeField] private LayerMask _collectedLayer;
//    private int _itemCount = 0;
//    private Collider[] _colliders;
//    [SerializeField] private float _distance = 1;

//    [Header("Components")]
//    private Animator anim;
//    private Rigidbody rb;

//    private bool _alive = true;

//    [Header("Drop")]

//    public Stack<Collider> Boxes;
//    float NextDropTime;
//    [SerializeField] float DropRate = 3;
//    [SerializeField] float DropSecond = 1;
//    [SerializeField] List<GameObject> CollectedItems;
//    [SerializeField] Transform DropArea;
//    int dropCount = 0;
//    [SerializeField] float DropDistanceBetween = 1f;

//    //[SerializeField] private GameObject obstaclePrefab;
//    //[SerializeField] private GameObject coinPrefab;
//    //[SerializeField] private Transform instantPos;
//    //[SerializeField] private GameObject treePrefab;
//    //[SerializeField] private GameObject leafPrefab;
//    //[SerializeField] private Transform instantTreeLeft, instantTreeRight;


//    GameObject spawnedObstacle;
//    SplineFollower _spline;

//    [SerializeField] private GameObject _backpack;

//    Coin _coin;

//    void Start()
//    {
//        isFinished = false;
//        anim = GetComponent<Animator>();
//        rb = GetComponent<Rigidbody>();
//        //SplineFollower follower = GetComponent<SplineFollower>();
//        //var lemons = GetComponent<Coin>();
//        //lemons.StartCoroutine(());
//        Boxes = new Stack<Collider>();

//        //StartCoroutine(ObstacleSpawner());
//        //StartCoroutine(TreeSpawner());
//        _spline = GetComponentInParent<SplineFollower>();

//        GameObject.FindObjectOfType<Coin>().MoveLemons();
        

//    }

//    private void OnDrawGizmos()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawSphere(_detectTransform.position, _detectionRange);
//    }
//    void Update()
//    {
        
//        if (isFinished)
//        {
//            _backpack.transform.parent = null;
//            _backpack.gameObject.AddComponent<Rigidbody>();
//        }
//        Debug.Log(Boxes.Count);

//        _horizontal = Input.GetAxis("Horizontal");
//        transform.localPosition += new Vector3(_horizontal * 5, 0, 0) * Time.deltaTime;

//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            anim.SetBool("Jumping", true);
//            rb.AddForce(new Vector3(0, 7, 0), ForceMode.Impulse);
//        }
//        if (Input.GetKeyUp(KeyCode.Space))
//        {
//            anim.SetBool("Jumping", false);
//        }

//        _colliders = Physics.OverlapSphere(_detectTransform.position, _detectionRange, _collectedLayer);
//        foreach (var hit in _colliders)
//        {
           
//            //lemons.StopCoroutine(MoveLemons());
//            //_coin.DOKill();
           
//            if (hit.CompareTag("Collectable") && _itemCount <= 15)
//            {
//                //Coin coin = hit.GetComponent<Coin>();
//                //hit.TryGetComponent<Coin>(out Coin coin);
//                hit.transform.DOKill();

//                hit.tag = "Collected";
//                //hit.gameObject.layer = 17;
//                hit.transform.parent = _holdTransform;
//                Boxes.Push(hit);
//                var seq = DOTween.Sequence();
//                seq.Append(hit.transform.DOLocalJump(new Vector3(0, (_itemCount * _distance), 0), 2, 1, 0.3f))
//                .Insert(0, hit.transform.DOScale(7, 0.1f))
//                //.Insert(0.1f, hit.transform.DOScale(5, 0.2f));
//                .Insert(0.1f, hit.transform.DOScale(0, 0.2f))
//                .Insert(0.1f, _backpack.transform.DOScaleX(_backpack.transform.localScale.x + _itemCount * 0.01f, 0.1f)).Join(_backpack.transform.DOScaleY(_backpack.transform.localScale.y + _itemCount * 0.005f, 0.1f)).Join(_backpack.transform.DOScaleZ(_backpack.transform.localScale.z + _itemCount * 0.01f, 0.1f));
//                seq.AppendCallback(() =>
//                {
//                    hit.transform.localRotation = Quaternion.Euler(0, 0, 0);
//                });
//                _itemCount++;
//            }
//        }

//        if (transform.localPosition.x < -2|| transform.localPosition.x > 2)
//        {
//            anim.SetBool("Falling", true);
//            _spline.enabled = false;
//            Die();
//        }

//    }
//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.CompareTag("Obstacle"))
//        {
//            _spline.enabled = false;
//            anim.SetBool("Falling", true);
//            Die();
//        }
//        if (collision.gameObject.CompareTag("StopCor"))
//        {
//            StopAllCoroutines();
//        }
//    }

//    public void Die()
//    {
//        _alive = false;
//        Invoke("Restart", 3);

//        isFinished = true;
//        //foreach (var box in Boxes)
//        //{
//        //    box.gameObject.AddComponent<Rigidbody>();
//        //}
//    }

//    void Restart()
//    {
//        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
//    }

//    private void OnTriggerStay(Collider other)
//    {

//        if (other.transform.CompareTag("FinishZone"))
//        {
//            isFinished = true;
            
//            _spline.enabled = false;
//            anim.SetBool("IdleMood", true);

//            if (Time.time >= NextDropTime)
//            {

//                if (Boxes.Count <= 0) return;
//                GameObject go = Boxes.Pop().gameObject;
//                go.transform.parent = null;
//                var Seq = DOTween.Sequence();
//                Seq.Append(go.transform.DOJump(DropArea.position + new Vector3(0, (dropCount * DropDistanceBetween), 0), 2, 1, 0.3f))
//                        .Join(go.transform.DOScale(7f, 0.1f))
//                        .Insert(0.1f, go.transform.DOScale(5, 0.2f))
//                        .AppendCallback(() => { go.transform.rotation = Quaternion.Euler(0, 0, 0); })
//                        .Insert(0.1f, _backpack.transform.DOScaleX(_backpack.transform.localScale.x - dropCount * 0.01f, 0.1f)).
//                        Join(_backpack.transform.DOScaleY(_backpack.transform.localScale.y - dropCount * 0.005f, 0.1f)).
//                        Join(_backpack.transform.DOScaleZ(_backpack.transform.localScale.z - dropCount * 0.01f, 0.1f));
//                //other.GetComponent<DropArea>().StackedDropItems.Push(go);
//                //other.GetComponent<DropArea>().StackedDropItems.Push(go);
//                dropCount++;
//                _itemCount--;
//                NextDropTime = Time.time + DropSecond / DropRate;

//                go.gameObject.AddComponent<Rigidbody>();


//                if (_itemCount == 0)
//                {
//                    _spline.enabled = true;

//                    anim.SetBool("Dancing", true);
//                }
//            }

//        }
//        //if (_itemCount == 0)
//        //{
//        //}
//    }
//    private void OnTriggerEnter(Collider other)
//    {
//        //StartCoroutine(other.GetComponent<DropArea>().SellDropedItems());
//    }
//    private void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag("FinishZone"))
//        {
//            anim.SetBool("IdleMood", false);
//            anim.SetBool("Dancing", true);
//            StartCoroutine(other.GetComponent<DropArea>().SellDropedItems());
//        }


//        //dropCount = 0;
//    }

//    //IEnumerator ObstacleSpawner()
//    //{
//    //    while (true)
//    //    {
//    //        yield return new WaitForSeconds(Random.Range(1, 2));
//    //        Instantiate(obstaclePrefab, new Vector3((instantPos.position.x + Random.Range(-2.5f, 2.5f)), instantPos.position.y + 1.2f, instantPos.position.z), Quaternion.Euler(instantPos.rotation.x + 90, instantPos.rotation.y, instantPos.rotation.z));
//    //        Instantiate(coinPrefab, new Vector3((instantPos.position.x + Random.Range(-4, 4)), instantPos.position.y, instantPos.position.z), Quaternion.Euler(0, 0, 0));

//    //        /* if (spawnedObstacle.transform.CompareTag("Collectable"))
//    //            {
//    //                Destroy(spawnedObstacle);
//    //            }*/

//    //    }
//    //}
//    //IEnumerator TreeSpawner()
//    //{
//    //    while (true)
//    //    {
//    //        yield return new WaitForSeconds(Random.Range(0.5f, 0.75f));
//    //        Instantiate(treePrefab, instantTreeLeft.position, Quaternion.Euler(0, 0, 0));
//    //        Instantiate(treePrefab, instantTreeRight.position, Quaternion.identity);
//    //        Instantiate(leafPrefab, instantTreeLeft.position, Quaternion.Euler(0, 0, 0));
//    //        Instantiate(leafPrefab, instantTreeRight.position, Quaternion.identity);
//    //    }
//    //}
//}
