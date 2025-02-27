using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class BossDemon_Animation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DemonKnightBoss DemonBoss;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform player; // Reference to the player
    [SerializeField] private BossDemon_Rotation movementController;
    [SerializeField] private BossSpawning bossSpawning;
    [SerializeField] public RangeSensor rangeSensor;
    [SerializeField] public MeleeSensor meleeSensor;
    [SerializeField] private DissolvingControllerTut DissolvingController;

    [Header("Malee")]
    [SerializeField] private bool isAttacking; // Reference to the flamethr
    [SerializeField] private List<BoxCollider> attackHitboxes = new List<BoxCollider>(); 

    [Header("MaleeVFX")]
    [SerializeField] private List<ParticleSystem> slashVFX = new List<ParticleSystem>(); 
    [SerializeField] private List<GameObject> slashVFX_Position = new List<GameObject>();
    
    [Header("Dashing")]
    [SerializeField] private bool isDashing = false;
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashSpeed;

    [Header("Dashing02")]
    [SerializeField] private GameObject Dash02VFX; // Assign your prefab in the inspector
    [SerializeField] private Transform Dash02spawnPosition;
    //[SerializeField] private float dash02Distance; // Total distance to dash
    [SerializeField] private float dash02Speed;

    [Header("Bullet")]
    [SerializeField] private int numberOfBullets;
    [SerializeField] private float bulletDelay;
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] private Transform firePoint;
    private Vector2 uiOffset;

    [Header("Laser")]
    [SerializeField] private List<FlamethrowerHitbox> laserEffectHitboxes; 
    [SerializeField] private List<VisualEffect> laserEffects;
    private bool isFiringLaser = false;

    [Header("Indicator")]
    [SerializeField] AttackIndicatorController attackIndicatorController;
    [SerializeField] BombIndicator BombIndicatorController;
    [SerializeField] SphereCollider OffmapHitBox;

    [Header("OffMapCast")]
    public GameObject OutMapCastEffect;


    private void Update()
    {
        if ((isFiringLaser == true) && player.transform.position != null)
        {
            movementController.RequestLookAtplayer_laser();
        }else{
            return;
        }
        
    }

    void StartShoot(){
        if(DemonBoss.GetisEnrage() == true){
            StartCoroutine(ShootWithDelay(numberOfBullets+1, bulletDelay));
        }else{
            StartCoroutine(ShootWithDelay(numberOfBullets, bulletDelay));
        }
    }

    IEnumerator ShootWithDelay(int numberOfBullets, float bulletDelay)
    {
        // Prevent starting multiple shooting coroutines
        for (int i = 0; i < numberOfBullets; i++)
        {
                ShootBullet(); // Fire a bullet
                yield return new WaitForSeconds(bulletDelay); // Wait before firing the next
        }
    }

    private void ShootBullet()
    {
        GameObject projectile = Instantiate(BulletPrefab, firePoint.position, firePoint.rotation);
        projectile.GetComponent<SkullBomb>().UpdateTarget(player.transform, (Vector3)uiOffset);
    }

    public void PerformSummonMinions()
    {
        Debug.Log("Dragon summons minions!");
        animator.SetTrigger("SummonMinions");
    }

    public void PerformAttack01()
    {
        Debug.Log("PerformAttack01");
        animator.SetTrigger("Attack01");
    }

    public void PerformAttack02()
    {
        Debug.Log("PerformAttack02");
        animator.SetTrigger("Attack02");
    }

    public void PerformAttack03()
    {
        Debug.Log("PerformAttack03");
        animator.SetTrigger("Attack03");
    }

    public void PerformAttack04()
    {
        Debug.Log("PerformAttack04");
        animator.SetTrigger("Attack04");
    }

    public void PerformAttack05()
    {
        Debug.Log("PerformAttack05");
        animator.SetTrigger("Attack05");
    }

    public void PerformCast01()
    {
        Debug.Log("PerformCast01");
        animator.SetTrigger("Cast01");
    }

    public void PerformCast02()
    {
        Debug.Log("PerformCast02");
        animator.SetTrigger("Cast02");
    }

    public void PerformCast03()
    {
        Debug.Log("PerformCast03");
        animator.SetTrigger("Cast03");
    }

    public void PerformCast04()
    {
        Debug.Log("PerformCast04");
        animator.SetTrigger("Cast04");
    }

    public void PerformCast05()
    {
        Debug.Log("PerformCast05");
        animator.SetTrigger("Cast05");
    }

    public void PerformCast06()
    {
        Debug.Log("PerformCast06");
        animator.SetTrigger("Cast06");
    }

    public void LockMovement()
    {
        //Debug.Log("Movement locked.");
        if (movementController) movementController.LockMovement(); // Call LockMovement from BossRotationWithAnimation
    }

    public void UnlockMovement()
    {
        //Debug.Log("Movement unlocked.");
        animator.SetTrigger("UnlockMovement");
        if (movementController) movementController.UnlockMovement(); // Call UnlockMovement from BossRotationWithAnimation
    }

    public void StartDashing(){
        //Debug.Log("StartDashing");
        movementController.RequestInsideLookAtPlayer();
        animator.SetTrigger("Dash");
        Instantiate(Dash02VFX, Dash02spawnPosition.transform.position, Dash02spawnPosition.transform.rotation);
        StartCoroutine(DashForward02());
    }

    public void StartLaser()
    {
        if(DemonBoss.GetisEnrage() == true){
            foreach (var laser in laserEffects){
                laser.Play();
            }
            foreach (var hitbox in laserEffectHitboxes)
            {
                hitbox.ActivateHitbox();
            }
        }
        else{

            if (laserEffects.Count > 0)
            {
                laserEffects[0].Play();
            }
            if (laserEffectHitboxes.Count > 0)
            {
                laserEffectHitboxes[0].ActivateHitbox();
            }
        }

        isFiringLaser = true;
        animator.SetBool("IsLaser", true);
    }

    public void StopLaser()
    {
        foreach (var hitbox in laserEffectHitboxes)
        {
            hitbox.DeactivateHitbox();
        }
        
        foreach (var laser in laserEffects)
        {
            laser.Stop();
        }
        
        isFiringLaser = false;
        animator.SetBool("IsLaser", false);
    }

    public void PlayOutMapCast(){
        Instantiate(OutMapCastEffect, this.transform.position, Quaternion.identity);
    }

  private IEnumerator DashForward()
    {
        isDashing = true;

        Vector3 dashDirection = transform.forward;
        float reducedDashDistance = dashDistance;
        Vector3 potentialTargetPosition = transform.position + dashDirection * reducedDashDistance;

        // Project the target position onto the NavMesh
        if (
            NavMesh.SamplePosition(
                potentialTargetPosition,
                out NavMeshHit hit,
                reducedDashDistance,
                NavMesh.AllAreas
            )
        )
        {
            Vector3 targetPosition = hit.position; // Use the position on the NavMesh
            float dashTime = Vector3.Distance(transform.position, targetPosition) / dashSpeed; // Adjust dash time
            float startTime = Time.time;

            // While the enemy has not reached the target position
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                float journeyProgress = (Time.time - startTime) / dashTime;
                transform.position = Vector3.Lerp(
                    transform.position,
                    targetPosition,
                    journeyProgress
                );
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("Dash target position is not on the NavMesh. Cancelling dash.");
        }
        isDashing = false;
    }

    void EnableAttack(string Number)
    {
        if (!isDashing)
        {
            StartCoroutine(DashForward());
        }

        int x = 0;
        if (Int32.TryParse(Number, out x) && x >= 0 && x < attackHitboxes.Count) // Use .Count instead of .Length
        {
            attackHitboxes[x].enabled = true;

            // Activate Slash VFX
            if (slashVFX != null)
            {
                //slashVFX[x].Play();
                Instantiate(slashVFX[x], slashVFX_Position[x].transform.position, slashVFX_Position[x].transform.rotation);
            }
        }
    }

    private IEnumerator DashForward02()
    {
    isDashing = true;

    Vector3 dashDirection = transform.forward;

    // Calculate distance to player, clamping it to a max of 8
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
    float temp = Mathf.Min(distanceToPlayer-2, 8f);
    float reducedDashDistance = Mathf.Max(temp, 0f);

    Vector3 potentialTargetPosition = transform.position + dashDirection * reducedDashDistance;

    // Project the target position onto the NavMesh
    if (
        NavMesh.SamplePosition(
            potentialTargetPosition,
            out NavMeshHit hit,
            reducedDashDistance,
            NavMesh.AllAreas
        )
    )
    {
        Vector3 targetPosition = hit.position; // Use the position on the NavMesh
        float dashTime = Vector3.Distance(transform.position, targetPosition) / dash02Speed; // Adjust dash time
        float startTime = Time.time;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            float journeyProgress = (Time.time - startTime) / dashTime;
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                journeyProgress
            );
            yield return null;
        }
    }
}



    void DisableAttack(String Number)
    {
        //HideIndicator();

        int x = 0;
        Int32.TryParse(Number, out x);
            
        //Debug.Log("DisableAttack");
        attackHitboxes[x].enabled = false;
        //if (AttackTimeFrame != 0)
        //{
        //    ShowIndicator(AttackTimeFrame);
        //}
        
    }

    void StartOffMapCast(int AttackTimeFrame){
        DissolvingController.StartDissolve();
        ShowBombIndicator(AttackTimeFrame * 4);
    }

    void EnableOffMapCast(){
        EnableOffMapHitBox();
        PlayOutMapCast();
    }

    void EndOffMapCast(){
        HideBombIndicator();
        DisableOffMapHitBox();
    }

    void EnableOffMapHitBox()
    {
        if(OffmapHitBox != null) OffmapHitBox.enabled = true;
    }

    void DisableOffMapHitBox()
    {
        if(OffmapHitBox != null) OffmapHitBox.enabled = false;
    }

    void ShowBombIndicator(int AttackTimeFrame){
        if (BombIndicatorController != null)
        {
            BombIndicatorController.ShowIndicator(AttackTimeFrame);
        }
    }

    void HideBombIndicator(){
        if (BombIndicatorController != null)
        {
            BombIndicatorController.HideIndicator();
        }
    }

    void StartSpawning(){
        if(bossSpawning != null){
            bossSpawning.SpawnEenemy();
        }
    }
    
    void ShowIndicator(int AttackTimeFrame)
    {
        if (attackIndicatorController != null)
        {
            //attackIndicatorCanvas.enabled = true;
            attackIndicatorController.ShowIndicator(AttackTimeFrame);
        }
    }

    void HideIndicator()
    {
        if (attackIndicatorController != null)
        {
            attackIndicatorController.HideIndicator();
        }
    }
    


    public void StartKnockBack(){
        if (player != null)
        {
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 forceDirection = (player.position - transform.position).normalized; // Direction away from dragon
                float knockbackForce = 10f; // Adjust force value as needed
                forceDirection.y = 1f; // Add slight upward force for impact
                playerRb.AddForce(forceDirection * knockbackForce, ForceMode.Impulse);
            }
        }
    }

    public void TransitionToPhase(String newPhase){
        if(newPhase == "Phase1_Enraged"){
            animator.SetBool("Phase01",false);
            animator.SetBool("Phase01_Enrage",true);
            animator.SetBool("Phase02",false);
            animator.SetBool("Phase02_Enrage",false);

        }else if(newPhase == "Phase2"){
            animator.SetBool("Phase01",false);
            animator.SetBool("Phase01_Enrage",false);
            animator.SetBool("Phase02",true);
            animator.SetBool("Phase02_Enrage",false);

        }else if(newPhase == "Phase2_Enraged"){
            animator.SetBool("Phase01",false);
            animator.SetBool("Phase01_Enrage",false);
            animator.SetBool("Phase02",false);
            animator.SetBool("Phase02_Enrage",true);

        }else{
            Debug.LogError("TransitionToPhase_Bug");
        }
    }

}
