using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WoW.API;

namespace Client.World.Network
{
	public interface Header
	{
		NetworkOperationCode Command { get; }
		int Size { get; }
	}
}
