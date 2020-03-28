using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0,0,0,0]")]
	public partial class NetworkedFlagNetworkObject : NetworkObject
	{
		public const int IDENTITY = 6;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private bool _isFlagThere;
		public event FieldEvent<bool> isFlagThereChanged;
		public Interpolated<bool> isFlagThereInterpolation = new Interpolated<bool>() { LerpT = 0f, Enabled = false };
		public bool isFlagThere
		{
			get { return _isFlagThere; }
			set
			{
				// Don't do anything if the value is the same
				if (_isFlagThere == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_isFlagThere = value;
				hasDirtyFields = true;
			}
		}

		public void SetisFlagThereDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_isFlagThere(ulong timestep)
		{
			if (isFlagThereChanged != null) isFlagThereChanged(_isFlagThere, timestep);
			if (fieldAltered != null) fieldAltered("isFlagThere", _isFlagThere, timestep);
		}
		[ForgeGeneratedField]
		private int _teamIndex;
		public event FieldEvent<int> teamIndexChanged;
		public Interpolated<int> teamIndexInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int teamIndex
		{
			get { return _teamIndex; }
			set
			{
				// Don't do anything if the value is the same
				if (_teamIndex == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_teamIndex = value;
				hasDirtyFields = true;
			}
		}

		public void SetteamIndexDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_teamIndex(ulong timestep)
		{
			if (teamIndexChanged != null) teamIndexChanged(_teamIndex, timestep);
			if (fieldAltered != null) fieldAltered("teamIndex", _teamIndex, timestep);
		}
		[ForgeGeneratedField]
		private int _type;
		public event FieldEvent<int> typeChanged;
		public Interpolated<int> typeInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int type
		{
			get { return _type; }
			set
			{
				// Don't do anything if the value is the same
				if (_type == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x4;
				_type = value;
				hasDirtyFields = true;
			}
		}

		public void SettypeDirty()
		{
			_dirtyFields[0] |= 0x4;
			hasDirtyFields = true;
		}

		private void RunChange_type(ulong timestep)
		{
			if (typeChanged != null) typeChanged(_type, timestep);
			if (fieldAltered != null) fieldAltered("type", _type, timestep);
		}
		[ForgeGeneratedField]
		private float _radius;
		public event FieldEvent<float> radiusChanged;
		public InterpolateFloat radiusInterpolation = new InterpolateFloat() { LerpT = 0f, Enabled = false };
		public float radius
		{
			get { return _radius; }
			set
			{
				// Don't do anything if the value is the same
				if (_radius == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x8;
				_radius = value;
				hasDirtyFields = true;
			}
		}

		public void SetradiusDirty()
		{
			_dirtyFields[0] |= 0x8;
			hasDirtyFields = true;
		}

		private void RunChange_radius(ulong timestep)
		{
			if (radiusChanged != null) radiusChanged(_radius, timestep);
			if (fieldAltered != null) fieldAltered("radius", _radius, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			isFlagThereInterpolation.current = isFlagThereInterpolation.target;
			teamIndexInterpolation.current = teamIndexInterpolation.target;
			typeInterpolation.current = typeInterpolation.target;
			radiusInterpolation.current = radiusInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _isFlagThere);
			UnityObjectMapper.Instance.MapBytes(data, _teamIndex);
			UnityObjectMapper.Instance.MapBytes(data, _type);
			UnityObjectMapper.Instance.MapBytes(data, _radius);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_isFlagThere = UnityObjectMapper.Instance.Map<bool>(payload);
			isFlagThereInterpolation.current = _isFlagThere;
			isFlagThereInterpolation.target = _isFlagThere;
			RunChange_isFlagThere(timestep);
			_teamIndex = UnityObjectMapper.Instance.Map<int>(payload);
			teamIndexInterpolation.current = _teamIndex;
			teamIndexInterpolation.target = _teamIndex;
			RunChange_teamIndex(timestep);
			_type = UnityObjectMapper.Instance.Map<int>(payload);
			typeInterpolation.current = _type;
			typeInterpolation.target = _type;
			RunChange_type(timestep);
			_radius = UnityObjectMapper.Instance.Map<float>(payload);
			radiusInterpolation.current = _radius;
			radiusInterpolation.target = _radius;
			RunChange_radius(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _isFlagThere);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _teamIndex);
			if ((0x4 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _type);
			if ((0x8 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _radius);

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
				if (isFlagThereInterpolation.Enabled)
				{
					isFlagThereInterpolation.target = UnityObjectMapper.Instance.Map<bool>(data);
					isFlagThereInterpolation.Timestep = timestep;
				}
				else
				{
					_isFlagThere = UnityObjectMapper.Instance.Map<bool>(data);
					RunChange_isFlagThere(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (teamIndexInterpolation.Enabled)
				{
					teamIndexInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					teamIndexInterpolation.Timestep = timestep;
				}
				else
				{
					_teamIndex = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_teamIndex(timestep);
				}
			}
			if ((0x4 & readDirtyFlags[0]) != 0)
			{
				if (typeInterpolation.Enabled)
				{
					typeInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					typeInterpolation.Timestep = timestep;
				}
				else
				{
					_type = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_type(timestep);
				}
			}
			if ((0x8 & readDirtyFlags[0]) != 0)
			{
				if (radiusInterpolation.Enabled)
				{
					radiusInterpolation.target = UnityObjectMapper.Instance.Map<float>(data);
					radiusInterpolation.Timestep = timestep;
				}
				else
				{
					_radius = UnityObjectMapper.Instance.Map<float>(data);
					RunChange_radius(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (isFlagThereInterpolation.Enabled && !isFlagThereInterpolation.current.UnityNear(isFlagThereInterpolation.target, 0.0015f))
			{
				_isFlagThere = (bool)isFlagThereInterpolation.Interpolate();
				//RunChange_isFlagThere(isFlagThereInterpolation.Timestep);
			}
			if (teamIndexInterpolation.Enabled && !teamIndexInterpolation.current.UnityNear(teamIndexInterpolation.target, 0.0015f))
			{
				_teamIndex = (int)teamIndexInterpolation.Interpolate();
				//RunChange_teamIndex(teamIndexInterpolation.Timestep);
			}
			if (typeInterpolation.Enabled && !typeInterpolation.current.UnityNear(typeInterpolation.target, 0.0015f))
			{
				_type = (int)typeInterpolation.Interpolate();
				//RunChange_type(typeInterpolation.Timestep);
			}
			if (radiusInterpolation.Enabled && !radiusInterpolation.current.UnityNear(radiusInterpolation.target, 0.0015f))
			{
				_radius = (float)radiusInterpolation.Interpolate();
				//RunChange_radius(radiusInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public NetworkedFlagNetworkObject() : base() { Initialize(); }
		public NetworkedFlagNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public NetworkedFlagNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
