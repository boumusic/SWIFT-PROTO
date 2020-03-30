using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0,0,0,0]")]
	public partial class CTFGameStateNetworkObject : NetworkObject
	{
		public const int IDENTITY = 10;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private int _team0Score;
		public event FieldEvent<int> team0ScoreChanged;
		public Interpolated<int> team0ScoreInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int team0Score
		{
			get { return _team0Score; }
			set
			{
				// Don't do anything if the value is the same
				if (_team0Score == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_team0Score = value;
				hasDirtyFields = true;
			}
		}

		public void Setteam0ScoreDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_team0Score(ulong timestep)
		{
			if (team0ScoreChanged != null) team0ScoreChanged(_team0Score, timestep);
			if (fieldAltered != null) fieldAltered("team0Score", _team0Score, timestep);
		}
		[ForgeGeneratedField]
		private int _team1Score;
		public event FieldEvent<int> team1ScoreChanged;
		public Interpolated<int> team1ScoreInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int team1Score
		{
			get { return _team1Score; }
			set
			{
				// Don't do anything if the value is the same
				if (_team1Score == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_team1Score = value;
				hasDirtyFields = true;
			}
		}

		public void Setteam1ScoreDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_team1Score(ulong timestep)
		{
			if (team1ScoreChanged != null) team1ScoreChanged(_team1Score, timestep);
			if (fieldAltered != null) fieldAltered("team1Score", _team1Score, timestep);
		}
		[ForgeGeneratedField]
		private float _timer;
		public event FieldEvent<float> timerChanged;
		public InterpolateFloat timerInterpolation = new InterpolateFloat() { LerpT = 0f, Enabled = false };
		public float timer
		{
			get { return _timer; }
			set
			{
				// Don't do anything if the value is the same
				if (_timer == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x4;
				_timer = value;
				hasDirtyFields = true;
			}
		}

		public void SettimerDirty()
		{
			_dirtyFields[0] |= 0x4;
			hasDirtyFields = true;
		}

		private void RunChange_timer(ulong timestep)
		{
			if (timerChanged != null) timerChanged(_timer, timestep);
			if (fieldAltered != null) fieldAltered("timer", _timer, timestep);
		}
		[ForgeGeneratedField]
		private float _originalTimer;
		public event FieldEvent<float> originalTimerChanged;
		public InterpolateFloat originalTimerInterpolation = new InterpolateFloat() { LerpT = 0f, Enabled = false };
		public float originalTimer
		{
			get { return _originalTimer; }
			set
			{
				// Don't do anything if the value is the same
				if (_originalTimer == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x8;
				_originalTimer = value;
				hasDirtyFields = true;
			}
		}

		public void SetoriginalTimerDirty()
		{
			_dirtyFields[0] |= 0x8;
			hasDirtyFields = true;
		}

		private void RunChange_originalTimer(ulong timestep)
		{
			if (originalTimerChanged != null) originalTimerChanged(_originalTimer, timestep);
			if (fieldAltered != null) fieldAltered("originalTimer", _originalTimer, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			team0ScoreInterpolation.current = team0ScoreInterpolation.target;
			team1ScoreInterpolation.current = team1ScoreInterpolation.target;
			timerInterpolation.current = timerInterpolation.target;
			originalTimerInterpolation.current = originalTimerInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _team0Score);
			UnityObjectMapper.Instance.MapBytes(data, _team1Score);
			UnityObjectMapper.Instance.MapBytes(data, _timer);
			UnityObjectMapper.Instance.MapBytes(data, _originalTimer);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_team0Score = UnityObjectMapper.Instance.Map<int>(payload);
			team0ScoreInterpolation.current = _team0Score;
			team0ScoreInterpolation.target = _team0Score;
			RunChange_team0Score(timestep);
			_team1Score = UnityObjectMapper.Instance.Map<int>(payload);
			team1ScoreInterpolation.current = _team1Score;
			team1ScoreInterpolation.target = _team1Score;
			RunChange_team1Score(timestep);
			_timer = UnityObjectMapper.Instance.Map<float>(payload);
			timerInterpolation.current = _timer;
			timerInterpolation.target = _timer;
			RunChange_timer(timestep);
			_originalTimer = UnityObjectMapper.Instance.Map<float>(payload);
			originalTimerInterpolation.current = _originalTimer;
			originalTimerInterpolation.target = _originalTimer;
			RunChange_originalTimer(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _team0Score);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _team1Score);
			if ((0x4 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _timer);
			if ((0x8 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _originalTimer);

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
				if (team0ScoreInterpolation.Enabled)
				{
					team0ScoreInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					team0ScoreInterpolation.Timestep = timestep;
				}
				else
				{
					_team0Score = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_team0Score(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (team1ScoreInterpolation.Enabled)
				{
					team1ScoreInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					team1ScoreInterpolation.Timestep = timestep;
				}
				else
				{
					_team1Score = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_team1Score(timestep);
				}
			}
			if ((0x4 & readDirtyFlags[0]) != 0)
			{
				if (timerInterpolation.Enabled)
				{
					timerInterpolation.target = UnityObjectMapper.Instance.Map<float>(data);
					timerInterpolation.Timestep = timestep;
				}
				else
				{
					_timer = UnityObjectMapper.Instance.Map<float>(data);
					RunChange_timer(timestep);
				}
			}
			if ((0x8 & readDirtyFlags[0]) != 0)
			{
				if (originalTimerInterpolation.Enabled)
				{
					originalTimerInterpolation.target = UnityObjectMapper.Instance.Map<float>(data);
					originalTimerInterpolation.Timestep = timestep;
				}
				else
				{
					_originalTimer = UnityObjectMapper.Instance.Map<float>(data);
					RunChange_originalTimer(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (team0ScoreInterpolation.Enabled && !team0ScoreInterpolation.current.UnityNear(team0ScoreInterpolation.target, 0.0015f))
			{
				_team0Score = (int)team0ScoreInterpolation.Interpolate();
				//RunChange_team0Score(team0ScoreInterpolation.Timestep);
			}
			if (team1ScoreInterpolation.Enabled && !team1ScoreInterpolation.current.UnityNear(team1ScoreInterpolation.target, 0.0015f))
			{
				_team1Score = (int)team1ScoreInterpolation.Interpolate();
				//RunChange_team1Score(team1ScoreInterpolation.Timestep);
			}
			if (timerInterpolation.Enabled && !timerInterpolation.current.UnityNear(timerInterpolation.target, 0.0015f))
			{
				_timer = (float)timerInterpolation.Interpolate();
				//RunChange_timer(timerInterpolation.Timestep);
			}
			if (originalTimerInterpolation.Enabled && !originalTimerInterpolation.current.UnityNear(originalTimerInterpolation.target, 0.0015f))
			{
				_originalTimer = (float)originalTimerInterpolation.Interpolate();
				//RunChange_originalTimer(originalTimerInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public CTFGameStateNetworkObject() : base() { Initialize(); }
		public CTFGameStateNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public CTFGameStateNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
