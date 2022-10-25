using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{


    SplineFollower _splineFollower;
    Animator anim;
    float Horizontal;
    public float Speed;
    public float JumpForce;
    public bool _IsDead=false;
     Collider[] colliders;
     [SerializeField]private float DetectionRange;
     [SerializeField] LayerMask layer;
    [SerializeField]private Transform detectTransform;
    [SerializeField]private Transform holdTransform;
    [SerializeField]private Transform dropTransform;
    [SerializeField]private float itemCount;
    [SerializeField]private float ItemDistanceBetween;
    [SerializeField]private float dropCount;
    [SerializeField]private float dropDistanceBetween;
    Rigidbody rb;
    float NextDropTime;

    private bool _isGround= true;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        _splineFollower = GetComponentInParent<SplineFollower>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && _isGround)
        {
            anim.SetBool("Jump", true);
            rb.AddForce(Vector3.up * JumpForce,ForceMode.Impulse);
            _isGround=false;
        }
         if (Input.GetKeyUp(KeyCode.Q))
        {
            anim.SetBool("Jump", false);
     
        }
        Horizontal = Input.GetAxis("Horizontal");         
        transform.localPosition += new Vector3(Horizontal, 0, 0) * Time.deltaTime * Speed;




        colliders = Physics.OverlapSphere(detectTransform.position, DetectionRange, layer);
        foreach (var hit in colliders)
        {
            if (hit.CompareTag("Collectable"))
            {
                Debug.Log(hit.name);
                hit.tag = "Collected";
                hit.transform.parent = holdTransform;

                 var seq = DOTween.Sequence();

                 seq.Append(hit.transform.DOLocalJump(new Vector3(0, itemCount * ItemDistanceBetween), 2, 1, 0.2f))
                     .Join(hit.transform.DOScale(0.3f, 0.2f));
                 seq.AppendCallback(() =>
                 {
                     hit.transform.localRotation = Quaternion.Euler(0, 0, 0);
                 });
                 itemCount++;

                // CollectedItems.Add(hit.gameObject);

                /// Type3
                // Boxes.Push(hit);
                // var seq = DOTween.Sequence();
                // seq.Append(hit.transform.DOLocalJump(new Vector3(0, itemCount * ItemDistanceBetween), 2, 1, 0.3f))
                //    .Join(hit.transform.DOScale(1.25f, 0.1f))
                //    .Insert(0.1f, hit.transform.DOScale(0.3f, 0.2f));
                // seq.AppendCallback(() =>
                // {
                //     hit.transform.localRotation = Quaternion.Euler(0, 0, 0);
                // });
                // itemCount++;
            }
        }
    }

         public void OnTriggerStay(Collider other)
         {

            other.transform.parent = null;
            var Seq = DOTween.Sequence();
            Seq.Append(other.transform.DOJump(dropTransform.position + new Vector3(0,0,(dropCount * dropDistanceBetween )), 2, 1, 0.3f))
                    .Join(other.transform.DOScale(1.5f, 0.1f))
                    .Insert(0.1f, other.transform.DOScale(1, 0.2f))
                    .AppendCallback(() => { other.transform.rotation = Quaternion.Euler(0, 0, 0); });
            //other.GetComponent<DropArea>().StackedDropItems.Push(go);
            itemCount--;
         }


    public void OnTriggerEnter(Collider other) 
    {

        if (other.transform.CompareTag("Enemy"))
        {
            Debug.Log("we Dead");
            _splineFollower.followSpeed = 0f;
            anim.SetTrigger("Death");
            _IsDead = true; 
            Speed = 0f;
        }
        
        
    }

     private void OnCollisionEnter(Collision other) 
     {
         if(other.transform.CompareTag("Ground"))
        {
            _isGround=true;
            Transform child = transform.GetChild(0);
            child.localRotation = Quaternion.identity;
        }
    }

    public void Death()
    {
        if (_IsDead)
        {
            
        }
    }
}
