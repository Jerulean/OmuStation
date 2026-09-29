using System.Runtime.CompilerServices;

namespace Content.Client.Administration.UI.CustomControls
{
    public sealed class SafetyCommandButton : CommandButton
    {
        private TimeSpan? DeleteResetOn { get; set; }
        private string? OriginalText { get; set; }

        public SafetyCommandButton()
        {
            OnPressed -= Execute;
            OnPressed += SafetyPress;
        }
        protected void SafetyPress(ButtonEventArgs obj)
        {
            OriginalText = Text;
            Text = Loc.GetString("administration-ui-round-tab-confirm");
            ModulateSelfOverride = Color.Red;

            OnPressed += Execute;
            OnPressed -= SafetyPress;
        }

        // Should be the action of the superclass,
        // firing after the safety.
        protected override void Execute(ButtonEventArgs obj)
        {
            OnPressed -= Execute;
            OnPressed += SafetyPress;

            Text = OriginalText;
            ModulateSelfOverride = null;
            base.Execute(obj);
        }
    }
}


