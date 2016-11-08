using System;
using System.Linq;
using Rebronx.Server.DataSenders.Interfaces;

namespace Rebronx.Server.DataSenders
{
	public class JoinSender : IJoinSender
	{
		private readonly ILobbySender lobbySender;
		
		public JoinSender(ILobbySender lobbySender)
		{
			this.lobbySender = lobbySender;
		}

		public void Join(Player player)
		{
			var position = player?.Position;
			if (position != null)
			{
				lobbySender.Update(position);
			}
		}
	}
}