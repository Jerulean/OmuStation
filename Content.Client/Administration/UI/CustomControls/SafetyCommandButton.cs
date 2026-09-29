namespace Content.Client.Administration.UI.CustomControls
{
    public sealed class SafetyCommandButton : CommandButton
    {
        private TimeSpan? DeleteResetOn { get; set; }
        private string? OriginalText { get; set; }

        protected override void Execute(ButtonEventArgs obj)
        {
            ModulateSelfOverride = Color.Red;
            base.Execute(obj);
        }
    }
}


