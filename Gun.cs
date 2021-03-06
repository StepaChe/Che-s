﻿using UnityEngine;

namespace Geekbrains
{
	public sealed class Gun : Weapon
    {
		public override void Fire()
		{
			if (!_isReady) return;
			if (Clip.CountAmmunition <= 0) return;
			//if (!Ammunition) return;
            var bullet = Resources.Load<Bullet>("Bullet");
			var temAmmunition = Instantiate(bullet, _barrel.position, _barrel.rotation);// Pool object
			temAmmunition.AddForce(_barrel.forward * _force);
			Clip.CountAmmunition--;
			_isReady = false;
			Invoke(nameof(ReadyShoot), _rechergeTime);
			//_timer.Start(_rechergeTime);
		}
	}
}