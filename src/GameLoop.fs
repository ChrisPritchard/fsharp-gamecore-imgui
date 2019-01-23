module GameCore.ImGui.GameLoop

open GameCore.GameLoop
open ImGuiNET.Xna.Renderer

type ImGuiGameLoop<'TModel> (config, updateModel, getView, getUI)
    as this = 
    inherit GameLoop<'TModel> (config, updateModel, getView)

    let mutable imGuiRenderer = Unchecked.defaultof<ImGuiRenderer>

    do
        this.IsMouseVisible <- config.mouseVisible

    override __.Initialize() = 

        imGuiRenderer <- new ImGuiRenderer(this)
        imGuiRenderer.RebuildFontAtlas ()

        base.Initialize ()

    override __.Draw (gameTime) = 
        base.Draw (gameTime)

        imGuiRenderer.BeforeLayout(gameTime);
        getUI ()
        imGuiRenderer.AfterLayout();