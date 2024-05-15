using UnityEngine;

public class MissileController : MonoBehaviour
{
    [SerializeField] private AnimationCurve missileCurve;
    [SerializeField] private float missileSpeed = 1;
    [SerializeField] private float missileArchHeight = 1.5f;
    [SerializeField] private float missileDecayTime = 2.5f;
    [SerializeField] private GameObject bloodEfffect;

    private Vector3 pos;
    private Vector3 target;
    private float time = 0;
    private float curvePos = 0;
    private float startDistance = 0;
    private float currentDistance = 0;
    private float damage = 0;
    private UnitController parentController;
    private bool canDamage = true;

    public UnitController ParentController { get => parentController; set => parentController = value; }
    public float Damage { get => damage; set => damage = value; }
    public Vector3 Target { get => target; set => target = value; }

    private void Start()
    {
        pos = transform.position;
        startDistance = Vector3.Distance(pos, target);
    }

    private void Update()
    {
        time += Time.deltaTime;

        pos = Vector3.Lerp(pos, target, time * (missileSpeed * 0.1f));
        currentDistance = Vector3.Distance(pos, target);

        if (currentDistance > 0)
        {
            if (missileDecayTime <= 0)
            {
                Destroy(this.gameObject);
            }
            else if (currentDistance < 2)
            {
                missileDecayTime -= Time.deltaTime;
            }

            curvePos = missileCurve.Evaluate(1 - (currentDistance / startDistance));
            pos.y = curvePos * (missileArchHeight * startDistance * 0.1f);
            transform.position = pos;

            if (currentDistance > startDistance * 0.5f)
            {
                transform.LookAt(new Vector3(target.x, target.y + (missileArchHeight * startDistance * 0.1f), target.z));
            }
            else
            {
                transform.LookAt(new Vector3(target.x, target.y - 1.5f, target.z));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canDamage)
        {
            if (other.CompareTag("Unit"))
            {
                if (other.gameObject.TryGetComponent<UnitController>(out UnitController controller) && controller.ParentFormation.ParentGroup.Team != parentController.ParentFormation.ParentGroup.Team)
                {
                    controller.Health -= damage;
                    if (controller.ParentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.TargetEnemy] == null)
                    {
                        controller.ParentAI.Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.TargetEnemy] = ParentController;
                    }
                    if (!controller.InCombat)
                    {
                        controller.InCombat = true;
                    }
                    Instantiate<GameObject>(bloodEfffect, controller.transform.position, Quaternion.identity);
                    canDamage = false;
                }
            }
        }
    }
}