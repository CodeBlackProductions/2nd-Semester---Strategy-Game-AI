using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

[RequireComponent(typeof(SphereCollider))]
public class UnitController : MonoBehaviour
{
    #region Fields

    [Header("Combat Stats")]
    [SerializeField] private bool isRangedUnit;

    [SerializeField] private float attackRange;

    [SerializeField] [Range(1, 60)] private float attackSpeed;
    [SerializeField] private float attackDamage;
    [SerializeField] private float armor;

    [Header("General Stats")]
    [SerializeField] private float health;

    [SerializeField] private float visionRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private DecalProjector selection;

    [SerializeField] private GameObject bloodEfffect;
    [SerializeField] private GameObject dustEffect;
    [SerializeField] private AudioSource audioSource;

    public float Health
    {
        get => health;
        set
        {
            health = value;
            OnReceiveDamage();
        }
    }

    public float Armor { get => armor; set => armor = value; }
    public float AttackDamage { get => attackDamage; set => attackDamage = value; }
    public float MoveSpeed { get => moveSpeed; }
    public FormationController ParentFormation { get; set; }
    public Transform TransformVar { get; private set; }

    private BT_Unit parentAI;
    private Animator animator;
    private NavMeshAgent navAgent;
    private UnitController hitEnemy;

    private List<Transform> enemies = new List<Transform>();

    private float attackCooldown = 0;

    private float detectionTimer = 0;

    private bool inCombat = false;

    public bool InCombat
    {
        get
        {
            return inCombat;
        }
        set
        {
            inCombat = value;
            if (value == true)
            {
                ParentFormation.ParentGroup.UnitsInCombat.Add(this);
                EventHandler.eventHandler.RTSUnitInCombat?.Invoke(this, true);
            }
            else
            {
                ParentFormation.ParentGroup.UnitsInCombat.Remove(this);
                EventHandler.eventHandler.RTSUnitInCombat?.Invoke(this, false);
            }
        }
    }

    public Animator Animator { get => animator; }

    public BT_Unit ParentAI { get => parentAI; }

    public int EnemyCount { get => enemies.Count; }

    private VisualEffect dustParticleSystem;

    #endregion Fields

    public void Awake()
    {
        EventHandler.eventHandler.OnSelection += SwitchSelection;
        EventHandler.eventHandler.RTSUnitInCombat += OnAllyCombatStatus;

        TransformVar = transform;
        animator = TransformVar.GetComponentInChildren<Animator>();
        parentAI = TransformVar.GetComponent<BT_Unit>();
        parentAI.Blackboard.Variables.Add((int)BT_Unit.UnitBlackBoardKey.VisionRange, visionRange * visionRange);
        parentAI.Blackboard.Variables.Add((int)BT_Unit.UnitBlackBoardKey.AttackRange, attackRange * attackRange);
        parentAI.Blackboard.Variables.Add((int)BT_Unit.UnitBlackBoardKey.MoveSpeed, moveSpeed);
        parentAI.Blackboard.Variables.Add((int)BT_Unit.UnitBlackBoardKey.RotationSpeed, rotationSpeed);
        parentAI.Blackboard.Variables.Add((int)BT_Unit.UnitBlackBoardKey.IsRanged, isRangedUnit);
        parentAI.Blackboard.Variables.Add((int)BT_Unit.UnitBlackBoardKey.AlliesInCombat, new List<BT_Unit>());

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.acceleration += UnityEngine.Random.Range(-3, 3);

        animator.SetBool("Ranged", isRangedUnit);
    }

    public void Start()
    {
        //Increase base cohesion by normal max cohesion if ranged
        if (isRangedUnit)
        {
            parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.UnitCohesion] *= 4;
        }
    }

    public void Update()
    {
        CheckForAllies();

        parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.EnemiesInSight] = EnemyCount;

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (detectionTimer > 0)
        {
            detectionTimer -= Time.deltaTime;
        }

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i] == null || enemies[i].gameObject.activeSelf == false)
            {
                enemies.RemoveAt(i);
            }
        }

        if ((parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.TargetEnemy] == null || parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.TargetEnemy].gameObject.activeSelf == false) && enemies.Count > 0)
        {
            parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.TargetEnemy] = FindClosestEnemy();
        }

        float temp = navAgent.velocity.magnitude;
        temp = Mathf.Clamp(temp, 0, 1);
        animator.SetFloat("RunBlend", temp);

        if (temp == 0 && dustParticleSystem != null)
        {
            dustParticleSystem.Stop();
        }
    }

    private void OnDisable()
    {
        EventHandler.eventHandler.OnSelection -= SwitchSelection;
        EventHandler.eventHandler.RTSUnitInCombat -= OnAllyCombatStatus;
    }

    public void Attack(UnitController enemy)
    {
        if (attackCooldown <= 0 && CheckFacing(enemy))
        {
            hitEnemy = enemy;
            animator.SetTrigger("AttackTrigger");
            int randNr = UnityEngine.Random.Range(0, 3);
            switch (randNr)
            {
                case 0:
                    SoundManager.soundManager.PlayOneShotLocal(SFX.Attack1, audioSource);
                    break;

                case 1:
                    SoundManager.soundManager.PlayOneShotLocal(SFX.Attack2, audioSource);
                    break;

                case 2:
                    SoundManager.soundManager.PlayOneShotLocal(SFX.Attack3, audioSource);
                    break;
            }
            WaitForAnimation("IsInAnim", true);
            attackCooldown = 60 / attackSpeed;
            WaitForAnimation("IsInAnim", false);
            parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.IsInAnim] = false;
        }
    }

    #region Animation Callbacks

    public void OnStep()
    {
        if (WeatherHandler.weatherHandler.LevelIndex == 1)
        {
            if (dustParticleSystem == null)
            {
                dustParticleSystem = Instantiate<GameObject>(dustEffect, new Vector3(transform.position.x, transform.position.y - transform.position.y * 0.5f, transform.position.z), Quaternion.identity, transform).GetComponent<VisualEffect>();
            }
            else
            {
                dustParticleSystem.Play();
            }
        }
    }

    public void OnDealDamage()
    {
        float damage = attackDamage - hitEnemy.Armor;
        damage += damage * CheckGroupFacing(hitEnemy);
        if (damage <= 2)
        {
            damage = 2;
        }
        if (isRangedUnit)
        {
            GameObject temp = GameObject.Instantiate(missilePrefab, transform.position, transform.rotation);
            MissileController mController = temp.GetComponent<MissileController>();
            mController.ParentController = this;
            mController.Target = hitEnemy.transform.position;
            mController.Damage = damage;
        }
        else
        {
            hitEnemy.Health -= damage;
            Instantiate<GameObject>(bloodEfffect, hitEnemy.transform.position, Quaternion.identity);
        }
    }

    private void OnReceiveDamage()
    {
        animator.SetTrigger("DamageTrigger");
        WaitForAnimation("IsInAnim", true);
        WaitForAnimation("IsInAnim", false);
        if (health <= 0)
        {
            animator.SetTrigger("IsDead");
        }
    }

    public void OnDeath()
    {
        InCombat = false;
        ParentFormation.ParentGroup.UpdateGroupSpeed();
        gameObject.SetActive(false);
    }

    #endregion Animation Callbacks

    #region Internal Methods

    #region Unit Vision & Enemy Detection

    private void OnTriggerEnter(Collider other)
    {
        if (parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.TargetEnemy] == null || parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.TargetEnemy].gameObject.activeSelf == false)
        {
            if (other.TryGetComponent<BT_Unit>(out BT_Unit enemy) && !CheckIfAlly(enemy))
            {
                parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.TargetEnemy] = enemy;
                enemies.Add(other.transform);
            }
        }
        else
        {
            if (other.TryGetComponent<BT_Unit>(out BT_Unit enemy) && !CheckIfAlly(enemy))
            {
                enemies.Add(other.transform);
                if (detectionTimer <= 0)
                {
                    detectionTimer = 1;
                    parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.TargetEnemy] = FindClosestEnemy();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<BT_Unit>(out BT_Unit enemy))
        {
            if (!CheckIfAlly(enemy))
            {
                int tempIndex;
                tempIndex = enemies.IndexOf(other.transform);

                enemies[tempIndex] = enemies[enemies.Count - 1];

                enemies.RemoveAt(enemies.Count - 1);

                if (enemies.Count > 0 && detectionTimer <= 0)
                {
                    detectionTimer = 1;
                    parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.TargetEnemy] = FindClosestEnemy();
                }
            }
        }
    }

    private BT_Unit FindClosestEnemy()
    {
        if (enemies.Count > 0)
        {
            Transform temp = enemies[0];
            for (int i = 0; i < enemies.Count; i++)
            {
                if (CalculateDistance(TransformVar, temp) > CalculateDistance(TransformVar, enemies[i]))
                {
                    if (enemies[i].gameObject.activeSelf)
                    {
                        temp = enemies[i];
                    }
                }
            }

            BT_Unit tempBT;
            if (temp != null && temp.gameObject.activeSelf)
            {
                tempBT = temp.GetComponent<BT_Unit>();
            }
            else
            {
                tempBT = null;
            }

            return tempBT;
        }

        return null;
    }

    private void CheckForAllies()
    {
        if (ParentFormation.ParentGroup.Stance != (int)UnitGroupController.GroupStance.Aggressive && Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 1))
        {
            if (hitInfo.collider.gameObject.TryGetComponent<UnitController>(out UnitController controller))
            {
                if (controller.parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.Team] == parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.Team] && controller.inCombat)
                {
                    navAgent.isStopped = true;
                }
                else
                {
                    navAgent.isStopped = false;
                }
            }
        }
    }

    #endregion Unit Vision & Enemy Detection

    private void SwitchSelection()
    {
        if (ParentFormation.ParentGroup.rtsController.Selected)
        {
            selection.enabled = true;
        }
        else
        {
            selection.enabled = false;
        }
    }

    private IEnumerator WaitForAnimation(string anim, bool compareValue)
    {
        yield return new WaitUntil(() => animator.GetBool(anim) == compareValue);
    }

    #region Checks & Conditions

    private bool CheckFacing(UnitController target)
    {
        Vector3 direction = target.transform.position - transform.position;
        direction = direction.normalized;
        float angle = Vector3.Angle(transform.forward, direction);

        if (angle < 45)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private float CheckGroupFacing(UnitController target)
    {
        Vector3 direction = ParentFormation.ParentGroup.transform.position - target.ParentFormation.ParentGroup.transform.position;
        direction = direction.normalized;
        float angle = Vector3.Angle(target.ParentFormation.ParentGroup.transform.forward, direction);

        if (angle > 45 && angle < 135)
        {
            return 0.5f;
        }
        else if (angle > 135)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private bool CheckIfAlly(BT_Unit unitToCheck)
    {
        return unitToCheck.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.Team] == parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.Team];
    }

    private float CalculateDistance(Transform from, Transform to)
    {
        if (from != null && to != null)
        {
            return Vector3.SqrMagnitude(to.position - from.position);
        }
        else
        {
            return Mathf.Infinity;
        }
    }

    private void OnAllyCombatStatus(UnitController controller, bool isInCombat)
    {
        if (parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.AlliesInCombat] == null)
        {
            parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.AlliesInCombat] = new List<BT_Unit>();
        }

        if (controller != this && controller.ParentFormation.ParentGroup == this.ParentFormation.ParentGroup)
        {
            if (isInCombat && !parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.AlliesInCombat].Contains(controller.parentAI))
            {
                parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.AlliesInCombat].Add(controller.parentAI);
            }
            else if (!isInCombat && parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.AlliesInCombat].Contains(controller.parentAI))
            {
                parentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.AlliesInCombat].Remove(controller.parentAI);
            }
        }
    }

    #endregion Checks & Conditions

    #endregion Internal Methods
}