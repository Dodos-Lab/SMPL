namespace SMPL.GUI
{
	internal class TextButtonInstance : ButtonInstance
	{
		public Thing.GUI.TextDetails TextDetails { get; } = new();

		public bool IsHyperlink { get; set; }

		#region Backend
		[JsonConstructor]
		internal TextButtonInstance() => Init();
		internal TextButtonInstance(string uid) : base(uid) => Init();
		private void Init()
		{
			Size = new(250, 60);
		}

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden)
				return;

			TextDetails.UpdateGlobalText(Scale);
			var text = TextInstance.textInstance;

			if(IsDisabled == false && IsHyperlink)
			{
				var b = TextInstance.textInstance.GetLocalBounds();
				var sc = new Vector2(text.Scale.X, text.Scale.Y);
				Size = new Vector2(b.Width + text.CharacterSize / 2f, text.CharacterSize + text.CharacterSize / 5f) * sc;
			}
			base.OnDraw(renderTarget);

			renderTarget.Draw(text);
		}
		#endregion
	}
}
