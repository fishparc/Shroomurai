using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UIElements;

public class BossAttackChecks : MonoBehaviour
{
    Rigidbody2D RB;
    public Animator Anim { get; private set; }
    GameObject player;
    Player playerScript;
    SpriteRenderer m_SpriteRenderer;
    public Transform FloatiingPoint;
    int currentHealth;
    [SerializeField] private int bossMaxHealth = 200;
    [SerializeField] private LayerMask _playerLayer;
    [Header("Slash")]
    [SerializeField] private Transform _slashPoint;
    [SerializeField] private float _slashRadius = 3f;
    [SerializeField] private int slashDamage = 10;
    [SerializeField] private float slashKnockBackStrength = 2f;
    [SerializeField] private Vector2 SlashKnockBackDirection = new Vector2(1, 0);
    [SerializeField] private Vector3 AttackStep; 
    [Header("LaidoAndCharge")]
    [SerializeField] private Transform _ChargePoint;
    [SerializeField] private Vector2 _ChargeCheckSize = new Vector2(1, 1);
    [SerializeField] private int laidodamage = 20;
    [SerializeField] private float LaidoAttackTime;
    [SerializeField] private float LaidoEndTime;
    [SerializeField] private float laidoSpeed;
    [SerializeField] private float laidoEndSpeed;
    [SerializeField] private float laidoKnockUpStrength = 2;
    [SerializeField] private Vector2 laidoKnockUpDirection = new Vector2(0, 1);
    [SerializeField] private Vector3 DashStep; 
    [Space(2)]
    [SerializeField] private Color warningColor =Color.red;
    [SerializeField] private Transform _warningAreaPoint;
    [SerializeField] private Vector2 _warningAreaSize;
    [SerializeField] private GameObject warningBox; 
    
    [Header("KnockUptoAir")]
    [SerializeField] private int knockToAirDamage = 10;
    [SerializeField] private float _knockToAirRadius = 1f;
    [SerializeField] private float knockToAirStrength = 4f;
    [SerializeField] private Vector2 knockToAirDirection = new Vector2(0, 1);
    [Header("AirProjectile")]
    public GameObject BulletPreFab;
    public Transform AimGun;
    [SerializeField] private int NumberOfProjectiles = 3;
    [Range(0, 360)]
    [SerializeField] private float SpreadAngle = 20;
    [SerializeField] private float force = 5f;//bullet force/speed

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        currentHealth = bossMaxHealth;

        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
        StartCoroutine(CooldownFlip());
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(_slashPoint.position, _slashRadius);
        Gizmos.DrawWireSphere(_slashPoint.position, _knockToAirRadius);
        Gizmos.DrawWireCube(_ChargePoint.position, _ChargeCheckSize);
        Gizmos.DrawWireCube(_warningAreaPoint.position,_warningAreaSize);
    }
    private void OnDrawGizmos() 
    {
        Gizmos.color=warningColor;
        Gizmos.DrawWireCube((Vector2)_warningAreaPoint.position,_warningAreaSize);
    }
    #region HP METHODS
    public void TakeDamage(int damage)
    {

        currentHealth -= damage;
        Hurt();
        if (currentHealth <= bossMaxHealth * 0.5)
        {
            Enraged();
        }
        if (currentHealth <= 0)
        {
            Debug.Log("bossdying!");
            //die
        }
    }
    public void Hurt()
    {
        m_SpriteRenderer.color = Color.red;
        CancelInvoke("Recovery"); // in case the method has already been invoked
        Invoke("Recovery", 0.2f);
    }
    void Recovery()
    {
        m_SpriteRenderer.color = Color.white;
    }
    public void Enraged()
    {
        slashDamage += 50;
        laidodamage += 50;

        //animaparameter
    }
    #endregion
    #region Ground Attacks

    public void BossSlash()
    {
        StartCoroutine(AttackStepLerp(0.3f));
        Collider2D[] hitThings = Physics2D.OverlapCircleAll(_slashPoint.position, _slashRadius, _playerLayer);
        foreach (Collider2D thing in hitThings)
        {
            if (thing.tag == "Player")
            {
                playerScript.TakeDamage(slashDamage);
                playerScript.TakeKnockBack(slashKnockBackStrength, SlashKnockBackDirection);
            }
        }
    }
    
    private IEnumerator AttackStepLerp(float time)
    {
        Vector3 startingPos = transform.position;
        Vector3 finalPos = transform.position + AttackStep*(IsRighter?-1:1);

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield break;
    }
    public void knockToAir()
    {
        Collider2D[] hitThings = Physics2D.OverlapCircleAll(_slashPoint.position, _knockToAirRadius, _playerLayer);
        foreach (Collider2D thing in hitThings)
        {
            if (thing.tag == "Player")
            {
                playerScript.TakeDamage(knockToAirDamage);
                playerScript.TakeKnockBack(knockToAirStrength, knockToAirDirection);
            }
        }
    }
    public void ToggleWarningBox()
    {
        if(warningBox.activeSelf==true)
        {
            warningBox.SetActive(false);
        }
        else
        {
            warningBox.SetActive(true);
        }

    }
    public void DoDashStep(float time)
    {
        StartCoroutine(DashStepLerp(time));
    }

    private IEnumerator DashStepLerp(float time)
    {
        Vector3 startingPos = transform.position;
        Vector3 finalPos = transform.position + DashStep*(IsRighter?-1:1);

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield break;
    }
    /*public void DoCharging(Vector2 dire)
    {
        StartCoroutine("StartCharging", dire);
    }
    private IEnumerator StartCharging(Vector2 dir)
    {
        float startTime = Time.time;

        RB.gravityScale = 0;

        //We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
        while (Time.time - startTime <= LaidoAttackTime)
        {
            RB.velocity = dir.normalized * laidoSpeed;
            //Pauses the loop until the next frame, creating something of a Update loop. 
            //This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
            yield return null;
        }
        startTime = Time.time;
        //Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
        RB.gravityScale = 1;
        RB.velocity = laidoEndSpeed * dir.normalized;
        while (Time.time - startTime <= LaidoEndTime)
        {
            yield return null;
        }
    }*/
    public void BossLaido()//do the dashSlashONhits
    {
        Collider2D[] hitThings = Physics2D.OverlapBoxAll(_ChargePoint.position, _ChargeCheckSize, _playerLayer);
        foreach (Collider2D thing in hitThings)
        {
            if (thing.tag == "Player")
            {
                playerScript.TakeDamage(laidodamage);
                Debug.Log("LaidoDamage");
                playerScript.TakeKnockBack(laidoKnockUpStrength, Vector2.up);
            }
        }
    }
    #endregion

    #region Facing Method
    [HideInInspector] public bool IsRighter;//flip

    private IEnumerator CooldownFlip()
    {
        while (true)
        {
            Vector3 scale = transform.localScale;
            if (player.transform.position.x > transform.position.x)
            {
                scale.x *= Mathf.Abs(scale.x) * (IsRighter ? -1 : 1);
                IsRighter = false;
            }
            else
            {
                scale.x *= Mathf.Abs(scale.x) * -1 * (IsRighter ? -1 : 1);
                IsRighter = true;
            }
            transform.localScale = scale;

            yield return new WaitForSeconds(0.3f);
        }
    }
    #endregion
    #region InAir Attacks
    public void DoFloating()
    {
        StartCoroutine(Floating());
    }
    private IEnumerator Floating()
    {
        RB.gravityScale = 0;
        while (transform.position != FloatiingPoint.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, FloatiingPoint.position, 10 * Time.deltaTime);
            yield return null;
        }
        Debug.Log("Floatinto!");
        yield return new WaitForSeconds(2f);
        Anim.SetTrigger("FloatingAtk");
        yield break;
    }
   
    public void DoFloatingAttack()
    {
        StartCoroutine(FloatingAttack());
    }
    public float rotSpeed;
    private IEnumerator FloatingAttack()
    {
        int AttackRound = 3;
        float angleStep = SpreadAngle / NumberOfProjectiles;
        float centeringOffset = (SpreadAngle / 2) - (angleStep / 2); //offsets every projectile so the spread is//centered on the mouse cursor


        while (AttackRound > 0)
        {
            float aimingAngle = AimGun.rotation.eulerAngles.z;
            for (int i = 0; i < NumberOfProjectiles; i++)
            {

                float currentBulletAngle = angleStep * i;

                Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, aimingAngle + currentBulletAngle - centeringOffset));
                GameObject bullet = Instantiate(BulletPreFab, transform.position, rotation);

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.AddForce(bullet.transform.right * force, ForceMode2D.Impulse);
            }
            yield return new WaitForSeconds(0.5f);
            AttackRound--;
        }

        AimGun.gameObject.SetActive(false);
        StopCoroutine(LockAndLoad());
        RB.gravityScale = 1;
        yield break;
    }

    public void GunAim()
    {
        Vector2 dir = player.transform.position - AimGun.position;
        float TargetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion TargetRotate = Quaternion.AngleAxis(TargetAngle, Vector3.forward);
        AimGun.rotation = Quaternion.Slerp(AimGun.rotation, TargetRotate, 0.6f);
        Vector3 AimGunscale = AimGun.localScale;
        if (player.transform.position.x < transform.position.x)
        {
            AimGunscale.x = -1;
        }
        else
        {
            AimGunscale.x = 1;
        }
        AimGun.localScale = AimGunscale;
    }
    public void DoSoftLock()
    {
        StartCoroutine(LockAndLoad());
    }
    private IEnumerator LockAndLoad()
    {
        AimGun.gameObject.SetActive(true);//off after Attacks
        while (true)
        {
            GunAim();
            yield return null;
        }
    }
    #endregion

}
