module GameCore.ImGui.GameRunner

open GameCore.ImGui.GameLoop

/// Entry point to start the game. Takes a config and two 
/// methods: one for advancing the model and one to get a view.
let runImGuiGame config advanceModel getView startUIModel getUI =
    use loop = new ImGuiGameLoop<'T, 'TU> (config, advanceModel, getView, startUIModel, getUI)
    loop.Run ()
