module GameCore.ImGui.GameLoop

open GameCore.GameLoop
open GameCore.ImGuiXnaRenderer

type ImGuiGameLoop<'TModel, 'TUIModel> (config, updateModel, getView, startUIModel, getUI)
    as this = 
    inherit GameLoop<'TModel> (config, (fun runState model -> updateModel runState this.CurrentUIModel model), getView)

    let mutable imGuiRenderer = Unchecked.defaultof<ImGuiRenderer>
    let mutable uiModel: 'TUIModel = startUIModel

    do
        this.IsMouseVisible <- config.mouseVisible

    member __.CurrentUIModel
        with get () = uiModel
    
    override __.Initialize() = 

        imGuiRenderer <- new ImGuiRenderer(this)
        imGuiRenderer.RebuildFontAtlas ()

        base.Initialize ()

    override __.Draw (gameTime) = 
        base.Draw (gameTime)

        match base.CurrentModel with
        | Some model -> 
            imGuiRenderer.BeforeLayout(gameTime);
            uiModel <- getUI uiModel model
            imGuiRenderer.AfterLayout();
        | _ -> ()