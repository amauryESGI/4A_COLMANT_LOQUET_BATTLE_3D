using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;

public enum EAnimation {
	isDead = 0,
	isAttacking,
	isRunning,
	isHit,
	isWinning,
	isWaiting,
	isIddle,
	isDanse
}

public class Army : MonoBehaviour {
	#region Declaration
	[SerializeField, Range(1, 5000)]
	private int nbUnits;
	private int _nbUnits;
	[SerializeField, Range(0f, 1f)]
	private float offensif_defensif;
	[SerializeField, Range(0f, 1f)]
	private float porte;
	[SerializeField]
	private GameObject Go;

	[SerializeField]
	private GameObject prefabSoldat;
	[SerializeField]
	private GameObject prefabArchee;
	[SerializeField]
	private GameObject prefabTank;

	[SerializeField]
	private bool isRandom;
	[SerializeField]
	private bool isContourn;
	[SerializeField]
	public string Team;

	public List<Unit> Units { get; private set; }
	public List<Unit> UnitsContourn { get; private set; }
	private List<Army> _allies;
	private List<Army> _enemies;
	private int _limitPos;
	private Vector3 _nextPos;

	private float _coefDamageMin;
	private float _coefDamageMax;
	private float _coefShieldMin;
	private float _coefShieldMax;
	private float _coefScopeMin;
	private float _coefScopeMid;
	private float _coefScopeMax;

	private List<Unit> _listUnitsToRemove;
	private List<Unit> _listUnitsContournToChange;

	private int _nbUnitsArch;
	private int _nbUnitsTank;
	private int _nbUnitsSoldat;

	public	float _intervalOfUpdate = 1f;
	private float _nextUpdate;
	private const float _coefContourn = -1f;

	#endregion Declaration

	void Start() {
		_allies = new List<Army>();
		_enemies = new List<Army>();
		Units = new List<Unit>();

		// DEBUG
		if (nbUnits > 500) {
			if (nbUnits < 1000)
				_nbUnits = 500;
			else
				_nbUnits = nbUnits / 2;
		} else
			_nbUnits = nbUnits;

		// RELEASE
		//_nbUnits = nbUnits;

		UnitsContourn = new List<Unit>();
		_listUnitsToRemove = new List<Unit>();
		_listUnitsContournToChange = new List<Unit>();

		_limitPos = Mathf.RoundToInt(Mathf.Sqrt(_nbUnits)) * 4;
		_nextPos = new Vector3(Go.transform.position.x, 1, Go.transform.position.z);
		foreach (var army in GameObject.FindGameObjectsWithTag("Army").SelectMany(go => go.GetComponents<Army>()).ToList().Where(army => army != this)) {
			if (army.Team == Team)
				_allies.Add(army);
			else
				_enemies.Add(army);
		}

		_coefDamageMin = 5 + 5 * (1 - offensif_defensif);
		_coefDamageMax = 10 + 5 * (1 - offensif_defensif);
		_coefShieldMin = 1 + 5 * offensif_defensif;
		_coefShieldMax = 5 + 5 * offensif_defensif;
		_coefScopeMin = 1 + 5 * porte;
		_coefScopeMid = _coefScopeMin + 1;
		_coefScopeMax = 3 + 7 * porte;

		if (isRandom) {
			for (var i = 0; i < _nbUnits; ++i) {
				StartCoroutine(AddUnitRand(_nextPos));
				NextPos();
			}
		} else {
			_nbUnitsArch = Mathf.FloorToInt(_nbUnits * porte);
			_nbUnitsSoldat = Mathf.FloorToInt((_nbUnits - _nbUnitsArch) * (1 - offensif_defensif));
			_nbUnitsTank = _nbUnits - _nbUnitsSoldat - _nbUnitsArch;


			if (Random.Range(0, 1) == 1 || isContourn) {
				//Debug.Log(string.Format("Army {0}\nArche : {1}, Soldat : {2}, Soldat Contourne : {2}, Tank : {3}", Team, _nbUnitsArch, _nbUnitsSoldat / 2, _nbUnitsTank));

				for (int i = 0; i < _nbUnitsSoldat / 2; i++) {
					StartCoroutine(AddUnitSoldat(_nextPos));
					NextPos();
				}

				for (int i = 0; i < _nbUnitsTank; i++) {
					StartCoroutine(AddUnitTank(_nextPos));
					NextPos();
				}

				for (int i = 0; i < _nbUnitsArch; i++) {
					StartCoroutine(AddUnitArch(_nextPos));
					NextPos();
				}

				for (int i = 0; i < _nbUnitsSoldat / 2; i++) {
					StartCoroutine(AddUnitContourn(_nextPos));
					UnitsContourn[i].Na.speed = 2f;
					NextPos();
				}
			} else {
				//Debug.Log(string.Format("Army {0}\nArche : {1}, Soldat : {2}, Tank : {3}", Team, _nbUnitsArch, _nbUnitsSoldat, _nbUnitsTank));

				for (int i = 0; i < _nbUnitsSoldat; i++) {
					StartCoroutine(AddUnitSoldat(_nextPos));
					NextPos();
				}

				for (int i = 0; i < _nbUnitsTank; i++) {
					StartCoroutine(AddUnitTank(_nextPos));
					NextPos();
				}

				for (int i = 0; i < _nbUnitsArch; i++) {
					StartCoroutine(AddUnitArch(_nextPos));
					NextPos();
				}
			}

			foreach (var unit in Units) {
				switch (unit.Type) {
					case EType.None:
						unit.Render.materials[0].color = Color.white;
						break;
					case EType.Arche:
						unit.Render.materials[0].color = Color.yellow;
						break;
					case EType.Soldat:
						unit.Render.materials[0].color = Color.red;
						break;
					case EType.Tank:
						unit.Render.materials[0].color = Color.black;
						break;
					default:
						Debug.Log("error type");
						break;
				}
			}

			foreach (var unit in UnitsContourn)
				unit.Render.materials[0].color = Color.blue;

			_nextUpdate = 0;
		}
	}

	void Update() {
		if (!_enemies.SelectMany(enemy => enemy.Units).Any()) {
			foreach (var unit in Units)
				unit.SetAnimation(EAnimation.isDanse);
			return;
		}

		var fps = 1.0f / Time.deltaTime;
		if (fps < 30 && _intervalOfUpdate < 5)
			_intervalOfUpdate *= 1.5f;
		else if (fps > 60 && _intervalOfUpdate > 0.1)
			_intervalOfUpdate *= 0.9f;

		var posToContourn = new Vector3();
		foreach (var unit in UnitsContourn) {
			if (unit.Health <= 0) {
				_listUnitsToRemove.Add(unit);
				continue;
			}
			unit._intervalOfUpdate = _intervalOfUpdate;

			if (Time.time < _nextUpdate) {
				if (unit.Ordre == ActionEvent.None) {
					var C_enemy = GravityCenter(_enemies.SelectMany(enemy => enemy.Units));
					var C_army = GravityCenter(Units);
					var distance = C_army.Distance(C_enemy) / 2;
					posToContourn.x = distance + _coefContourn * ((C_army.z + C_enemy.z) / 2 - C_enemy.z) + C_enemy.x;
					posToContourn.z = distance + _coefContourn * ((C_army.x + C_enemy.x) / 2 - C_enemy.x) + C_enemy.z;

					StartCoroutine(UpdateUnitIAContourn(unit, posToContourn));
				}
			} else {
				if (_intervalOfUpdate < 5) {
					var C_enemy = GravityCenter(_enemies.SelectMany(enemy => enemy.Units));
					var C_army = GravityCenter(Units);
					var distance = C_army.Distance(C_enemy) / 2;
					posToContourn.x = distance + _coefContourn * ((C_army.z + C_enemy.z) / 2 - C_enemy.z) + C_enemy.x;
					posToContourn.z = distance + _coefContourn * ((C_army.x + C_enemy.x) / 2 - C_enemy.x) + C_enemy.z;

					StartCoroutine(UpdateUnitIAContourn(unit, posToContourn));
				}
			}
		}

		foreach (var unitToRemove in _listUnitsContournToChange) {
			UnitsContourn.Remove(unitToRemove);
			Units.Remove(unitToRemove);
		}
		_listUnitsContournToChange.Clear();

		foreach (var unit in Units) {
			if (unit.Health <= 0) {
				_listUnitsToRemove.Add(unit);
				continue;
			}
			unit._intervalOfUpdate = _intervalOfUpdate;

			if (Time.time < _nextUpdate) {
				if (unit.Ordre == ActionEvent.None)
					StartCoroutine(UpdateUnitIA(unit));
				_nextUpdate = Time.time + _intervalOfUpdate;
			} else {
				_nextUpdate = Time.time + _intervalOfUpdate;
				//if (_intervalOfUpdate < 5)
					StartCoroutine(UpdateUnitIA(unit));
			}
		}
	}

	void LateUpdate() {
		foreach (var unitToRemove in _listUnitsToRemove)
			StartCoroutine(RemoveUnitC(unitToRemove));
		_listUnitsToRemove.Clear();
	}

	private IEnumerator UpdateUnitIAContourn(Unit unit, Vector3 pos) {
		var unitsInRange = unit.UnitsInRangeOf(_enemies.SelectMany(enemy => enemy.Units));

		if (unitsInRange.Count > 0) {
			_listUnitsContournToChange.Add(unit);
		} else {
			unit.SetAnimation(EAnimation.isRunning);
			//unit.anim.Play("run");
			unit.Ordre = ActionEvent.Contourn;
			unit.Na.SetDestination(pos);
		}

		yield return null;
	}

	private IEnumerator UpdateUnitIA(Unit unit) {
		Unit cible;
		switch (unit.Type) {
			case EType.Arche:
				var unitsInRange = unit.UnitsInRangeOf(_enemies.SelectMany(enemy => enemy.Units));
				if (unitsInRange.Count > 0) {
					unit.Cible = unitsInRange[0];
					unit.Ordre = ActionEvent.Attack;
					break;
				}

				var tmp = new List<Unit>(Units);
				tmp.AddRange(_allies.SelectMany(army => army.Units));

				cible = unit.NearestUnit(tmp.Where(u => u.Type == EType.Tank));
				if (cible != null) {
					unit.Cible = cible;
					unit.Ordre = ActionEvent.Support;
					break;
				}
				cible = unit.NearestUnit(tmp.Where(u => u.Type == EType.Soldat));
				if (cible != null) {
					unit.Cible = cible;
					unit.Ordre = ActionEvent.Support;
					break;
				}
				cible = NearestEnnemy(unit);
				if (cible != null) {
					unit.Cible = cible;
					unit.Ordre = ActionEvent.Attack;
				}
				break;
			case EType.Soldat:
				cible = NearestEnnemy(unit);
				if (cible != null) {
					unit.Cible = cible;
					unit.Ordre = ActionEvent.Attack;
				}
				break;
			case EType.Tank:
				cible = NearestEnnemy(unit);
				if (cible != null) {
					unit.Cible = cible;
					unit.Ordre = ActionEvent.Tank;
				}
				break;
			default:
				Debug.Log("error type");
				break;
		}

		yield return null;
	}

	private IEnumerator AddUnitRand(Vector3 pos) {
		var go = (GameObject)Instantiate(prefabSoldat);
		var u = go.GetComponent<Unit>();

		var damage		= Mathf.FloorToInt(Random.Range(_coefDamageMin, _coefDamageMax));
		var shield		= Mathf.FloorToInt(Random.Range(_coefShieldMin, _coefShieldMax));
		var scopeMin	= Mathf.FloorToInt(Random.Range(1, _coefScopeMin));
		var scopeMax	= Mathf.FloorToInt(Random.Range(_coefScopeMid, _coefScopeMax));

		u.Init(
			go,
			pos,
			30,
			damage,
			shield,
			1,
			scopeMin,
			scopeMax
			);

		if (offensif_defensif > 0.5) {
			if (damage > _coefDamageMin + ((_coefDamageMax - _coefDamageMin) / 2)) {
				if ((scopeMin > (_coefScopeMin - 1) / 2) && (scopeMax > (_coefScopeMax - _coefScopeMid) / 2)) {
					u.Type = EType.Arche;
				} else {
					u.Type = EType.Soldat;
				}
			} else {
				if ((scopeMin > 1 + ((_coefScopeMin - 1) / 2)) || (scopeMax > _coefScopeMid + ((_coefScopeMax - _coefScopeMid) / 2))) {
					u.Type = EType.Arche;
				} else {
					u.Type = EType.Tank;
				}
			}
		} else {
			if (shield > _coefShieldMin + ((_coefShieldMax - _coefShieldMin) / 2)) {
				if ((scopeMin > 1 + ((_coefScopeMin - 1) / 2)) || (scopeMax > _coefScopeMid + ((_coefScopeMax - _coefScopeMid) / 2))) {
					u.Type = EType.Arche;
				} else {
					u.Type = EType.Tank;
				}
			} else {
				if ((scopeMin > 1 + ((_coefScopeMin - 1) / 2)) || (scopeMax > _coefScopeMid + ((_coefScopeMax - _coefScopeMid) / 2))) {
					u.Type = EType.Arche;
				} else {
					u.Type = EType.Soldat;
				}
			}
		}

		Units.Add(u);
		yield return null;
	}

	private IEnumerator AddUnitArch(Vector3 pos) {
		var go = (GameObject)Instantiate(prefabArchee);
		var u = go.GetComponent<Unit>();

		u.Init(
			go,
			pos,
			10,		// Life
			13,		// Damage
			5,		// Shield
			1.25f,	// Speed
			5,		// Scope Min
			10		// Scope Max
			);
		u.Type = EType.Arche;

		Units.Add(u);
		yield return null;
	}

	private IEnumerator AddUnitTank(Vector3 pos) {
		var go = (GameObject)Instantiate(prefabTank);
		var u = go.GetComponent<Unit>();

		u.Init(
			go,
			pos,
			30,		// Life
			5,		// Damage
			7,		// Shield
			0.75f,	// Speed
			1,		// Scope Min
			3		// Scope Max
			);
		u.Type = EType.Tank;

		Units.Add(u);
		yield return null;
	}

	private IEnumerator AddUnitSoldat(Vector3 pos) {
		var go = (GameObject)Instantiate(prefabSoldat);
		var u = go.GetComponent<Unit>();

		u.Init(
			go,
			pos,
			20,		// Life
			10,		// Damage
			5,		// Shield
			1f,		// Speed
			1,		// Scope Min
			3		// Scope Max
			);
		u.Type = EType.Soldat;

		Units.Add(u);
		yield return null;
	}

	private IEnumerator AddUnitContourn(Vector3 pos) {
		var go = (GameObject)Instantiate(prefabSoldat);
		var u = go.GetComponent<Unit>();

		u.Init(
			go,
			pos,
			30,		// Life
			10,		// Damage
			5,		// Shield
			1f,		// Speed
			1,		// Scope Min
			3		// Scope Max
			);
		u.Type = EType.Soldat;

		UnitsContourn.Add(u);
		yield return null;
	}

	private void RemoveUnit(Unit u) {
		u.SetAnimation(EAnimation.isDead);
		if (Units.Contains(u))
			Units.Remove(u);
		else if (UnitsContourn.Contains(u))
			UnitsContourn.Remove(u);

		u.unInitUnit();
		Destroy(u.Go);
	}

	private IEnumerator RemoveUnitC(Unit u) {
		u.SetAnimation(EAnimation.isDead);
		if (Units.Contains(u))
			Units.Remove(u);
		else if (UnitsContourn.Contains(u))
			UnitsContourn.Remove(u);

		u.unInitUnit();
		Destroy(u.Go);
		yield return null;
	}

	private void NextPos() {
		if (_nextPos.x + 2 - Go.transform.position.x < _limitPos) {
			_nextPos.x += 2;
		} else {
			_nextPos.x = Go.transform.position.x;
			_nextPos.y = 1;
			_nextPos.z += 2;
		}
	}

	private Unit NearestEnnemy(Unit u) {
		return u.NearestUnit(_enemies.SelectMany(army => army.Units));
	}

	private List<Unit> NearestEnnemies(Unit u) {
		return u.NearestUnits(_enemies.SelectMany(army => army.Units));
	}

	private Vector3 GravityCenter(IEnumerable<Unit> units) {
		return units.Aggregate(new Vector3(0, 0, 0), (current, unit) => current + unit.Tr.position) / units.Count();
	}

	private int RayonCenter(Vector3 centre, IEnumerable<Unit> units) {
		if (units == null || !units.Any())
			return 0;

		Unit nearestUnit = null;

		foreach (var unit in units) {
			if (nearestUnit == null) // TODO Amaury : change null
				nearestUnit = unit;
			else if (unit.Tr.position.Distance(centre) < nearestUnit.Tr.position.Distance(centre))
				nearestUnit = unit;
		}

		return centre.Distance(nearestUnit.Tr.position);
	}
}