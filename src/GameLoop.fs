module GameCore.Imgui.GameLoop

open GameCore.GameLoop
open imgui_renderer

type ImguiGameLoop<'TModel> (config, updateModel, getView, getUI)
    as this = 
    inherit GameLoop<'TModel> (config, updateModel, getView)

    let mutable imGuiRenderer = Unchecked.defaultof<ImGuiRenderer>

    override __.Initialize() = 

        imGuiRenderer = new ImGuiRenderer(this) |> ignore
        imGuiRenderer.RebuildFontAtlas ()

        base.Initialize ()

    override __.Draw (gameTime) = 
        base.Draw (gameTime)

        imGuiRenderer.BeforeLayout(gameTime);
        getUI this.currentModel
        imGuiRenderer.AfterLayout();