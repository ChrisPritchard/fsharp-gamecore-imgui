module GameCore.Imgui.GameLoop

type ImguiGameLoop<'TModel> (config, updateModel, getView)
    as this = 
    inherit Game()

