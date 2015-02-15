using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class Army : MonoBehaviour {
	[SerializeField, Range(1, 100)]
	private int nbUnits;
	[SerializeField, Range(0f, 1f)]
	private float offensif_defensif;
	[SerializeField, Range(0f, 1f)]
	private float porte;
	[SerializeField]
	private GameObject Go;
	[SerializeField]
	private GameObject prefabUnit;

	[SerializeField]
	public string Team;

	public List<Unit> Units { get; private set; }
	private List<Army> _allies;
	private List<Army> _enemies;
	private int _limitPos;
	private Vector3 _nextPos;

	// Use this for initialization
	void Start() {
		_allies = new List<Army>();
		_enemies = new List<Army>();
		Units = new List<Unit>();

		_limitPos = Mathf.RoundToInt(Mathf.Sqrt(nbUnits)) * 2;
		_nextPos = new Vector3(Go.transform.position.x, 1, Go.transform.position.z);
		foreach (var army in GameObject.FindGameObjectsWithTag("Army").SelectMany(go => go.GetComponents<Army>()).ToList().Where(army => army != this)) {
			if (army.Team == Team)
				_allies.Add(army);
			else
				_enemies.Add(army);
		}

		for (var i = 0; i < nbUnits; ++i) {
			AddUnit();
			nextPos();
		}
	}

	// Update is called once per frame
	void Update() {
		Debug.Log(_enemies[0].Units.Count);
		var listUnitiesToRemove = new List<Unit>();
		foreach (var unit in Units) {
			if (unit.Health <= 0) {
				Debug.Log("go to remove : " + unit);
				listUnitiesToRemove.Add(unit);
				continue;
			}

			var cible =  NearestEnnemy(unit);
			if (cible != null) {
				unit.Cible = cible;
				unit.Ordre = ActionEvent.Attack;
			}
		}

		foreach (var unitToRemove in listUnitiesToRemove)
			RemoveUnit(unitToRemove);
	}

	public void AddUnit() {
		var go = Instantiate(prefabUnit) as GameObject;
		var u = go.GetComponent<Unit>();
		u.Init(
			go,
			_nextPos,
			30,
			Mathf.FloorToInt(Random.Range(10 + 5 * (1 - offensif_defensif), 5 + 5 * (1 - offensif_defensif))),
			Mathf.FloorToInt(Random.Range(1 + 5 * offensif_defensif, 5 + 5 * offensif_defensif)),
			1,
			Mathf.FloorToInt(1 + 3 * offensif_defensif),
			Mathf.FloorToInt(2 + 7 * offensif_defensif)
			);
		Units.Add(u);
	}

	public void RemoveUnit(Unit u) {
		if (!Units.Contains(u))
			return;

		u.unInitUnit();
		Units.Remove(u);
		Destroy(u.Go);
	}

	private void nextPos() {
		if (_nextPos.x + 2 - Go.transform.position.x < _limitPos) {
			_nextPos.x += 2;
		} else {
			_nextPos.x = Go.transform.position.x;
			_nextPos.y = 1;
			_nextPos.z += 2;
		}
	}

	private Unit NearestEnnemy(Unit u) {
		Unit nearestUnit = null;
		foreach (var unit in _enemies.SelectMany(army => army.Units)) {
			if (nearestUnit == null)
				nearestUnit = unit;
			else if (unit.Tr.position.Distance(u.Tr.position) < nearestUnit.Tr.position.Distance(u.Tr.position))
				nearestUnit = unit;
		}

		return nearestUnit;
	}
}