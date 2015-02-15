public class ActionMove : Action {
	protected override void _Interact(ActionEvent actionType, Unit cible) {
		if (actionType == ActionEvent.MoveOff) {
			// TODO: move on cible
			throw new System.NotImplementedException();
		}
	}
}