// SPDX-FileCopyrightText: 2024 brainfood1183 <113240905+brainfood1183@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Content.Shared.Random;
using Robust.Shared.Prototypes;

namespace Content.Shared.Plunger.Components
{
    /// <summary>
    /// Entity can interact with plungers.
    /// </summary>
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class PlungerUseComponent : Component
    {
        /// <summary>
        /// If true entity has been plungered.
        /// </summary>
        [DataField]
        [AutoNetworkedField]
        public bool Plunged;

        /// <summary>
        /// If true entity can interact with plunger.
        /// </summary>
        [DataField]
        [AutoNetworkedField]
        public bool NeedsPlunger = false;

        /// <summary>
        /// A weighted random entity prototype containing the different loot that rummaging can provide.
        /// </summary>
        [DataField]
        [AutoNetworkedField]
        public ProtoId<WeightedRandomEntityPrototype> PlungerLoot = "PlungerLoot";


        /// <summary>
        /// Sound played on rummage completion.
        /// </summary>
        [DataField]
        public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/Effects/Fluids/glug.ogg");
    }
}
