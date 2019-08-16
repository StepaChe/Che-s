﻿using System.Collections.Generic;
using UnityEngine;

namespace Geekbrains
{
	public class BotController : BaseController, IOnUpdate, IInitialization
	{
		public int CountBot = 5;
		public List<Bot> GetBotList { get; } = new List<Bot>(5);

		public void OnStart()
		{
			for (var index = 0; index < CountBot; index++)
			{
                var bot = Resources.Load<Bot>("Bot");
				var tempBot = Object.Instantiate(//Main.Instance.RefBotPrefab,
                    bot,
					Patrol.GenericPoint(Main.Instance.Player),
					Quaternion.identity);

				tempBot.Agent.avoidancePriority = index;
				tempBot.Target = Main.Instance.Player; // разных противников
				AddBotToList(tempBot);
			}
		}

		private void AddBotToList(Bot bot)
		{
			if (!GetBotList.Contains(bot))
			{
				GetBotList.Add(bot);
			}
		}
		public void RemoveBotToList(Bot bot)
		{
			if (GetBotList.Contains(bot))
			{
				GetBotList.Remove(bot);
			}
		}

		public void OnUpdate()
		{
			if (!IsActive) return;
			foreach (var bot in GetBotList)
			{
				bot.Tick();
			}
		}
	}
}