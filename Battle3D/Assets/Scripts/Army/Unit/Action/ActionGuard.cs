using UnityEngine;

public class ActionGard : Action {
	protected override void _Interact(ActionEvent actionType, Vector3 pos) {
		if (actionType == ActionEvent.Guard) {
			owner.isProtect = true;
			// TODO : Add modif animation
		} else if (actionType == ActionEvent.Tank && owner.Cible != null) {
			var direction = owner.Cible.IsInRangeOf(owner.Tr.position);
			if (direction == 0) {
				owner.Na.Stop();
				owner.isProtect = true;
				// TODO : Add modif animation
			} else if (direction > 0) {
				owner.isProtect = false;
				// TODO : Doit se raprocher de la cible.Cible
				owner.Na.SetDestination(pos);
			} else if (direction < 0) {
				owner.isProtect = false;
				// TODO : Doit s'éloigner de la cible.Cible
				owner.Na.SetDestination(owner.Tr.position.PosOpposite(pos));
			} else {
				// Erreur
				throw new System.NotImplementedException();
			}
		} else {
			owner.isProtect = false;
		}
	}
}
