using UnityEngine;

public delegate void UnitInteraction(ActionEvent actionType, Unit entity);

public class Unit : MonoBehaviour {
	public event UnitInteraction OnEntityInteraction;

	public	bool isProtect	= false;

	public GameObject	Go { get; private set; }
	public Transform	Tr { get; private set; }
	[SerializeField] public NavMeshAgent Na;
	[SerializeField] public MeshRenderer Render;

	public Unit Cible { get; set; }
	public ActionEvent Ordre { get; set; }

	public int Health { get; private set; }

	public Action ActionAttack { get; private set; }
	public Action ActionGuard { get; private set; }
	public Action ActionMove { get; private set; }

	private Scope _scope;

	public void Init(GameObject me, Vector3 pos, int health, int damage, int shield, int speed, int scopeMin, int scopeMax) {
		Go = me;
		Tr = Go.transform;
		Tr.position = pos;

		Health = health;
		Ordre = ActionEvent.None;

		_scope = new Scope(scopeMin, scopeMax);

		ActionAttack = new ActionAttack();
		ActionAttack.initAction(_scope, damage);
		ActionAttack.AddListener(this);

		ActionGuard = new ActionGard();
		ActionGuard.initAction(_scope, shield);
		ActionGuard.AddListener(this);

		ActionMove = new ActionMove();
		ActionMove.initAction(_scope, speed);
		ActionMove.AddListener(this);
	}

	public void unInitUnit() {
		ActionAttack.RemoveListener(this);
		ActionGuard.RemoveListener(this);
		ActionMove.RemoveListener(this);
	}

	public void Interact(ActionEvent action, Unit entity) {
		if (OnEntityInteraction != null)
			OnEntityInteraction(action, entity);
	}

	void Update() {
		if (Health<=0)
			Ordre = ActionEvent.None;

		if (Ordre != ActionEvent.None && Cible != null)
			Interact(Ordre, Cible);
	}

	public int IsInRangeOf(Vector3 target) {
		return _scope.IsInRangeOf(Tr.position, target);
	}

	public void takeDamage(int value) {
		if (isProtect)
			value -= ActionGuard.value;

		if (value > 0 && Health - value >= 0)
			Health -= value;
		else
			Health = 0;
	}
}