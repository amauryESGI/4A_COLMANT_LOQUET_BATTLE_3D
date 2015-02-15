public class ActionGard : Action {
	protected override void _Interact(ActionEvent actionType, Unit cible) {
		if (actionType == ActionEvent.Guard) {
			owner.isProtect = true;
			// TODO : Add modif animation
		} else if (actionType == ActionEvent.Tank) {
			var direction = cible.Cible.IsInRangeOf(owner.Tr.position);

			if (direction == 0) {
				owner.isProtect = true;
				// TODO : Add modif animation
			} else if (direction > 0) {
				owner.isProtect = false;
				// TODO : Doit se raprocher de la cible.Cible
			} else if (direction < 0) {
				owner.isProtect = false;
				// TODO : Doit s'éloigner de la cible.Cible
			} else {
				// Erreur
				throw new System.NotImplementedException();
			}
		} else {
			owner.isProtect = false;
		}
	}
}
