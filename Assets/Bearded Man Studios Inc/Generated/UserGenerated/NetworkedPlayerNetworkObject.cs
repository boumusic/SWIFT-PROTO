using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0.5,0.5,0,0,0,0]")]
	public partial class NetworkedPlayerNetworkObject : NetworkObject
	{
		public const int IDENTITY = 9;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private Vector3 _position;
		public event FieldEvent<Vector3> positionChanged;
		public InterpolateVector3 positionInterpolation = new InterpolateVector3() { LerpT = 0.5f, Enabled = true };
		public Vector3 position
		{
			get { return _position; }
			set
			{
				// Don't do anything if the value is the same
				if (_position == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_position = value;
				hasDirtyFields = true;
			}
		}

		public void SetpositionDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_position(ulong timestep)
		{
			if (positionChanged != null) positionChanged(_position, timestep);
			if (fieldAltered != null) fieldAltered("position", _position, timestep);
		}
		[ForgeGeneratedField]
		private Quaternion _rotation;
		public event FieldEvent<Quaternion> rotationChanged;
		public InterpolateQuaternion rotationInterpolation = new InterpolateQuaternion() { LerpT = 0.5f, Enabled = true };
		public Quaternion rotation
		{
			get { return _rotation; }
			set
			{
				// Don't do anything if the value is the same
				if (_rotation == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_rotation = value;
				hasDirtyFields = true;
			}
		}

		public void SetrotationDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_rotation(ulong timestep)
		{
			if (rotationChanged != null) rotationChanged(_rotation, timestep);
			if (fieldAltered != null) fieldAltered("rotation", _rotation, timestep);
		}
		[ForgeGeneratedField]
		private Vector3 _localVelocity;
		public event FieldEvent<Vector3> localVelocityChanged;
		public InterpolateVector3 localVelocityInterpolation = new InterpolateVector3() { LerpT = 0f, Enabled = false };
		public Vector3 localVelocity
		{
			get { return _localVelocity; }
			set
			{
				// Don't do anything if the value is the same
				if (_localVelocity == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x4;
				_localVelocity = value;
				hasDirtyFields = true;
			}
		}

		public void SetlocalVelocityDirty()
		{
			_dirtyFields[0] |= 0x4;
			hasDirtyFields = true;
		}

		private void RunChange_localVelocity(ulong timestep)
		{
			if (localVelocityChanged != null) localVelocityChanged(_localVelocity, timestep);
			if (fieldAltered != null) fieldAltered("localVelocity", _localVelocity, timestep);
		}
		[ForgeGeneratedField]
		private bool _climbing;
		public event FieldEvent<bool> climbingChanged;
		public Interpolated<bool> climbingInterpolation = new Interpolated<bool>() { LerpT = 0f, Enabled = false };
		public bool climbing
		{
			get { return _climbing; }
			set
			{
				// Don't do anything if the value is the same
				if (_climbing == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x8;
				_climbing = value;
				hasDirtyFields = true;
			}
		}

		public void SetclimbingDirty()
		{
			_dirtyFields[0] |= 0x8;
			hasDirtyFields = true;
		}

		private void RunChange_climbing(ulong timestep)
		{
			if (climbingChanged != null) climbingChanged(_climbing, timestep);
			if (fieldAltered != null) fieldAltered("climbing", _climbing, timestep);
		}
		[ForgeGeneratedField]
		private bool _running;
		public event FieldEvent<bool> runningChanged;
		public Interpolated<bool> runningInterpolation = new Interpolated<bool>() { LerpT = 0f, Enabled = false };
		public bool running
		{
			get { return _running; }
			set
			{
				// Don't do anything if the value is the same
				if (_running == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x10;
				_running = value;
				hasDirtyFields = true;
			}
		}

		public void SetrunningDirty()
		{
			_dirtyFields[0] |= 0x10;
			hasDirtyFields = true;
		}

		private void RunChange_running(ulong timestep)
		{
			if (runningChanged != null) runningChanged(_running, timestep);
			if (fieldAltered != null) fieldAltered("running", _running, timestep);
		}
		[ForgeGeneratedField]
		private bool _alive;
		public event FieldEvent<bool> aliveChanged;
		public Interpolated<bool> aliveInterpolation = new Interpolated<bool>() { LerpT = 0f, Enabled = false };
		public bool alive
		{
			get { return _alive; }
			set
			{
				// Don't do anything if the value is the same
				if (_alive == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x20;
				_alive = value;
				hasDirtyFields = true;
			}
		}

		public void SetaliveDirty()
		{
			_dirtyFields[0] |= 0x20;
			hasDirtyFields = true;
		}

		private void RunChange_alive(ulong timestep)
		{
			if (aliveChanged != null) aliveChanged(_alive, timestep);
			if (fieldAltered != null) fieldAltered("alive", _alive, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			positionInterpolation.current = positionInterpolation.target;
			rotationInterpolation.current = rotationInterpolation.target;
			localVelocityInterpolation.current = localVelocityInterpolation.target;
			climbingInterpolation.current = climbingInterpolation.target;
			runningInterpolation.current = runningInterpolation.target;
			aliveInterpolation.current = aliveInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _position);
			UnityObjectMapper.Instance.MapBytes(data, _rotation);
			UnityObjectMapper.Instance.MapBytes(data, _localVelocity);
			UnityObjectMapper.Instance.MapBytes(data, _climbing);
			UnityObjectMapper.Instance.MapBytes(data, _running);
			UnityObjectMapper.Instance.MapBytes(data, _alive);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_position = UnityObjectMapper.Instance.Map<Vector3>(payload);
			positionInterpolation.current = _position;
			positionInterpolation.target = _position;
			RunChange_position(timestep);
			_rotation = UnityObjectMapper.Instance.Map<Quaternion>(payload);
			rotationInterpolation.current = _rotation;
			rotationInterpolation.target = _rotation;
			RunChange_rotation(timestep);
			_localVelocity = UnityObjectMapper.Instance.Map<Vector3>(payload);
			localVelocityInterpolation.current = _localVelocity;
			localVelocityInterpolation.target = _localVelocity;
			RunChange_localVelocity(timestep);
			_climbing = UnityObjectMapper.Instance.Map<bool>(payload);
			climbingInterpolation.current = _climbing;
			climbingInterpolation.target = _climbing;
			RunChange_climbing(timestep);
			_running = UnityObjectMapper.Instance.Map<bool>(payload);
			runningInterpolation.current = _running;
			runningInterpolation.target = _running;
			RunChange_running(timestep);
			_alive = UnityObjectMapper.Instance.Map<bool>(payload);
			aliveInterpolation.current = _alive;
			aliveInterpolation.target = _alive;
			RunChange_alive(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _position);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _rotation);
			if ((0x4 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _localVelocity);
			if ((0x8 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _climbing);
			if ((0x10 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _running);
			if ((0x20 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _alive);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (positionInterpolation.Enabled)
				{
					positionInterpolation.target = UnityObjectMapper.Instance.Map<Vector3>(data);
					positionInterpolation.Timestep = timestep;
				}
				else
				{
					_position = UnityObjectMapper.Instance.Map<Vector3>(data);
					RunChange_position(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (rotationInterpolation.Enabled)
				{
					rotationInterpolation.target = UnityObjectMapper.Instance.Map<Quaternion>(data);
					rotationInterpolation.Timestep = timestep;
				}
				else
				{
					_rotation = UnityObjectMapper.Instance.Map<Quaternion>(data);
					RunChange_rotation(timestep);
				}
			}
			if ((0x4 & readDirtyFlags[0]) != 0)
			{
				if (localVelocityInterpolation.Enabled)
				{
					localVelocityInterpolation.target = UnityObjectMapper.Instance.Map<Vector3>(data);
					localVelocityInterpolation.Timestep = timestep;
				}
				else
				{
					_localVelocity = UnityObjectMapper.Instance.Map<Vector3>(data);
					RunChange_localVelocity(timestep);
				}
			}
			if ((0x8 & readDirtyFlags[0]) != 0)
			{
				if (climbingInterpolation.Enabled)
				{
					climbingInterpolation.target = UnityObjectMapper.Instance.Map<bool>(data);
					climbingInterpolation.Timestep = timestep;
				}
				else
				{
					_climbing = UnityObjectMapper.Instance.Map<bool>(data);
					RunChange_climbing(timestep);
				}
			}
			if ((0x10 & readDirtyFlags[0]) != 0)
			{
				if (runningInterpolation.Enabled)
				{
					runningInterpolation.target = UnityObjectMapper.Instance.Map<bool>(data);
					runningInterpolation.Timestep = timestep;
				}
				else
				{
					_running = UnityObjectMapper.Instance.Map<bool>(data);
					RunChange_running(timestep);
				}
			}
			if ((0x20 & readDirtyFlags[0]) != 0)
			{
				if (aliveInterpolation.Enabled)
				{
					aliveInterpolation.target = UnityObjectMapper.Instance.Map<bool>(data);
					aliveInterpolation.Timestep = timestep;
				}
				else
				{
					_alive = UnityObjectMapper.Instance.Map<bool>(data);
					RunChange_alive(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (positionInterpolation.Enabled && !positionInterpolation.current.UnityNear(positionInterpolation.target, 0.0015f))
			{
				_position = (Vector3)positionInterpolation.Interpolate();
				//RunChange_position(positionInterpolation.Timestep);
			}
			if (rotationInterpolation.Enabled && !rotationInterpolation.current.UnityNear(rotationInterpolation.target, 0.0015f))
			{
				_rotation = (Quaternion)rotationInterpolation.Interpolate();
				//RunChange_rotation(rotationInterpolation.Timestep);
			}
			if (localVelocityInterpolation.Enabled && !localVelocityInterpolation.current.UnityNear(localVelocityInterpolation.target, 0.0015f))
			{
				_localVelocity = (Vector3)localVelocityInterpolation.Interpolate();
				//RunChange_localVelocity(localVelocityInterpolation.Timestep);
			}
			if (climbingInterpolation.Enabled && !climbingInterpolation.current.UnityNear(climbingInterpolation.target, 0.0015f))
			{
				_climbing = (bool)climbingInterpolation.Interpolate();
				//RunChange_climbing(climbingInterpolation.Timestep);
			}
			if (runningInterpolation.Enabled && !runningInterpolation.current.UnityNear(runningInterpolation.target, 0.0015f))
			{
				_running = (bool)runningInterpolation.Interpolate();
				//RunChange_running(runningInterpolation.Timestep);
			}
			if (aliveInterpolation.Enabled && !aliveInterpolation.current.UnityNear(aliveInterpolation.target, 0.0015f))
			{
				_alive = (bool)aliveInterpolation.Interpolate();
				//RunChange_alive(aliveInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public NetworkedPlayerNetworkObject() : base() { Initialize(); }
		public NetworkedPlayerNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public NetworkedPlayerNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
