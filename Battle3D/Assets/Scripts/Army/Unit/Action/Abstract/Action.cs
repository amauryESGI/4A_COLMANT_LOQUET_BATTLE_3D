using UnityEngine;

public enum ActionEvent {
	MoveOff,
	MoveDef,
	Attack,
	Support,
	Guard,
	Tank,
	None,
	Contourn
};

public abstract class Action {
	protected Scope _scope;

	public int value { get; private set; }
	public Unit owner { get; private set; }

	protected abstract void _Interact(ActionEvent actionType, Vector3 pos);

	private void Interact(ActionEvent actionType, Vector3 pos) {
		try {
			_Interact(actionType, pos);
		} catch (System.Exception e) {
			Debug.LogException(e);
		}
	}

	public void initAction(Scope scope, int value) {
		_scope = scope;
		this.value = value;
	}

	public void AddListener(Unit actuator) {
		owner = actuator;
		actuator.OnEntityInteraction += Interact;
	}

	public void RemoveListener(Unit actuator) {
		actuator.OnEntityInteraction -= Interact;
	}
}