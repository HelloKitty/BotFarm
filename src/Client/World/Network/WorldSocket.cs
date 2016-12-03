using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using Client.Authentication;
using Client.UI;
using System.IO;
using WoW.API;

namespace Client.World.Network
{
	public partial class WorldSocket : GameSocket
	{
		static HashSet<NetworkOperationCode> IgnoredOpcodes = new HashSet<NetworkOperationCode>()
		{
			NetworkOperationCode.SMSG_ADDON_INFO,
			NetworkOperationCode.SMSG_CLIENTCACHE_VERSION,
			NetworkOperationCode.SMSG_TUTORIAL_FLAGS,
			NetworkOperationCode.SMSG_WARDEN_DATA,
			NetworkOperationCode.MSG_SET_DUNGEON_DIFFICULTY,
			NetworkOperationCode.SMSG_ACCOUNT_DATA_TIMES,
			NetworkOperationCode.SMSG_FEATURE_SYSTEM_STATUS,
			NetworkOperationCode.SMSG_MOTD,
			NetworkOperationCode.SMSG_GUILD_EVENT,
			NetworkOperationCode.SMSG_GUILD_BANK_LIST,
			NetworkOperationCode.SMSG_GUILD_ROSTER,
			NetworkOperationCode.SMSG_LEARNED_DANCE_MOVES,
			NetworkOperationCode.SMSG_SET_PCT_SPELL_MODIFIER,
			NetworkOperationCode.SMSG_CONTACT_LIST,
			NetworkOperationCode.SMSG_BINDPOINTUPDATE,
			NetworkOperationCode.SMSG_INSTANCE_DIFFICULTY,
			NetworkOperationCode.SMSG_SEND_UNLEARN_SPELLS,
			NetworkOperationCode.SMSG_ACTION_BUTTONS,
			NetworkOperationCode.SMSG_EQUIPMENT_SET_LIST,
			NetworkOperationCode.SMSG_LOGIN_SETTIMESPEED,
			NetworkOperationCode.SMSG_INIT_WORLD_STATES,
			NetworkOperationCode.SMSG_UPDATE_WORLD_STATE,
			NetworkOperationCode.SMSG_WEATHER,
			NetworkOperationCode.SMSG_TIME_SYNC_REQ,
			NetworkOperationCode.SMSG_NOTIFICATION,
			NetworkOperationCode.SMSG_SPLINE_MOVE_STOP_SWIM,
			NetworkOperationCode.SMSG_SPLINE_MOVE_SET_WALK_MODE,
			NetworkOperationCode.SMSG_SPLINE_MOVE_SET_RUN_MODE,
			NetworkOperationCode.SMSG_SPLINE_MOVE_START_SWIM,
			NetworkOperationCode.MSG_MOVE_SET_FACING,
			NetworkOperationCode.SMSG_TRIGGER_CINEMATIC,
			NetworkOperationCode.SMSG_UPDATE_INSTANCE_OWNERSHIP,
			NetworkOperationCode.SMSG_EMOTE,
			NetworkOperationCode.SMSG_LFG_UPDATE_PARTY,
			NetworkOperationCode.SMSG_FORCE_SWIM_SPEED_CHANGE,
			NetworkOperationCode.SMSG_FORCE_SWIM_BACK_SPEED_CHANGE,
			NetworkOperationCode.SMSG_FORCE_RUN_SPEED_CHANGE,
			NetworkOperationCode.SMSG_FORCE_RUN_BACK_SPEED_CHANGE,
			NetworkOperationCode.SMSG_FORCE_FLIGHT_SPEED_CHANGE,
			NetworkOperationCode.SMSG_FORCE_FLIGHT_SPEED_CHANGE,
			NetworkOperationCode.SMSG_FORCE_FLIGHT_BACK_SPEED_CHANGE,
			NetworkOperationCode.CMSG_MOVE_SET_COLLISION_HGT_ACK,
			NetworkOperationCode.SMSG_ITEM_TIME_UPDATE,
			NetworkOperationCode.SMSG_SPLINE_MOVE_UNROOT,
			NetworkOperationCode.SMSG_SPELLENERGIZELOG,
			NetworkOperationCode.SMSG_PET_SPELLS,
			NetworkOperationCode.SMSG_MOVE_SET_CAN_FLY,
			NetworkOperationCode.SMSG_RECEIVED_MAIL,
			NetworkOperationCode.MSG_CHANNEL_START,
			NetworkOperationCode.MSG_CHANNEL_UPDATE,
			NetworkOperationCode.SMSG_FRIEND_STATUS,
			NetworkOperationCode.SMSG_SPLINE_MOVE_GRAVITY_ENABLE,
			NetworkOperationCode.SMSG_SPLINE_MOVE_GRAVITY_DISABLE,
			NetworkOperationCode.SMSG_SPLINE_MOVE_UNSET_FLYING,
			NetworkOperationCode.SMSG_SPLINE_MOVE_ROOT,
			NetworkOperationCode.SMSG_GAMEOBJECT_DESPAWN_ANIM,
			NetworkOperationCode.SMSG_DISMOUNT,
			NetworkOperationCode.CMSG_MOVE_FALL_RESET,
		};

		static HashSet<NetworkOperationCode> NotYetImplementedOpcodes = new HashSet<NetworkOperationCode>()
		{
			NetworkOperationCode.SMSG_SET_PROFICIENCY,
			NetworkOperationCode.SMSG_POWER_UPDATE,
			NetworkOperationCode.SMSG_CANCEL_COMBAT,
			NetworkOperationCode.SMSG_TALENTS_INFO,
			NetworkOperationCode.SMSG_INITIAL_SPELLS,
			NetworkOperationCode.SMSG_INITIALIZE_FACTIONS,
			NetworkOperationCode.SMSG_SET_FORCED_REACTIONS,
			NetworkOperationCode.SMSG_COMPRESSED_UPDATE_OBJECT,
			NetworkOperationCode.SMSG_AURA_UPDATE,
			NetworkOperationCode.SMSG_DESTROY_OBJECT,
			NetworkOperationCode.SMSG_MONSTER_MOVE,
			NetworkOperationCode.SMSG_SPELL_GO,
			NetworkOperationCode.SMSG_AURA_UPDATE_ALL,
			NetworkOperationCode.SMSG_AI_REACTION,
			NetworkOperationCode.SMSG_HIGHEST_THREAT_UPDATE,
			NetworkOperationCode.SMSG_THREAT_UPDATE,
			NetworkOperationCode.MSG_MOVE_START_FORWARD,
			NetworkOperationCode.MSG_MOVE_JUMP,
			NetworkOperationCode.MSG_MOVE_START_BACKWARD,
			NetworkOperationCode.MSG_MOVE_START_STRAFE_RIGHT,
			NetworkOperationCode.MSG_MOVE_START_TURN_RIGHT,
			NetworkOperationCode.MSG_MOVE_START_TURN_LEFT,
			NetworkOperationCode.MSG_MOVE_STOP,
			NetworkOperationCode.MSG_MOVE_STOP_TURN,
			NetworkOperationCode.MSG_MOVE_HEARTBEAT,
			NetworkOperationCode.MSG_MOVE_FALL_LAND,
			NetworkOperationCode.SMSG_SPELL_START,
			NetworkOperationCode.SMSG_SPELLHEALLOG,
			NetworkOperationCode.SMSG_ATTACKSTART,
			NetworkOperationCode.SMSG_ATTACKERSTATEUPDATE,
			NetworkOperationCode.SMSG_ATTACKSTOP,
			NetworkOperationCode.SMSG_THREAT_REMOVE,
			NetworkOperationCode.SMSG_PERIODICAURALOG,
			NetworkOperationCode.MSG_MOVE_START_STRAFE_LEFT,
			NetworkOperationCode.MSG_MOVE_STOP_STRAFE,
			NetworkOperationCode.SMSG_SPELLNONMELEEDAMAGELOG,
			NetworkOperationCode.SMSG_LOOT_LIST,
			NetworkOperationCode.SMSG_THREAT_CLEAR,
			NetworkOperationCode.SMSG_GM_MESSAGECHAT,
			NetworkOperationCode.SMSG_SET_FLAT_SPELL_MODIFIER,
			NetworkOperationCode.SMSG_SPELL_FAILURE,
			NetworkOperationCode.SMSG_SPELL_FAILED_OTHER,
			NetworkOperationCode.SMSG_MONSTER_MOVE_TRANSPORT,
			NetworkOperationCode.SMSG_MOVE_WATER_WALK,
			NetworkOperationCode.SMSG_BREAK_TARGET,
			NetworkOperationCode.SMSG_DEATH_RELEASE_LOC,
			NetworkOperationCode.SMSG_SET_PHASE_SHIFT,
			NetworkOperationCode.SMSG_PARTY_MEMBER_STATS
		};

		WorldServerInfo ServerInfo;

		private long transferred;
		public long Transferred { get { return transferred; } }

		private long sent;
		public long Sent { get { return sent; } }

		private long received;
		public long Received { get { return received; } }

		public override string LastOutOpcodeName
		{
			get
			{
				return LastOutOpcode?.ToString();
			}
		}
		public NetworkOperationCode? LastOutOpcode
		{
			get;
			protected set;
		}
		public override string LastInOpcodeName
		{
			get
			{
				return LastInOpcode?.ToString();
			}
		}
		public NetworkOperationCode? LastInOpcode
		{
			get;
			protected set;
		}

		BatchQueue<InPacket> packetsQueue = new BatchQueue<InPacket>();

		public WorldSocket(IGame program, WorldServerInfo serverInfo)
		{
			Game = program;
			ServerInfo = serverInfo;
		}

		#region Handler registration

		Dictionary<NetworkOperationCode, PacketHandler> PacketHandlers;

		public override void InitHandlers()
		{
			PacketHandlers = new Dictionary<NetworkOperationCode, PacketHandler>();

			RegisterHandlersFrom(this);
			RegisterHandlersFrom(Game);
		}

		void RegisterHandlersFrom(object obj)
		{
			// create binding flags to discover all non-static methods
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			IEnumerable<PacketHandlerAttribute> attributes;
			foreach (var method in obj.GetType().GetMethods(flags))
			{
				if (!method.TryGetAttributes(false, out attributes))
					continue;

				PacketHandler handler = (PacketHandler)PacketHandler.CreateDelegate(typeof(PacketHandler), obj, method);

				foreach (var attribute in attributes)
				{
					Game.UI.LogDebug(string.Format("Registered '{0}.{1}' to '{2}'", obj.GetType().Name, method.Name, attribute.Command));
					PacketHandlers[attribute.Command] = handler;
				}
			}
		}

		#endregion

		#region Asynchronous Reading

		int Index;
		int Remaining;
		
		private void ReadAsync(EventHandler<SocketAsyncEventArgs> callback, object state = null)
		{
			if (Disposing)
				return;

			SocketAsyncState = state;
			SocketArgs.SetBuffer(ReceiveData, Index, Remaining);
			SocketCallback = callback;
			connection.Client.ReceiveAsync(SocketArgs);
		}

		/// <summary>
		/// Determines how large the incoming header will be by
		/// inspecting the first byte, then initiates reading the header.
		/// </summary>
		private void ReadSizeCallback(object sender, SocketAsyncEventArgs e)
		{
			try
			{
				int bytesRead = e.BytesTransferred;
				if (bytesRead == 0)
				{
					// TODO: world server disconnect
					Game.UI.LogLine("Server has closed the connection");
					Game.Reconnect();
					return;
				}

				Interlocked.Increment(ref transferred);
				Interlocked.Increment(ref received);

				authenticationCrypto.Decrypt(ReceiveData, 0, 1);
				if ((ReceiveData[0] & 0x80) != 0)
				{
					// need to resize the buffer
					byte temp = ReceiveData[0];
					ReserveData(5);
					ReceiveData[0] = (byte)((0x7f & temp));

					Remaining = 4;
				}
				else
					Remaining = 3;

				Index = 1;
				ReadAsync(ReadHeaderCallback);
			}
			// these exceptions can happen as race condition on shutdown
			catch(ObjectDisposedException ex)
			{
				Game.UI.LogException(ex);
			}
			catch(NullReferenceException ex)
			{
				Game.UI.LogException(ex);
			}
			catch(InvalidOperationException ex)
			{
				Game.UI.LogException(ex);
			}
			catch(SocketException ex)
			{
				Game.UI.LogException(ex);
				Game.UI.LogLine("Last InPacket: " + LastInOpcodeName, LogLevel.Warning);
				Game.UI.LogLine("Last OutPacket: " + LastOutOpcodeName, LogLevel.Warning);
				Game.Reconnect();
			}
		}

		/// <summary>
		/// Reads the rest of the incoming header.
		/// </summary>
		private void ReadHeaderCallback(object sender, SocketAsyncEventArgs e)
		{
			try
			{
				int bytesRead = e.BytesTransferred;
				if (bytesRead == 0)
				{
					// TODO: world server disconnect
					Game.UI.LogLine("Server has closed the connection");
					Game.Reconnect();
					return;
				}

				Interlocked.Add(ref transferred, bytesRead);
				Interlocked.Add(ref received, bytesRead);

				if (bytesRead == Remaining)
				{
					// finished reading header
					// the first byte was decrypted already, so skip it
					authenticationCrypto.Decrypt(ReceiveData, 1, ReceiveDataLength - 1);
					ServerHeader header = new ServerHeader(ReceiveData, ReceiveDataLength);

					Game.UI.LogDebug(header.ToString());
					if (header.InputDataLength > 5 || header.InputDataLength < 4)
						Game.UI.LogException(String.Format("Header.InputDataLength invalid: {0}", header.InputDataLength));

					if (header.Size > 0)
					{
						// read the packet payload
						Index = 0;
						Remaining = header.Size;
						ReserveData(header.Size);
						ReadAsync(ReadPayloadCallback, header);
					}
					else
					{
						// the packet is just a header, start next packet
						QueuePacket(new InPacket(header));
						Start();
					}
				}
				else
				{
					// more header to read
					Index += bytesRead;
					Remaining -= bytesRead;
					ReadAsync(ReadHeaderCallback);
				}
			}
			// these exceptions can happen as race condition on shutdown
			catch (ObjectDisposedException ex)
			{
				Game.UI.LogException(ex);
			}
			catch (NullReferenceException ex)
			{
				Game.UI.LogException(ex);
			}
			catch (SocketException ex)
			{
				Game.UI.LogException(ex);
			}
		}

		/// <summary>
		/// Reads the payload data.
		/// </summary>
		private void ReadPayloadCallback(object sender, SocketAsyncEventArgs e)
		{
			try
			{
				int bytesRead = e.BytesTransferred;
				if (bytesRead == 0)
				{
					// TODO: world server disconnect
					Game.UI.LogLine("Server has closed the connection");
					Game.Reconnect();
					return;
				}

				Interlocked.Add(ref transferred, bytesRead);
				Interlocked.Add(ref received, bytesRead);

				if (bytesRead == Remaining)
				{
					// get header and packet, handle it
					ServerHeader header = (ServerHeader)SocketAsyncState;
					QueuePacket(new InPacket(header, ReceiveData, ReceiveDataLength));

					// start new asynchronous read
					Start();
				}
				else
				{
					// more payload to read
					Index += bytesRead;
					Remaining -= bytesRead;
					ReadAsync(ReadPayloadCallback, SocketAsyncState);
				}
			}
			catch(NullReferenceException ex)
			{
				Game.UI.LogException(ex);
			}
			catch(SocketException ex)
			{
				Game.UI.LogException(ex);
				Game.Reconnect();
				return;
			}
		}

		#endregion

		public void HandlePackets()
		{
			foreach (var packet in packetsQueue.BatchDequeue())
			{
				Game.UI.LogDebug(packet.Header.Command.ToString());
				HandlePacket(packet);
			}
		}

		private void HandlePacket(InPacket packet)
		{
			try
			{
				LastInOpcode = packet.Header.Command;
				PacketHandler handler;
				if (PacketHandlers.TryGetValue(packet.Header.Command, out handler))
				{
					Game.UI.LogDebug(string.Format("Received {0}", packet.Header.Command));
					handler(packet);
				}
				else
				{
					if (!IgnoredOpcodes.Contains(packet.Header.Command) && !NotYetImplementedOpcodes.Contains(packet.Header.Command))
						Game.UI.LogDebug(string.Format("Unknown or unhandled command '{0}'", packet.Header.Command));
				}
				Game.HandleTriggerInput(TriggerActionType.Opcode, packet);
			}
			catch(Exception ex)
			{
				Game.UI.LogException(ex);
			}
			finally
			{
				packet.Dispose();
			}
		}

		private void QueuePacket(InPacket packet)
		{
			packetsQueue.Enqueue(packet);
		}

		#region GameSocket Members

		public override void Start()
		{
			ReserveData(4, true);
			Index = 0;
			Remaining = 1;
			ReadAsync(ReadSizeCallback);
		}

		public override bool Connect()
		{
			try
			{
				Game.UI.LogDebug(string.Format("Connecting to realm {0}... ", ServerInfo.Name));

				if (connection != null)
					connection.Close();
				connection = new TcpClient(ServerInfo.Address, ServerInfo.Port);

				Game.UI.LogDebug("done!");
			}
			catch (SocketException ex)
			{
				Game.UI.LogLine(string.Format("World connect failed. ({0})", (SocketError)ex.ErrorCode), LogLevel.Error);
				return false;
			}

			return true;
		}

		#endregion

		public void Send(OutPacket packet)
		{
			LastOutOpcode = packet.Header.Command;
			byte[] data = packet.Finalize(authenticationCrypto);

			try
			{
				connection.Client.Send(data, 0, data.Length, SocketFlags.None);
			}
			catch(ObjectDisposedException ex)
			{
				Game.UI.LogException(ex);
			}
			catch(EndOfStreamException ex)
			{
				Game.UI.LogException(ex);
			}

			Interlocked.Add(ref transferred, data.Length);
			Interlocked.Add(ref sent, data.Length);
		}
	}
}
