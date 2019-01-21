module GameCore.ImGui.GameRunner

open GameCore.GameModel
open GameCore.ImGui.GameLoop

/// Entry point to start the game. Takes a config and two 
/// methods: one for advancing the model and one to get a view.
let runImGuiGame config (advanceModel : RunState -> 'T option -> 'T option) getView getUI =
    use loop = new ImGuiGameLoop<'T> (config, advanceModel, getView, getUI)
    loop.Run ()
