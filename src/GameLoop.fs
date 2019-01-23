module GameCore.ImGui.GameLoop

open GameCore.GameLoop
open ImGuiNET.Xna.Renderer

type ImGuiGameLoop<'TModel, 'TUIModel> (config, updateModel, getView, startUIModel, getUI)
    as this = 
    inherit GameLoop<'TModel> (config, updateModel, getView)

    let mutable imGuiRenderer = Unchecked.defaultof<ImGuiRenderer>
    let mutable uiModel: 'TUIModel = startUIModel

    do
        this.IsMouseVisible <- config.mouseVisible

    override __.Initialize() = 

        imGuiRenderer <- new ImGuiRenderer(this)
        imGuiRenderer.RebuildFontAtlas ()

        base.Initialize ()

    override __.Draw (gameTime) = 
        base.Draw (gameTime)

        match base.CurrentModel with
        | Some m -> 
            imGuiRenderer.BeforeLayout(gameTime);
            uiModel <- getUI uiModel m
            imGuiRenderer.AfterLayout();
        | _ -> ()