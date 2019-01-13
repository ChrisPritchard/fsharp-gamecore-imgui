module GameCore.UIElements.Button

open GameCore.GameModel
open Common

/// A button is a panel with a centred piece of text that 
/// responds to hover and pressed events. it has optional hover 
/// and pressed colours
type Button = {
    destRect: int * int * int * int
    text: string list
    fontAsset: string
    fontSize: int
    idleColours: ColourSet
    hoverColours: ColourSet option
    pressedColours: ColourSet option
    state: UIState list
}

/// Checks for and updates the ui states affecting the button (e.g. pressed, hover, idle)
let updateButton (runState: RunState) (button: Button) = 
    let isPressed = List.contains Pressed button.state
    if not <| contains runState.mouse.position (pointsFor button.destRect) then
        let newState = if isPressed then [JustReleased;Idle] else [Idle]
        { button with state = newState }
    else
        let pressed = isMousePressed (true, false) runState
        let newState = 
            match pressed, isPressed with
            | true, true -> [Hover;Pressed]
            | true, false -> [Hover;Pressed;JustPressed]
            | false, true -> [Hover;JustReleased]
            | false, false -> [Hover]
        { button with state = newState }

/// Returns a list of view elements representing the current state of the button
let getButtonView button =
    let colours = 
        if List.contains Pressed button.state then 
            button.pressedColours 
            |> Option.orElse button.hoverColours 
            |> Option.defaultValue button.idleColours
        else if List.contains Hover button.state then 
            button.hoverColours 
            |> Option.defaultValue button.idleColours
        else 
            button.idleColours
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

/// Checks if the button's state contains the JustPressed ui state
let wasJustPressed button =
    List.contains JustPressed button.state

/// Checks if the button's state contains the JustReleased ui state
let wasJustReleased button =
    List.contains JustReleased button.state