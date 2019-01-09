module GameCore.UIElements

open Microsoft.Xna.Framework
open GameCore.GameModel

type UIColourSet = {
    text: Color
    background: Color
    border: (int * Color) option
}

type ButtonConfig = {
    position: int * int
    size: int * int
    text: string
    textAsset: string
    textScale: float
    idleColours: UIColourSet
    hoverColours: UIColourSet option
    pressedColours: UIColourSet
}

type UIElement =
    | Button of ButtonConfig

type UIEvent = | Pressed | Hover

let private pointsFor (x, y) (w, h) = 
    x, y, x + w, y + h

let contains (x, y) (rx, ry, rmx, rmy) = 
    x >= rx && y >= ry && x <= rmx && y <= rmy

let private rectFor (x, y) (w, h) = 
    x, y, w, h

let contract n (rx, ry, rw, rh) =
    rx + n, ry + n, rw - 2*n, rh - 2*n

let centre (rx, ry, rw, rh) =
    rx + rw/2, ry + rh/2

let getElementEvents runState =
    function
    | Button config -> 
        if not <| contains runState.mouse.position (pointsFor config.position config.size)
        then None
        else
            match runState.mouse.pressed with
            | true, _ | _, true -> Some Pressed | _ -> Some Hover

let getElementView runState =
    function
    | Button config ->
        let events = getElementEvents runState (Button config)
        let colours = 
            match events, config.hoverColours with
            | None, _ | Some Hover, None -> config.idleColours
            | Some Hover, Some hoverColours -> hoverColours
            | Some Pressed, _ -> config.pressedColours
        let rect = rectFor config.position config.size
        [
            match colours.border with
            | None ->
                yield Colour (rect, colours.background)
            | Some (width, borderColour) ->
                yield Colour (rect, borderColour)
                let inner = contract width rect
                yield Colour (inner, colours.background)
            yield Text (config.textAsset, config.text, centre rect, Centre, config.textScale, colours.text)
        ]