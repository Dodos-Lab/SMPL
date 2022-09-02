namespace SMPL.UI
{
	internal class TextButtonInstance : ButtonInstance
	{
		public bool IsHyperlink { get; set; }
		public string TextUID
		{
			get => textUID;
			set
			{
				textUID = value;

				var text = Get<TextInstance>(TextUID);
				if(text != null)
					text.ParentUID = UID;
			}
		}

		#region Backend
		private string textUID;

		[JsonConstructor]
		internal TextButtonInstance()
		{
			Init(null);
		}
		internal TextButtonInstance(string uid, string textUID) : base(uid)
		{
			Init(textUID);
		}
		private void Init(string textUID)
		{
			Size = new(250, 60);
			TextUID = textUID;

			var text = Get<TextInstance>(TextUID);
			if(text != null)
				text.ParentUID = UID;
		}

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden)
				return;

			var text = Get<TextInstance>(TextUID);

			if(text != null)
				text.UpdateGlobalText();

			if(IsDisabled == false && IsHyperlink && text != null)
			{
				var b = TextInstance.textInstance.GetLocalBounds();
				Size = new((b.Width + text.SymbolSize / 2f) * text.Scale, (text.SymbolSize + text.SymbolSize / 5f) * text.Scale);
			}
			base.OnDraw(renderTarget);

			if(text != null)
				text.Draw(renderTarget);
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();

			var text = Get<TextInstance>(TextUID);
			if(text != null)
			{
				text.ParentUID = null;
				text.Destroy(false);
			}
		}

		#endregion
	}
}
