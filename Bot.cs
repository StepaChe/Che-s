﻿using System;
using UnityEngine;
using UnityEngine.AI;

namespace Geekbrains
{
	public sealed class Bot : BaseObjectScene
	{
		public float Hp = 100;
		public Vision Vision;
		public Weapon Weapon; // с разным оружием 
		public Renderer[] Materials;
		public Transform Target { get; set; }
		public NavMeshAgent Agent { get; private set; }
		private float _waitTime = 3;
		private StateBot _stateBot;
		private Vector3 _point;

		private StateBot StateBot
		{
			get => _stateBot;
			set
			{
				_stateBot = value;
				if (Materials == null) return;
				switch (value)
				{
					case StateBot.Non:
						foreach (var material in Materials)
						{
							material.material.color = Color.white;
						}
						break;
					case StateBot.Patrol:
						foreach (var material in Materials)
						{
							material.material.color = Color.green;
						}
						break;
					case StateBot.Inspection:
						foreach (var material in Materials)
						{
							material.material.color = Color.yellow;
						}
						break;
					case StateBot.Detected:
						foreach (var material in Materials)
						{
							material.material.color = Color.red;
						}
						break;
					case StateBot.Died:
						foreach (var material in Materials)
						{
							material.material.color = Color.grey;
						}
						break;
					default:
						{
							foreach (var material in Materials)
							{
								material.material.color = Color.white;
							}
							break;
						}
				}

			}
		}

		protected override void Awake()
		{
			base.Awake();
			Agent = GetComponent<NavMeshAgent>();

			//var dam = GetComponentsInChildren<ISetDamage>();
			//foreach (var setDamage in dam)
			//{
				
			//}
			var bodyBot = GetComponentInChildren<BodyBot>();
			if (bodyBot != null) bodyBot.OnApplyDamageChange += SetDamage;

			var headBot = GetComponentInChildren<HeadBot>();
			if (headBot != null) headBot.OnApplyDamageChange += SetDamage;
		}

		public void Tick()
		{
			if (StateBot == StateBot.Died) return;

			if (StateBot != StateBot.Detected)
			{
				if (!Agent.hasPath)
				{
                    InPatrol();
                }

				if (Vision.VisionM(transform, Target))
				{
					StateBot = StateBot.Detected;
				}
			}
			else
			{
				Agent.SetDestination(Target.position);
				Agent.stoppingDistance = 3;
				if (Vision.VisionM(transform, Target))
				{
					// остановиться 
					Weapon.Fire();
				}
                else 
                {
                    // Потеря персонажа
                    InPatrol();
                }
            }
		}

		public void SetDamage(InfoCollision info)
		{
			if (Hp > 0)
			{
				Hp -= info.Damage;
			}

			if (Hp <= 0)
			{
				StateBot = StateBot.Died;
				Agent.enabled = false;
				foreach (var child in GetComponentsInChildren<Transform>())
				{
					child.parent = null;
					var tempRbChild = child.GetComponent<Rigidbody>();
					if (!tempRbChild)
					{
						tempRbChild = child.gameObject.AddComponent<Rigidbody>();
					}
					//tempRbChild.AddForce(info.Dir * Random.Range(10, 300));
					
					Destroy(child.gameObject, 10);
				}
				Main.Instance.BotController.RemoveBotToList(this);// унести
			}
		}

		private void ReadyPatrol()
		{
			StateBot = StateBot.Non;
		}

		public void MovePoint(Vector3 point)
		{
			Agent.SetDestination(point);
		}

        private void InPatrol()
        {
            if (StateBot != StateBot.Inspection)
            {
                if (StateBot != StateBot.Patrol)
                {
                    StateBot = StateBot.Patrol;
                    _point = Patrol.GenericPoint(transform);
                    Agent.SetDestination(_point);
                    Agent.stoppingDistance = 0;
                }
                else
                {
                    if (Vector3.Distance(_point, transform.position) <= 1)
                    {
                        StateBot = StateBot.Inspection;
                        Invoke(nameof(ReadyPatrol), _waitTime);
                    }
                }
            }
        }
	}
}