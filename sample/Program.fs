
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open GameCore.GameModel
open GameCore.GameRunner

[<EntryPoint>]
let main _ =
    
    let config = {
        clearColour = Some (new Color (50, 50, 50))
        resolution = Windowed (800, 600)
        assetsToLoad = []
        fpsFont = None
    }

    let advanceModel runState _ = 
        if wasJustPressed Keys.Escape runState then None
        else Some ()
    let getView _ _ = []

    runGame config advanceModel getView

    0
