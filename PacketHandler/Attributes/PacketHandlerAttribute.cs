using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.API;

namespace PacketHandler.Attributes
{
	/// <summary>
	/// Meta-data Attribute to mark a handler class as a packet handler.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public sealed class PacketHandlerAttribute : Attribute
	{
		public NetworkOperationCode Command { get; }

		public PacketHandlerAttribute(NetworkOperationCode command)
		{
			if (!Enum.IsDefined(typeof(NetworkOperationCode), command))
				throw new ArgumentOutOfRangeException($"Provided {nameof(command)} with value {(int)command} is not a defined {nameof(NetworkOperationCode)}.");

			Command = command;
		}
	}
}
