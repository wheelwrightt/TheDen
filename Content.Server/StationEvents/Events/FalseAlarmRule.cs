// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 DEATHB4DEFEAT <77995199+DEATHB4DEFEAT@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 DEATHB4DEFEAT <zachcaffee@outlook.com>
// SPDX-FileCopyrightText: 2024 Mnemotechnican <69920617+Mnemotechnician@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 VMSolidus <evilexecutive@gmail.com>
// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using System.Linq;
using Content.Server.StationEvents.Components;
using Content.Shared.GameTicking.Components;
using JetBrains.Annotations;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Content.Server.Announcements.Systems;

namespace Content.Server.StationEvents.Events;

[UsedImplicitly]
public sealed class FalseAlarmRule : StationEventSystem<FalseAlarmRuleComponent>
{
    [Dependency] private readonly EventManagerSystem _event = default!;
    [Dependency] private readonly AnnouncerSystem _announcer = default!;

    protected override void Started(EntityUid uid, FalseAlarmRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        var allEv = _event.AllEvents()
            .Where(p => p.Value.StartAnnouncement)
            .Select(p => p.Key).ToList();
        var picked = RobustRandom.Pick(allEv);

        _announcer.SendAnnouncement(
            _announcer.GetAnnouncementId(picked.ID),
            Filter.Broadcast(),
            _announcer.GetEventLocaleString(_announcer.GetAnnouncementId(picked.ID)),
            null,
            Color.Gold,
            null, null,
            //TODO This isn't a good solution, but I can't think of something better
            ("data", Loc.GetString($"random-sentience-event-data-{RobustRandom.Next(1, 6)}"))
        );
    }
}
