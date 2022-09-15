namespace SMPL
{
	internal abstract class ThingInstance
	{
		public bool IsDisabled
		{
			get
			{
				var parent = Get_(ParentUID);
				return parent == null ? isDisabled : isDisabled || parent.IsDisabled;
			}
		}
		public bool IsDisabledSelf
		{
			get => isDisabled;
			set => isDisabled = value;
		}

		public ReadOnlyCollection<string> Types
		{
			get
			{
				var result = new List<string>();
				var curType = GetType();

				Add(curType);
				while(curType.BaseType != null)
				{
					Add(curType.BaseType);
					curType = curType.BaseType;
				}
				return result.AsReadOnly();

				void Add(Type type)
				{
					if(type.Name == nameof(Object))
						return;

					result.Add(type.Name.Replace("Instance", ""));
				}
			}
		}
		public string UID
		{
			get => uid;
			set
			{
				var objs = Scene.CurrentScene.objs;

				if(value == uid || objs.ContainsKey(value))
					return;

				if(uid != null)
					objs.Remove(uid);

				oldUID = uid;
				uid = value;
				objs[uid] = this;

				var parent = Get_(parentUID);
				if(parent != null)
				{
					parent.childrenUIDs.Remove(oldUID);
					parent.childrenUIDs.Add(uid);
				}

				for(int i = 0; i < childrenUIDs.Count; i++)
				{
					var child = Get_(childrenUIDs[i]);
					if(child != null)
						child.parentUID = uid;
				}
			}
		}
		[JsonIgnore]
		public string OldUID => oldUID;
		public int NumericUID => numUID;
		public List<string> Tags { get; } = new();

		public string ParentUID
		{
			get => parentUID;
			set
			{
				if(parentUID == value || uid == value)
					return;

				if(childrenUIDs.Contains(value))
				{
					var child = Get_(value);
					child.ParentUID = null;
				}

				var parent = Get_(parentUID);

				if(parent != null)
					parent.childrenUIDs.Remove(uid);

				var prevPos = Position;
				var prevAng = Angle;
				var prevSc = Scale;

				parOldUID = parentUID;
				parentUID = value;

				Position = prevPos;
				Angle = prevAng;
				Scale = prevSc;

				var newParent = Get_(parentUID);
				if(newParent != null && newParent.childrenUIDs.Contains(uid) == false)
					newParent.childrenUIDs.Add(uid);
			}
		}
		public string ParentOldUID => parOldUID;
		[JsonIgnore]
		public ReadOnlyCollection<string> ChildrenUIDs => childrenUIDs.AsReadOnly();

		[JsonIgnore]
		public float Age => age.ElapsedTime.AsSeconds();

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
		public virtual float LocalAngle
		{
			get => localAng.Wrap(360);
			set { localAng = value; UpdateSelfAndChildren(); }
		}
		[JsonIgnore]
		public Vector2 LocalDirection
		{
			get => Vector2.Normalize(LocalAngle.ToDirection());
			set => LocalAngle = Vector2.Normalize(value).ToAngle();
		}

		[JsonIgnore]
		public Vector2 Position
		{
			get => GetPosition(global);
			set => LocalPosition = LocalPositionFromParent(value);
		}
		[JsonIgnore]
		public float Scale
		{
			get => GetScale(global);
			set => LocalScale = GetScale(GlobalToLocal(value, Angle, Position));
		}
		[JsonIgnore]
		public float Angle
		{
			get => GetAngle(global).Wrap(360);
			set => LocalAngle = GetAngle(GlobalToLocal(Scale, value, Position));
		}
		[JsonIgnore]
		public Vector2 Direction
		{
			get => Vector2.Normalize(Angle.ToDirection());
			set => Angle = Vector2.Normalize(value).ToAngle();
		}

		public Hitbox Hitbox
		{
			get
			{
				hitbox.TransformLocalLines(uid);
				return hitbox;
			}
		}
		[JsonIgnore]
		public virtual Hitbox BoundingBox
		{
			get
			{
				var sz = DEFAULT_BB_SIZE;
				var tl = new Vector2(-sz, -sz);
				var tr = new Vector2(sz, -sz);
				var br = new Vector2(sz, sz);
				var bl = new Vector2(-sz, sz);

				bb.Lines.Clear();
				bb.LocalLines.Clear();
				bb.LocalLines.Add(new(tl, tr));
				bb.LocalLines.Add(new(tr, br));
				bb.LocalLines.Add(new(br, bl));
				bb.LocalLines.Add(new(bl, tl));
				bb.TransformLocalLines(uid);
				return bb;
			}
		}

		public Vector2 LocalPositionFromParent(Vector2 position)
		{
			return GetPosition(GlobalToLocal(Scale, Angle, position));
		}
		public Vector2 PositionFromParent(Vector2 localPosition)
		{
			return GetPosition(LocalToGlobal(LocalScale, LocalAngle, localPosition));
		}
		public Vector2 LocalPositionFromSelf(Vector2 position)
		{
			Matrix3x2.Invert(global, out var m);
			return Vector2.Transform(position, m);
		}
		public Vector2 PositionFromSelf(Vector2 localPosition)
		{
			return Vector2.Transform(localPosition, global);
		}

		public void Destroy(bool includeChildren)
		{
			OnDestroy();

			var children = new List<string>(childrenUIDs);
			for(int i = 0; i < children.Count; i++)
			{
				var child = Get_(children[i]);
				if(child == null)
					continue;

				// prevents hard jump to world coordinates
				child.ParentUID = null;

				if(includeChildren)
					child.Destroy(includeChildren);
			}
			Scene.CurrentScene.objs.Remove(uid);

			// prevents hard jump to world coordinates
			for(int i = 0; i < children.Count; i++)
			{
				var child = Get_(children[i]);
				if(child == null)
					continue;

				child.ParentUID = uid;
			}
		}
		public override string ToString()
		{
			return $"{GetType().GetPrettyName()}{{{uid}}}";
		}

		#region Backend
		private bool isDisabled;
		private const float DEFAULT_BB_SIZE = 50f;
		private readonly Clock age = new();
		protected readonly Hitbox hitbox = new(), bb = new();
		[JsonProperty]
		internal readonly List<string> childrenUIDs = new();
		internal string parentUID, oldUID, parOldUID;
		private Vector2 localPos;
		private float localAng, localSc;
		private Matrix3x2 global;
		private string uid;
		[JsonProperty]
		private readonly int numUID;

		[JsonConstructor]
		internal ThingInstance() => Init();
		internal ThingInstance(string uid)
		{
			var objs = Scene.CurrentScene.objs;
			numUID = objs.Count; // before UID to be 0 based

			UID = Thing.FreeUID(uid);
			LocalScale = 1;

			Event.ThingCreate(UID);
		}
		private void Init()
		{
			Event.ThingCreate(UID);
		}

		internal virtual void OnDestroy() { }

		internal Matrix3x2 LocalToGlobal(float localScale, float localAngle, Vector2 localPosition)
		{
			var c = Matrix3x2.Identity;
			c *= Matrix3x2.CreateScale(localScale);
			c *= Matrix3x2.CreateRotation(localAngle.ToRadians());
			c *= Matrix3x2.CreateTranslation(localPosition);

			return c * GetParentMatrix();
		}
		internal Matrix3x2 GlobalToLocal(float scale, float angle, Vector2 position)
		{
			var c = Matrix3x2.Identity;
			c *= Matrix3x2.CreateScale(scale);
			c *= Matrix3x2.CreateRotation(angle.ToRadians());
			c *= Matrix3x2.CreateTranslation(position);

			return c * GetInverseParentMatrix();
		}
		internal Matrix3x2 GetParentMatrix()
		{
			var p = Matrix3x2.Identity;
			var parent = Get_(parentUID);
			if(parent != null)
			{
				p *= Matrix3x2.CreateScale(parent.Scale);
				p *= Matrix3x2.CreateRotation(parent.Angle.ToRadians());
				p *= Matrix3x2.CreateTranslation(parent.Position);
			}
			return p;
		}
		internal Matrix3x2 GetInverseParentMatrix()
		{
			var inverseParent = Matrix3x2.Identity;
			var parent = Get_(parentUID);
			if(parent != null)
			{
				Matrix3x2.Invert(Matrix3x2.CreateScale(parent.Scale), out var s);
				Matrix3x2.Invert(Matrix3x2.CreateRotation(parent.Angle.ToRadians()), out var r);
				Matrix3x2.Invert(Matrix3x2.CreateTranslation(parent.Position), out var t);

				inverseParent *= t;
				inverseParent *= r;
				inverseParent *= s;
			}

			return inverseParent;
		}

		internal void UpdateParency()
		{
			var objs = Scene.CurrentScene.objs;
			foreach(var kvp in objs)
			{
				if(kvp.Value.parentUID == uid && childrenUIDs.Contains(kvp.Key) == false)
				{
					kvp.Value.ParentUID = null;
					kvp.Value.ParentUID = uid;
				}
				else if(kvp.Value.childrenUIDs.Contains(uid))
				{
					ParentUID = null;
					ParentUID = kvp.Value.UID;
				}
			}
		}
		private void UpdateSelfAndChildren()
		{
			UpdateGlobalMatrix();

			for(int i = 0; i < childrenUIDs.Count; i++)
			{
				var child = Get_(childrenUIDs[i]);
				child?.UpdateSelfAndChildren();
			}
		}
		private void UpdateGlobalMatrix()
		{
			global = LocalToGlobal(LocalScale, LocalAngle, LocalPosition);
		}

		internal static float GetAngle(Matrix3x2 matrix)
		{
			return MathF.Atan2(matrix.M12, matrix.M11).ToDegrees();
		}
		internal static Vector2 GetPosition(Matrix3x2 matrix)
		{
			return new(matrix.M31, matrix.M32);
		}
		internal static float GetScale(Matrix3x2 matrix)
		{
			return MathF.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12);
		}

		internal static ThingInstance Get_(string uid)
		{
			return Get_<ThingInstance>(uid);
		}
		internal static T Get_<T>(string uid) where T : ThingInstance
		{
			var objs = Scene.CurrentScene.objs;
			return Thing.Exists(uid) == false || objs[uid] is not T ? default : (T)objs[uid];
		}
		internal static ThingInstance GetTryError(string uid)
		{
			var obj = Get_(uid);
			if(obj == null)
			{
				MissingError(uid);
				return default;
			}
			return obj;
		}
		internal static void MissingError(string uid)
		{
			Console.LogError(0, $"Thing '{uid}' does not exist.");
		}
		#endregion
	}
}
