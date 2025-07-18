// SPDX-FileCopyrightText: 2025 portfiend
//
// SPDX-License-Identifier: MPL-2.0

using System.Linq;
using Content.Shared.CCVar;
using Content.Shared.Clothing.Loadouts.Prototypes;
using Content.Shared.Clothing.Loadouts.Systems;
using Content.Shared.Guidebook;
using Content.Shared.Preferences;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;

namespace Content.Client._DEN.Lobby.UI.Loadouts;

[GenerateTypedNameReferences]
public sealed partial class LoadoutsTab : BoxContainer
{
    [Dependency] private readonly IConfigurationManager _configuration = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    /// <summary>
    ///     Fired when a loadout preference changes, prompting the character editor to update the profile.
    /// </summary>
    public event Action<LoadoutPreference>? OnPreferenceChanged;

    /// <summary>
    ///     Fired when requesting to remove unusable loadouts.
    /// </summary>
    public event Action<List<LoadoutPrototype>>? OnRemoveUnusableLoadouts;

    /// <summary>
    ///     Fired when this tab would like to open a guidebook entry.
    /// </summary>
    public event Action<List<ProtoId<GuideEntryPrototype>>>? OnOpenGuidebook;

    private int MaxPoints => _configuration.GetCVar(CCVars.GameLoadoutsPoints);
    private LoadoutPreference? CurrentlyCustomizing => CustomizationPanel.Preference;

    public LoadoutsTab()
    {
        IoCManager.InjectDependencies(this);
        RobustXamlLoader.Load(this);

        ItemListPanel.OnPointsUpdaated += UpdatePointsDisplay;
        ItemListPanel.OnCustomizeToggled += ToggleCustomizePreference;
        ItemListPanel.OnRemoveUnusableLoadouts += RemoveUnusableLoadouts;
        ItemListPanel.OnOpenGuidebook += OpenGuidebook;
        ItemListPanel.OnPreferenceChanged += UpdateLoadoutPreference;
        ItemListPanel.OnPreferenceChanged += p =>
        {
            // If we're equipping a new loadout
            if (p.Selected)
            {
                SetCustomizePreference(p);
                return;
            }

            // If we're unequipping the loadout that we're currently customizing
            if (CurrentlyCustomizing?.LoadoutName == p.LoadoutName)
                SetCustomizePreference(null);
        };

        CategoryPanel.OnCategorySelected += ItemListPanel.SetVisibleCategory;
        CategoryPanel.SelectLoadoutCategory(null, true);

        CustomizationPanel.OnCustomizationSaved += UpdateLoadoutPreference;
    }

    /// <summary>
    ///     Syncs the loadout tab's UI to a new profile.
    /// </summary>
    /// <remarks>
    ///     This gets updated every time a loadout changes, or when the profile updates elsewhere in
    ///     the character editor (species, jobs...)
    /// </remarks>
    /// <param name="profile">The profile to sync.</param>
    public void SetProfile(HumanoidCharacterProfile? profile)
    {
        ItemListPanel.SetProfile(profile);
        CategoryPanel.UpdateLoadoutCounts(profile);

        // If the customization panel is open, then we need to sync the panel's Preference
        // with the preference stored in the new profile.
        if (CurrentlyCustomizing?.LoadoutName != null
            && profile?.LoadoutPreferences != null
            && profile.LoadoutPreferences.Count > 0)
        {
            var newPref = profile.LoadoutPreferences
                .FirstOrDefault(p => p.LoadoutName == CurrentlyCustomizing.LoadoutName);

            if (newPref != null)
                SetCustomizePreference(newPref);
        }
    }

    /// <summary>
    ///     Sets the character preview dummy the loadout tab will use.
    /// </summary>
    /// <remarks>
    ///     This is used for checking is a loadout can be worn by the current character, as
    ///     the dummy (ideally) should have the same inventory slots as the player species.
    /// </remarks>
    /// <param name="dummy">The dummy entity to use.</param>
    public void SetCharacterDummy(EntityUid? dummy)
    {
        ItemListPanel.SetCharacterDummy(dummy);
    }

    private void SetCustomizePreference(LoadoutPreference? preference)
    {
        CustomizationPanel.Preference = preference;
        EntityUid? previewSprite = null;

        if (preference != null
            && _prototype.TryIndex<LoadoutPrototype>(preference.LoadoutName, out var loadout))
            previewSprite = ItemListPanel.GetPreviewEntity(loadout);

        CustomizationPanel.SetPreviewSprite(previewSprite);
    }

    private void ToggleCustomizePreference(LoadoutPreference preference)
    {
        var newPreference = CurrentlyCustomizing?.LoadoutName == preference.LoadoutName
            ? null
            : preference;

        SetCustomizePreference(newPreference);
    }

    /// <summary>
    ///     Deletes the UI's contents and remakes it from scratch.
    /// </summary>
    /// <remarks>
    ///     This is called when a loadout prototype is modified while in-game.
    /// </remarks>
    public void Reset()
    {
        // TODO: This is bugged, can't figure out why. The UI will successfully create all of the
        // item buttons again, but they won't actually display for some unknown reason.
        SetCustomizePreference(null);
        ItemListPanel.PopulateLoadouts(reset: true);
    }

    private void UpdateLoadoutPreference(LoadoutPreference preference)
    {
        OnPreferenceChanged?.Invoke(preference);
    }

    private void RemoveUnusableLoadouts()
    {
        var unusable = ItemListPanel.UnusableLoadouts;
        OnRemoveUnusableLoadouts?.Invoke(unusable);
    }

    private void UpdatePointsDisplay(int points)
    {
        PointsLabel.Text = Loc.GetString("humanoid-profile-editor-loadouts-points-label",
            ("points", points),
            ("max", MaxPoints));
        PointsBar.MaxValue = MaxPoints;
        PointsBar.Value = points;
    }

    private void OpenGuidebook(string guideEntry)
    {
        if (!_prototype.TryIndex<GuideEntryPrototype>(guideEntry, out var entry))
            return;

        var entries = new List<ProtoId<GuideEntryPrototype>>() { entry.ID };
        OnOpenGuidebook?.Invoke(entries);
    }

    /// <summary>
    ///     Gets the category name for a given category ID.
    /// </summary>
    /// <remarks>
    ///     This is public static, because both the item list and the category panel use it.
    /// </remarks>
    /// <param name="categoryId">The category ID.</param>
    /// <returns>A localized category name.</returns>
    public static string GetCategoryName(string categoryId)
        => Loc.GetString($"loadout-category-{categoryId}");
}
