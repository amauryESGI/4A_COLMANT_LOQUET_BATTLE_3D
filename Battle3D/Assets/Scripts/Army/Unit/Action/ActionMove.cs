using UnityEngine;

public class ActionMove : Action {
	protected override void _Interact(ActionEvent actionType, Vector3 pos) {
		if (actionType == ActionEvent.MoveOff) {
			// TODO: move on cible
			throw new System.NotImplementedException();
		}
	}
}