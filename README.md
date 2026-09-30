# Paper Dolls

Casual cozy game based on old-time paper dolls. This repository is the Unity project and the single source of truth for the project files and gameplay foundation.

## MVP foundation

The initial gameplay code is under `Assets/_Project/Scripts/`. It provides local content definitions, player inventory and outfit state, event scoring, a currency balance, JSON saving, and a small game-flow coordinator. It does not include UI, a character model, or production art; those are integration steps rather than assumptions embedded in the services.

### Data and services

- `ItemDefinition` and `EventDefinition` are ScriptableObjects for the local MVP catalog. Item definitions contain stable IDs, slots, tags, and optional visual references. Event definitions contain tag/slot scoring rules and a reward.
- `ContentCatalog` resolves definitions by stable ID and rejects missing, blank, or duplicate IDs.
- `PlayerSaveData` stores player-owned item IDs, equipped item IDs/slots, and balances. `SaveService` stores version 1 JSON at `Application.persistentDataPath/player-save.json`.
- `InventoryService`, `OutfitService`, `ScoringService`, and `EconomyService` are plain C# classes. Scoring is deterministic and returns a per-rule breakdown. Currency is identified by string ID, initially `GEMS`.
- `GameBootstrap` wires the services from a scene object. `GameFlowController` exposes the MVP flow; UI can start an event, equip/remove items, submit, display `LastResult`, and continue to event selection.

### Unity setup

1. Open this repository in Unity 6 (the project version is recorded in `ProjectSettings/ProjectVersion.txt`); its `Packages/` manifest and Unity project settings are already included.
2. Create `ItemDefinition` assets, fill in unique IDs, slots, and tags, and add them to a `ContentCatalog` asset.
3. Create an `EventDefinition` asset with tag/slot score rules and a non-empty reward currency ID; add it to the same catalog.
4. Add `GameBootstrap` to a scene GameObject, assign the catalog, and configure starter item IDs and initial currency.
5. Build UI views that call `GameBootstrap.Flow` and render item definitions and the score result. Assign visuals through the item icon/prefab references as needed.

On the first launch, starter ownership and starting currency are written to the save. Subsequent launches load the existing save; changing starter settings does not overwrite existing player data. Outfit changes, item acquisitions, and currency transactions are saved locally. Unsupported or corrupt save data raises an error instead of silently resetting progress.

### Scope and next steps

The current scoring model sums configured points for each matching item tag and each selected slot rule. It intentionally has no set bonuses, penalties by default, AI, networking, analytics, purchases, ads, or remote configuration. Google Sheets export and runtime JSON content are deferred until the catalog is large enough to benefit from bulk authoring; stable IDs provide the migration seam. JSON save data is not the content pipeline.

The included Unity project provides the project manifest, settings, and sample scene; the gameplay foundation still needs integration into the scene and UI. Validate catalog ID handling, equip/replace/remove behavior, scoring breakdowns, save/reload, and submitting a round exactly once in the Unity Editor.
