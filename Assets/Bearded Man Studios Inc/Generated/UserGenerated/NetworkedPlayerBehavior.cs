using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[][\"string\"][][][\"string\", \"int\"][\"uint\", \"string\", \"int\", \"Vector3\"][\"int\", \"Vector3\"][\"bool\"][\"Vector3\"][][]]")]
	[GeneratedRPCVariableNames("{\"types\":[[][\"\"][][][\"killerName\", \"killerTeam\"][\"id\", \"killerName\", \"killerTeam\", \"viewDir\"][\"\", \"\"][\"\"][\"direction\"][][]]")]
	public abstract partial class NetworkedPlayerBehavior : NetworkBehavior
	{
		public const byte RPC_ATTACK = 0 + 5;
		public const byte RPC_CHANGE_NAME = 1 + 5;
		public const byte RPC_JUMP = 2 + 5;
		public const byte RPC_LAND = 3 + 5;
		public const byte RPC_DIE = 4 + 5;
		public const byte RPC_TRY_HIT = 5 + 5;
		public const byte RPC_INIT = 6 + 5;
		public const byte RPC_TOGGLE_FLAG = 7 + 5;
		public const byte RPC_KNOCKBACK = 8 + 5;
		public const byte RPC_DEBUG_ATTACK = 9 + 5;
		public const byte RPC_HITMARKER = 10 + 5;
		
		public NetworkedPlayerNetworkObject networkObject = null;

		public override void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;
			
			networkObject = (NetworkedPlayerNetworkObject)obj;
			networkObject.AttachedBehavior = this;

			base.SetupHelperRpcs(networkObject);
			networkObject.RegisterRpc("Attack", Attack);
			networkObject.RegisterRpc("ChangeName", ChangeName, typeof(string));
			networkObject.RegisterRpc("Jump", Jump);
			networkObject.RegisterRpc("Land", Land);
			networkObject.RegisterRpc("Die", Die, typeof(string), typeof(int));
			networkObject.RegisterRpc("TryHit", TryHit, typeof(uint), typeof(string), typeof(int), typeof(Vector3));
			networkObject.RegisterRpc("Init", Init, typeof(int), typeof(Vector3));
			networkObject.RegisterRpc("ToggleFlag", ToggleFlag, typeof(bool));
			networkObject.RegisterRpc("Knockback", Knockback, typeof(Vector3));
			networkObject.RegisterRpc("DebugAttack", DebugAttack);
			networkObject.RegisterRpc("Hitmarker", Hitmarker);

			networkObject.onDestroy += DestroyGameObject;

			if (!obj.IsOwner)
			{
				if (!skipAttachIds.ContainsKey(obj.NetworkId)){
					uint newId = obj.NetworkId + 1;
					ProcessOthers(gameObject.transform, ref newId);
				}
				else
					skipAttachIds.Remove(obj.NetworkId);
			}

			if (obj.Metadata != null)
			{
				byte transformFlags = obj.Metadata[0];

				if (transformFlags != 0)
				{
					BMSByte metadataTransform = new BMSByte();
					metadataTransform.Clone(obj.Metadata);
					metadataTransform.MoveStartIndex(1);

					if ((transformFlags & 0x01) != 0 && (transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() =>
						{
							transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform);
							transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform);
						});
					}
					else if ((transformFlags & 0x01) != 0)
					{
						MainThreadManager.Run(() => { transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform); });
					}
					else if ((transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() => { transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform); });
					}
				}
			}

			MainThreadManager.Run(() =>
			{
				NetworkStart();
				networkObject.Networker.FlushCreateActions(networkObject);
			});
		}

		protected override void CompleteRegistration()
		{
			base.CompleteRegistration();
			networkObject.ReleaseCreateBuffer();
		}

		public override void Initialize(NetWorker networker, byte[] metadata = null)
		{
			Initialize(new NetworkedPlayerNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject(NetWorker sender)
		{
			MainThreadManager.Run(() => { try { Destroy(gameObject); } catch { } });
			networkObject.onDestroy -= DestroyGameObject;
		}

		public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new NetworkedPlayerNetworkObject(networker, this, createCode, metadata);
		}

		protected override void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void Attack(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void ChangeName(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void Jump(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void Land(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void Die(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void TryHit(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void Init(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void ToggleFlag(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void Knockback(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void DebugAttack(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void Hitmarker(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}