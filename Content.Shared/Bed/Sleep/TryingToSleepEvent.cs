// SPDX-FileCopyrightText: 2022 Errant <35878406+dmnct@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

namespace Content.Shared.Bed.Sleep;

/// <summary>
///     Raised by an entity about to fall asleep.
///     Set Cancelled to true on event handling to interrupt
/// </summary>
[ByRefEvent]
public record struct TryingToSleepEvent(EntityUid uid, bool Cancelled = false);
