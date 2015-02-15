using UnityEngine;

public enum ActionEvent {
	MoveOff,
	MoveDef,
	Attack,
	Guard,
	Tank,
	None
};

public abstract class Action {
	protected Scope _scope;

	public int value { get; private set; }
	public Unit owner { get; private set; }

	protected abstract void _Interact(ActionEvent actionType, Unit entity);

	private void Interact(ActionEvent actionType, Unit entity) {
		try {
			_Interact(actionType, entity);
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