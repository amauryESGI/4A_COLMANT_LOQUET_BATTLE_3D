using UnityEngine;

public class ActionAttack : Action {
	private float _cooldown = 2.0f;
	private float _nextAttack;

	protected override void _Interact(ActionEvent actionType, Unit cible) {
		if (actionType == ActionEvent.Attack) {
			var direction = owner.IsInRangeOf(cible.Tr.position);

			if (direction == 0) {
				owner.Na.Stop();
				if (Time.time > _nextAttack)
				{
					owner.Render.materials[0].color = Color.red;
					_nextAttack = Time.time + _cooldown;
					cible.takeDamage(value);
				}
			} else if (direction > 0) {
				owner.Render.materials[0].color = Color.cyan;
				owner.Na.SetDestination(cible.Tr.position);
			} else if (direction < 0) {
				owner.Render.materials[0].color = Color.green;
				owner.Na.SetDestination(owner.Tr.position.PosOpposite(cible.Tr.position));
				// TODO : Doit s'éloigner de la cible
			} else {
				// Erreur
				throw new System.NotImplementedException();
			}
		}
	}
}