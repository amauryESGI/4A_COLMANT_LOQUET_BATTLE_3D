using System;
using UnityEngine;

public class ActionAttack : Action {
	private float _cooldown = 2.0f;
	private float _nextAttack;

	protected override void _Interact(ActionEvent actionType, Vector3 pos) {

		if (actionType == ActionEvent.Attack) {
			var direction = owner.IsInRangeOf(pos);

			if (direction == 0) {
				owner.Na.Stop();
				if (Time.time > _nextAttack) {
					owner.anim.SetBool("isAttacking", true);
					_nextAttack = Time.time + _cooldown;
					owner.Cible.takeDamage(value);
				}
			} else if (direction > 0) {
				owner.anim.SetBool("isRunning", true);
				//owner.anim.Play("run");
				owner.Na.SetDestination(pos);
			} else if (direction < 0) {
				owner.anim.SetBool("isRunning", true);
				//owner.anim.Play("run"); 
				owner.Na.SetDestination(owner.Tr.position.PosOpposite(pos));
			} else {
				// Erreur
				throw new System.NotImplementedException();
			}
		} else if (actionType == ActionEvent.Support && owner.Cible.Cible != null) {
			var destination = Math.Abs(owner.Cible.Na.velocity.sqrMagnitude) < 0.1f ? owner.Cible.Cible.Tr.position : pos;

			var direction = owner.IsInRangeOf(destination);

			if (direction == 0) {
				owner.Na.Stop();
				if (Time.time > _nextAttack) {
					_nextAttack = Time.time + _cooldown;
					owner.Cible.Cible.takeDamage(value);
				}
			} else if (direction > 0) {
				owner.anim.SetBool("isRunning", true);
				//owner.anim.Play("run"); 
				owner.Na.SetDestination(destination);
			} else if (direction < 0) {
				owner.anim.SetBool("isRunning", true);
				//owner.anim.Play("run"); 
				owner.Na.SetDestination(owner.Tr.position.PosOpposite(destination));
			} else {
				// Erreur
				throw new System.NotImplementedException();
			}
		}
	}
}