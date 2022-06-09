namespace SMPL
{
	internal class Thing
	{
		private int order;
		internal readonly static Dictionary<string, Thing> objs = new();
		internal readonly static SortedDictionary<int, List<Thing>> objsOrder = new();

		private readonly List<string> childrenUIDs = new();
		private string parentUID;
		private Vector2 localPos;
		private float localAng, localSc;
		private Matrix3x2 global;
		private string uid;

		public string UID
		{
			get => uid;
			set
			{
				if(objs.ContainsKey(value))
				{
					Console.LogError(1, $"Another {{{nameof(Thing)}}} already exists with the [{nameof(UID)}] '{value}'.");
					return;
				}

				if(uid != null)
					objs.Remove(uid);

				uid = value;
				objs[uid] = this;
			}
		}
		public string ParentUID
		{
			get => parentUID;
			set
			{
				if(parentUID == value)
					return;

				var parent = Get(parentUID);

				if(parent != null)
					parent.childrenUIDs.Remove(uid);

				var prevPos = Position;
				var prevAng = Angle;
				var prevSc = Scale;

				parentUID = value;

				var newParent = Get(parentUID);
				if(newParent == null)
					return;

				newParent.childrenUIDs.Add(uid);

				Position = prevPos;
				Angle = prevAng;
				Scale = prevSc;
			}
		}
		public ReadOnlyCollection<string> ChildrenUIDs => childrenUIDs.AsReadOnly();
		public int UpdateOrder
		{
			get => order;
			set
			{
				TryCreateOrder(order);

				if(objsOrder[order].Contains(this))
					objsOrder[order].Remove(this);

				order = value;

				TryCreateOrder(order);
				objsOrder[order].Add(this);

				void TryCreateOrder(int order)
				{
					if(objsOrder.ContainsKey(order) == false)
						objsOrder[order] = new();
				}
			}
		}

		public Vector2 LocalPosition
		{
			get => localPos;
			set { localPos = value; UpdateSelfAndChildren(); }
		}
		public float LocalScale
		{
			get => localSc;
			set { localSc = value; UpdateSelfAndChildren(); }
		}
		public float LocalAngle
		{
			get => localAng;
			set { localAng = value; UpdateSelfAndChildren(); }
		}
		public Vector2 LocalDirection
		{
			get => Vector2.Normalize(LocalAngle.AngleToDirection());
			set => LocalAngle = Vector2.Normalize(value).DirectionToAngle();
		}

		public Vector2 Position
		{
			get => GetPosition(global);
			set => LocalPosition = GetLocalPositionFromParent(value);
		}
		public float Scale
		{
			get => GetScale(global);
			set => LocalScale = GetScale(GlobalToLocal(value, Angle, Position));
		}
		public float Angle
		{
			get => GetAngle(global);
			set => LocalAngle = GetAngle(GlobalToLocal(Scale, value, Position));
		}
		public Vector2 Direction
		{
			get => Vector2.Normalize(Angle.AngleToDirection());
			set => Angle = Vector2.Normalize(value).DirectionToAngle();
		}

		public string TypeName => GetType().Name;

		public Vector2 GetLocalPositionFromParent(Vector2 position)
		{
			return GetPosition(GlobalToLocal(Scale, Angle, position));
		}
		public Vector2 GetPositionFromParent(Vector2 localPosition)
		{
			return GetPosition(LocalToGlobal(LocalScale, LocalAngle, localPosition));
		}
		public Vector2 GetLocalPositionFromSelf(Vector2 position)
		{
			var m = Matrix3x2.Identity;
			m *= Matrix3x2.CreateTranslation(position);
			m *= Matrix3x2.CreateTranslation(Position);

			return GetPosition(m);
		}
		public Vector2 GetPositionFromSelf(Vector2 localPosition)
		{
			var m = Matrix3x2.Identity;
			m *= Matrix3x2.CreateTranslation(localPosition);
			m *= Matrix3x2.CreateRotation(LocalAngle.DegreesToRadians());
			m *= Matrix3x2.CreateScale(LocalScale);
			m *= Matrix3x2.CreateTranslation(LocalPosition);

			return GetPosition(m * GetParentMatrix());
		}

		public void Destroy(bool includeChildren = true)
		{
			OnDestroy();

			if(includeChildren)
				for(int i = 0; i < childrenUIDs.Count; i++)
				{
					var child = Get(childrenUIDs[i]);
					child?.Destroy();
				}
			objs.Remove(uid);
			objsOrder[order].Remove(this);
		}
		public virtual Vector2 CornerClockwise(int index) => Position;

		public override string ToString()
		{
			return uid;
		}

		#region Backend
		internal Thing(string uid)
		{
			UID = uid;
			UpdateOrder = 0;
			LocalScale = 1;
		}

		internal void Update() => OnUpdate();
		internal virtual void OnUpdate() { }
		internal virtual void OnDestroy() { }

		internal Matrix3x2 LocalToGlobal(float localScale, float localAngle, Vector2 localPosition)
		{
			var c = Matrix3x2.Identity;
			c *= Matrix3x2.CreateScale(localScale);
			c *= Matrix3x2.CreateRotation(localAngle.DegreesToRadians());
			c *= Matrix3x2.CreateTranslation(localPosition);

			return c * GetParentMatrix();
		}
		internal Matrix3x2 GlobalToLocal(float scale, float angle, Vector2 position)
		{
			var c = Matrix3x2.Identity;
			c *= Matrix3x2.CreateScale(scale);
			c *= Matrix3x2.CreateRotation(angle.DegreesToRadians());
			c *= Matrix3x2.CreateTranslation(position);

			return c * GetInverseParentMatrix();
		}
		internal Matrix3x2 GetParentMatrix()
		{
			var p = Matrix3x2.Identity;
			var parent = Get(parentUID);
			if(parent != null)
			{
				p *= Matrix3x2.CreateScale(parent.Scale);
				p *= Matrix3x2.CreateRotation(parent.Angle.DegreesToRadians());
				p *= Matrix3x2.CreateTranslation(parent.Position);
			}
			return p;
		}
		internal Matrix3x2 GetInverseParentMatrix()
		{
			var inverseParent = Matrix3x2.Identity;
			var parent = Get(parentUID);
			if(parent != null)
			{
				Matrix3x2.Invert(Matrix3x2.CreateScale(parent.Scale), out var s);
				Matrix3x2.Invert(Matrix3x2.CreateRotation(parent.Angle.DegreesToRadians()), out var r);
				Matrix3x2.Invert(Matrix3x2.CreateTranslation(parent.Position), out var t);

				inverseParent *= t;
				inverseParent *= r;
				inverseParent *= s;
			}

			return inverseParent;
		}

		private void UpdateSelfAndChildren()
		{
			UpdateGlobalMatrix();

			for(int i = 0; i < childrenUIDs.Count; i++)
			{
				var child = Get(childrenUIDs[i]);
				child?.UpdateSelfAndChildren();
			}
		}
		private void UpdateGlobalMatrix()
		{
			global = LocalToGlobal(LocalScale, LocalAngle, LocalPosition);
		}

		internal static float GetAngle(Matrix3x2 matrix)
		{
			return MathF.Atan2(matrix.M12, matrix.M11).RadiansToDegrees();
		}
		internal static Vector2 GetPosition(Matrix3x2 matrix)
		{
			return new(matrix.M31, matrix.M32);
		}
		internal static float GetScale(Matrix3x2 matrix)
		{
			return MathF.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12);
		}

		internal static Thing Get(string uid, int depth = 1)
		{
			return Get<Thing>(uid, depth);
		}
		internal static T Get<T>(string uid, int depth = 1) where T : Thing
		{
			if(uid == null || objs.ContainsKey(uid) == false)
				return default;

			if(objs[uid] is not T)
			{
				Console.LogError(depth + 1, $"The {nameof(Thing)} with UID '{uid}' exists but is not a {typeof(T).Name} - it is a {objs[uid].GetType().Name}");
				return default;
			}

			return (T)objs[uid];
		}
		#endregion

	}
}
