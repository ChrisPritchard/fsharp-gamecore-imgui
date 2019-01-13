module GameCore.UIElements.Button

open GameCore.GameModel
open Common

type ButtonState = Idle | Hover | JustPressed | Pressed | JustReleased

type Button = {
    destRect: int * int * int * int
    text: string list
    fontAsset: string
    fontSize: int
    idleColours: ColourSet
    hoverColours: ColourSet option
    pressedColours: ColourSet
    state: ButtonState list
}

let updateButton (runState: RunState) (button: Button) = 
    let isPressed = List.contains Pressed button.state
    if not <| contains runState.mouse.position (pointsFor button.destRect) then
        let newState = if isPressed then [JustReleased;Idle] else [Idle]
        { button with state = newState }
    else
        let pressed = isMousePressed (true, false) runState
        let newState = 
            match pressed, isPressed with
            | true, true -> [Pressed]
            | true, false -> [Pressed;JustPressed]
            | false, true -> [Hover;JustReleased]
            | false, false -> [Hover]
        { button with state = newState }

let getButtonView button =
    let colours = 
        if List.contains Pressed button.state then button.pressedColours
        else if List.contains Hover button.state then button.hoverColours |> Option.defaultValue button.idleColours
        else button.idleColours
    [
        match colours.border with
        | None ->
            yield Colour (button.destRect, colours.background)
        | Some (width, borderColour) ->
            yield Colour (button.destRect, borderColour)
            let inner = contract width button.destRect
            yield Colour (inner, colours.background)

        yield Paragraph (button.fontAsset, button.text, centre button.destRect, button.fontSize, Centre, colours.text)
    ]