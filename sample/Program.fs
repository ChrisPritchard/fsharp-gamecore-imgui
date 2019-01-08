open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open GameCore.GameModel
open GameCore.GameRunner

open GameCore.UI

[<EntryPoint>]
let main _ =
    
    let config = {
        clearColour = Some (new Color (50, 50, 50))
        resolution = Windowed (800, 600)
        assetsToLoad = [
            Font ("connection", "./connection")
        ]
        fpsFont = None
    }

    let startModel = [
        Button { 
            position = 20,20
            size = 200, 50
            text = "hello world"
            textAsset = "connection"
            textScale = 0.4
            borderWidth = 2
            idleColours = { background = Color.Black; border = None; text = Color.White }
            hoverColours = Some { background = Color.White; border = Some Color.Black; text = Color.Black }
            pressedColours = { background = Color.Gray; border = Some Color.Black; text = Color.White }
        }
    ]

    let advanceModel runState model = 
        if wasJustPressed Keys.Escape runState then None
        else
            match model with
            | None -> Some startModel
            | _ -> model

    let getView runState model = List.collect (GameCore.UI.getView runState) model

    runGame config advanceModel getView

    0
