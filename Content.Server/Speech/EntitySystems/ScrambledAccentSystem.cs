// SPDX-FileCopyrightText: 2022 Jessica M <jessica@jessicamaybe.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 DEATHB4DEFEAT <77995199+DEATHB4DEFEAT@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using System.Linq;
using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Robust.Shared.Random;

namespace Content.Server.Speech.EntitySystems
{
    public sealed class ScrambledAccentSystem : EntitySystem
    {
        private static readonly Regex RegexLoneI = new(@"(?<=\ )i(?=[\ \.\?]|$)");

        [Dependency] private readonly IRobustRandom _random = default!;

        public override void Initialize()
        {
            SubscribeLocalEvent<ScrambledAccentComponent, AccentGetEvent>(OnAccent);
        }

        public string Accentuate(string message)
        {
            var words = message.ToLower().Split();

            if (words.Length < 2)
            {
                var pick = _random.Next(1, 8);
                // If they try to weasel out of it by saying one word at a time we give them this.
                return Loc.GetString($"accent-scrambled-words-{pick}");
            }

            // Scramble the words
            var scrambled = words.OrderBy(x => _random.Next()).ToArray();

            var msg = string.Join(" ", scrambled);

            // First letter should be capital
            msg = msg[0].ToString().ToUpper() + msg.Remove(0, 1);

            // Capitalize lone i's
            msg = RegexLoneI.Replace(msg, "I");
            return msg;
        }

        private void OnAccent(EntityUid uid, ScrambledAccentComponent component, AccentGetEvent args)
        {
            args.Message = Accentuate(args.Message);
        }
    }
}
