using UnityEngine;

public class ActionGard : Action {
	protected override void _Interact(ActionEvent actionType, Vector3 pos) {
		if (actionType == ActionEvent.Guard) {
			owner.isProtect = true;
			// TODO : protect animation ?
		} else if (actionType == ActionEvent.Tank && owner.Cible != null) {
			var direction = owner.Cible.IsInRangeOf(owner.Tr.position);
			if (direction == 0) {
				// TODO : protect animation ?
				owner.Na.Stop();
				owner.isProtect = true;
				// TODO : Add modif animation
			} else if (direction > 0) {
				owner.isProtect = false;
				owner.anim.SetBool("isRunning", true);
				//owner.anim.Play("run");
				owner.Na.SetDestination(pos);
			} else if (direction < 0) {
				owner.anim.SetBool("isRunning", true);
				//owner.anim.Play("run");
				owner.isProtect = false;
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
