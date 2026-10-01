using Content.Goobstation.Shared.StationRadio.Components;
using Content.Goobstation.Shared.StationRadio.Events;
using Content.Omu.Common.CCVar;
using Content.Shared.Interaction;
using Content.Shared.Power;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;

namespace Content.Goobstation.Shared.StationRadio.Systems;

public sealed class StationRadioReceiverSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedPowerReceiverSystem _power = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StationRadioReceiverComponent, StationRadioMediaPlayedEvent>(OnMediaPlayed);
        SubscribeLocalEvent<StationRadioReceiverComponent, StationRadioMediaStoppedEvent>(OnMediaStopped);
        SubscribeLocalEvent<StationRadioReceiverComponent, ActivateInWorldEvent>(OnRadioToggle);
        SubscribeLocalEvent<StationRadioReceiverComponent, PowerChangedEvent>(OnPowerChanged);
    }

    private void OnPowerChanged(EntityUid uid, StationRadioReceiverComponent comp, PowerChangedEvent args)
    {
        if(comp.SoundEntity != null && args.Powered)
            SetAdjustedGain(comp.SoundEntity, comp.Active ? comp.DefaultParams.Volume : 0f);
        else if(comp.SoundEntity != null)
            SetAdjustedGain(comp.SoundEntity, 0);
    }

    private void OnRadioToggle(EntityUid uid, StationRadioReceiverComponent comp, ActivateInWorldEvent args)
    {
        comp.Active = !comp.Active;
        if (comp.SoundEntity != null && _power.IsPowered(uid))
            SetAdjustedGain(comp.SoundEntity, comp.Active ? comp.DefaultParams.Volume : 0f);
    }

    private void OnMediaPlayed(EntityUid uid, StationRadioReceiverComponent comp, StationRadioMediaPlayedEvent args)
    {
        var audio = _audio.PlayPredicted(args.MediaPlayed, uid, uid, comp.DefaultParams);
        if (audio != null && _power.IsPowered(uid) && comp.Active)
            comp.SoundEntity = audio.Value.Entity;
        else if (audio != null && !_power.IsPowered(uid) || !comp.Active && audio != null)
        {
            comp.SoundEntity = audio.Value.Entity;
            SetAdjustedGain(comp.SoundEntity, 0);
        }
    }

    private void OnMediaStopped(EntityUid uid, StationRadioReceiverComponent comp, StationRadioMediaStoppedEvent args)
    {
        if (comp.SoundEntity == null)
            return;

        comp.SoundEntity = _audio.Stop(comp.SoundEntity);
    }

    private void SetAdjustedGain(EntityUid? soundEntity, float baseGain)
    {
        // var clientRadioVolume = _cfg.GetCVar(OmuCVars.RadioMusicVolume);
        // _audio.SetGain(soundEntity, baseGain * clientRadioVolume);
        _audio.SetGain(soundEntity, baseGain);
    }
}
