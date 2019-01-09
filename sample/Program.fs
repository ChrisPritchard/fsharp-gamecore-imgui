open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open GameCore.GameModel
open GameCore.GameRunner

open GameCore.UIElements.Model
open GameCore.UIElements.Functions

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
            position = 20, 20
            size = 200, 50
            text = "sample button"
            fontAsset = "connection"
            idleColours = { background = Color.Black; border = None; text = Color.White }
            hoverColours = Some { background = Color.White; border = Some (2, Color.Black); text = Color.Black }
            pressedColours = { background = Color.Gray; border = Some (2, Color.Black); text = Color.White }
        }
        Label {
            position = 240, 20
            size = 200, 100
            text = ["this is some sample text"; "that runs over three lines."; "labels have no events"]
            fontAsset = "connection"
            colours = { background = Color.DarkBlue; border = Some (2, Color.Blue); text = Color.White }
        }
    ]

    let advanceModel runState model = 
        if wasJustPressed Keys.Escape runState then None
        else
            match model with
            | None -> Some startModel
            | _ -> model

    let getView runState model = 
        List.collect (getElementView runState) model
        @ 
        [
            let isPressed = isMousePressed (true, false) runState
            let mx, my = runState.mouse.position 
            yield Colour ((mx, my, 5, 5), (if isPressed then Color.Red else Color.Yellow))
        ]

    runGame config advanceModel getView

    0
