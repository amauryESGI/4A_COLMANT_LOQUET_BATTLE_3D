using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public delegate void UnitInteraction(ActionEvent actionType, Vector3 pos);

public enum EType {
	None = 0,
	Tank,
	Soldat,
	Arche
}

public class Unit : MonoBehaviour, IComparer<Unit> {

	#region Declaration
	public event UnitInteraction OnEntityInteraction;

	public bool isProtect = false;

	public EType Type { get; set; }

	public GameObject Go { get; private set; }
	public Transform Tr { get; private set; }
	public NavMeshAgent Na;
	public MeshRenderer Render;

	public Unit Cible { get; set; }
	public ActionEvent Ordre { get; set; }

	public int Health { get; private set; }

	public Action ActionAttack { get; private set; }
	public Action ActionGuard { get; private set; }
	public Action ActionMove { get; private set; }

	private Scope _scope;

	public	float _intervalOfUpdate	 = 1f;
	private float _nextUpdate		 = 0f;

	#endregion Declaration

	public void Init(GameObject me, Vector3 pos, int health, int damage, int shield, float speed, int scopeMin, int scopeMax) {
		Go = me;
		Tr = Go.transform;
		Tr.position = pos;

		Health = health;
		Ordre = ActionEvent.None;
		Type = EType.None;
		Na.speed *= speed;
		_scope = new Scope(scopeMin, scopeMax);

		ActionAttack = new ActionAttack();
		ActionAttack.initAction(_scope, damage);
		ActionAttack.AddListener(this);

		ActionGuard = new ActionGard();
		ActionGuard.initAction(_scope, shield);
		ActionGuard.AddListener(this);

		ActionMove = new ActionMove();
		ActionMove.initAction(_scope, 1);
		ActionMove.AddListener(this);
	}

	public void unInitUnit() {
		ActionAttack.RemoveListener(this);
		ActionGuard.RemoveListener(this);
		ActionMove.RemoveListener(this);
	}

	public void Interact(ActionEvent action, Vector3 pos) {
		if (OnEntityInteraction != null)
			OnEntityInteraction(action, pos);
	}

	void Update() {
		if (Health <= 0)
			Ordre = ActionEvent.None;

		if (Time.time > _nextUpdate) {
			if (Ordre == ActionEvent.Contourn) {
				_nextUpdate = Time.time + _intervalOfUpdate;
			} else if (Ordre != ActionEvent.None && Cible != null) {
				//_nextUpdate = Time.time + _intervalOfUpdate + _intervalOfUpdate > 5 ? _rand : 0;
				_nextUpdate = Time.time + _intervalOfUpdate;
				Interact(Ordre, Cible.Tr.position);
			} else {
				if (Cible == null)
					Ordre = ActionEvent.None;

				// TODO : Stop animation (Wait)
				Na.Stop();
			}
		}
	}

	public int IsInRangeOf(Vector3 target) {
		return _scope.IsInRangeOf(Tr.position, target);
	}

	public void takeDamage(int value) {
		// TODO : Take Damage Animation
		if (isProtect)
			value -= ActionGuard.value;

		if (value > 0 && Health - value >= 0)
			Health -= value;
		else
			Health = 0;
	}

	public int Compare(Unit x, Unit y) {
		return Tr.position.CompartUnitDistance(x, y);
	}

	public int CompareLifeTo(Unit other) {
		// A null value means that this object is greater.
		if (other == null)
			return 1;
		return Health.CompareTo(other.Health);
	}

	public Unit NearestUnit(IEnumerable<Unit> units) {
		Unit nearestUnit = null;
		foreach (var unit in units) {
			if (nearestUnit == null) // TODO Amaury : change null
				nearestUnit = unit;
			else if (unit.Tr.position.Distance(Tr.position) < nearestUnit.Tr.position.Distance(Tr.position))
				nearestUnit = unit;
		}

		return nearestUnit;
	}

	public List<Unit> NearestUnits(IEnumerable<Unit> units) {
		var nearestUnits = new List<Unit>(units);
		nearestUnits.Sort(Compare);

		return nearestUnits;
	}

	public List<Unit> UnitsInRangeOf(IEnumerable<Unit> units) {
		return units.Where(unit => IsInRangeOf(unit.Tr.position) == 0).ToList();
	}
}