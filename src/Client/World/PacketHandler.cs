using System;
using Client.World.Network;
using WoW.API;

namespace Client.World
{
	public delegate void PacketHandler(InPacket packet);

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public sealed class PacketHandlerAttribute : Attribute
	{
		public NetworkOperationCode Command { get; private set; }

		public PacketHandlerAttribute(NetworkOperationCode command)
		{
			Command = command;
		}
	}
}
