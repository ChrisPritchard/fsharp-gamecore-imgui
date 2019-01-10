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
        yield Button { 
            destRect = 20, 20, 200, 50
            text = "sample button"
            fontAsset = "connection"
            idleColours = { background = Color.Black; border = None; text = Color.White }
            hoverColours = Some { background = Color.White; border = Some (2, Color.Black); text = Color.Black }
            pressedColours = { background = Color.Gray; border = Some (2, Color.Black); text = Color.White }
        }
        yield Label {
            destRect = 240, 20, 200, 100
            text = ["this is some sample text"; "that runs over three lines."; "labels have no events"]
            fontAsset = "connection"
            colours = { background = Color.DarkBlue; border = Some (2, Color.Blue); text = Color.White }
        }

        let labelColours = { background = Color.DarkBlue; border = Some (2, Color.Blue); text = Color.White }
        let innerLabel text = Label { destRect = 0,0,0,0; text = [text]; fontAsset = "connection"; colours = labelColours }
        yield Panel ({
            destRect = 20, 80, 200, 200
            background = None
            border = None
            padding = None
            alignment = Some (AlignVertically 0)
        }, [
            yield innerLabel "a panel   "
            yield innerLabel "with no   "
            yield innerLabel "background"
            yield innerLabel "or spacing"
        ])

        yield Panel ({
            destRect = 240, 200, 400, 50
            background = Some Color.DarkGray
            border = Some (4, Color.Gray)
            padding = Some 10
            alignment = Some (AlignHorizontally 10)
        }, [
            yield innerLabel "  a panel  "
            yield innerLabel "   with a  "
            yield innerLabel " background"
            yield innerLabel "and spacing"
        ])

        yield Panel ({
            destRect = 20, 320, 300, 250
            background = Some Color.DarkGray
            border = Some (4, Color.Gray)
            padding = Some 10
            alignment = Some (AlignHorizontally 10)
        }, [
            yield Panel ({
                destRect = 0, 0, 0, 0
                background = None
                border = None
                padding = None
                alignment = Some (AlignVertically 10)
            }, [
                yield innerLabel "label 1"
                yield innerLabel "label 2"
                yield innerLabel "label 3"
            ])
            yield Panel ({
                destRect = 0, 0, 0, 0
                background = None
                border = None
                padding = None
                alignment = Some (AlignVertically 10)
            }, [
                yield innerLabel "label 4"
                yield innerLabel "label 5"
                yield innerLabel "label 6"
            ])
            yield Panel ({
                destRect = 0, 0, 0, 0
                background = None
                border = None
                padding = None
                alignment = Some (AlignVertically 10)
            }, [
                yield innerLabel "label 7"
                yield innerLabel "label 8"
                yield innerLabel "label 9"
            ])
        ])

        let innerButton text = Button { 
            destRect = 0, 0, 0, 0
            text = text
            fontAsset = "connection"
            idleColours = { background = Color.Red; border = None; text = Color.White }
            hoverColours = Some { background = Color.White; border = Some (2, Color.Red); text = Color.Black }
            pressedColours = { background = Color.DarkRed; border = Some (2, Color.Red); text = Color.Gray }
        }
        yield Panel ({
            destRect = 340, 320, 300, 250
            background = Some Color.DarkGray
            border = Some (4, Color.Gray)
            padding = Some 10
            alignment = Some (AlignHorizontally 10)
        }, [
            yield Panel ({
                destRect = 0, 0, 0, 0
                background = None
                border = None
                padding = None
                alignment = Some (AlignVertically 10)
            }, [
                yield innerButton "button 1"
                yield innerButton "button 2"
                yield innerButton "button 3"
            ])
            yield Panel ({
                destRect = 0, 0, 0, 0
                background = None
                border = None
                padding = None
                alignment = Some (AlignVertically 10)
            }, [
                yield innerButton "button 4"
                yield innerButton "button 5"
                yield innerButton "button 6"
            ])
            yield Panel ({
                destRect = 0, 0, 0, 0
                background = None
                border = None
                padding = None
                alignment = Some (AlignVertically 10)
            }, [
                yield innerButton "button 7"
                yield innerButton "button 8"
                yield innerButton "button 9"
            ])
        ])
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
