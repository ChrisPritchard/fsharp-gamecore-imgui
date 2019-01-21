open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open GameCore.GameModel
open GameCore.GameRunner

open GameCore.UIElements.Button

type SampleModel = {
    button1: Button
    label1: Button
}

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

    let startModel = {
        button1 = { 
            destRect = 20, 20, 150, 40
            text = ["sample button"]
            fontAsset = "connection"
            fontSize = 16
            idleColours = { background = Color.Black; border = Some (1, Color.DarkGray); text = Color.White }
            hoverColours = Some { background = Color.White; border = Some (4, Color.Black); text = Color.Black }
            pressedColours = Some { background = Color.Gray; border = Some (4, Color.Black); text = Color.White }
            state = []
        }
        label1 = {
            destRect = 190, 20, 300, 80
            text = ["this is a centred label";"it uses a button with hover"]
            fontAsset = "connection"
            fontSize = 16
            idleColours = { background = Color.DarkBlue; border = Some (2, Color.Blue); text = Color.Gray }
            hoverColours = Some { background = Color.DarkBlue; border = Some (2, Color.Blue); text = Color.White }
            pressedColours = None
            state = []
        }
    }

    let advanceModel runState model = 
        if GameCore.GameModel.wasJustPressed Keys.Escape runState then None
        else
            match model with
            | None -> Some startModel
            | Some m -> 
                Some { 
                    m with 
                        button1 = updateButton runState m.button1
                        label1 = updateButton runState m.label1 }

    let getView runState model = 
        [
            yield! getButtonView model.button1
            yield Text ("connection", sprintf "%A" model.button1.state, (20, 70), 16, TopLeft, Color.White)

            yield! getButtonView model.label1
            yield Text ("connection", sprintf "%A" model.label1.state, (190, 110), 16, TopLeft, Color.White)

            let isPressed = isMousePressed (true, false) runState
            let mx, my = runState.mouse.position 
            yield Colour ((mx, my, 5, 5), (if isPressed then Color.Red else Color.Yellow))
        ]

    runGame config advanceModel getView

    0
