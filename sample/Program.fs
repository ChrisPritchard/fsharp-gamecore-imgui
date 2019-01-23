open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open GameCore.GameModel
open ImGuiNET
open GameCore.ImGui.GameRunner
open GameCore.ImGui.Helpers

[<EntryPoint>]
let main _ =
    
    let config = {
        clearColour = Some (new Color (50, 50, 50))
        resolution = Windowed (800, 600)
        assetsToLoad = []
        fpsFont = None
        mouseVisible = true
    }
    
    let advanceModel runState _ model = 
        if GameCore.GameModel.wasJustPressed Keys.Escape runState then None
        else
            match model with
            | None -> Some ()
            | _ -> model

    let getView _ _ = [ ]

    let startModel = { Button1 = false; Button2 = false; Text = "test" }

    let getUI uiModel _ =
        render uiModel ui
        //ImGui.SliderFloat ("float", ref f, 0.0f, 1.0f, string.Empty, 1.f);
        //ImGui.ColorEdit3("clear color", ref clear_color);
        //if (ImGui.Button("Test Window")) show_test_window = !show_test_window;
        //if (ImGui.Button("Another Window")) show_another_window = !show_another_window;
        //ImGui.Text(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000f / ImGui.GetIO().Framerate, ImGui.GetIO().Framerate));

        //ImGui.InputText("Text input", _textBuffer, 100);

    runImGuiGame config advanceModel getView startModel getUI

    0
