module GameCore.UIElements.Functions

open GameCore.GameModel
open GameCore.UIElements.Model
open GameCore.UIElements.Helpers

let getElementEvents runState =
    function
    | Button config -> 
        if not <| contains runState.mouse.position (pointsFor config.position config.size)
        then None
        else
            match runState.mouse.pressed with
            | true, _ | _, true -> Some Pressed | _ -> Some Hover
    | _ -> None

let private getButtonView runState config =
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

        let _, _, _, height = rect
        let padding = height / 4
        let textRect = contract padding rect
        yield Text (config.fontAsset, config.text, textRect, Centre, colours.text)
    ]

let private getLabelView _ config =
    let rect = rectFor config.position config.size
    let colours = config.colours
    [
        match colours.border with
        | None ->
            yield Colour (rect, colours.background)
        | Some (width, borderColour) ->
            yield Colour (rect, borderColour)
            let inner = contract width rect
            yield Colour (inner, colours.background)

        let _, _, _, height = rect
        let padding = height / (2 + List.length config.text + 1)
        let textRect = contract padding rect

        yield Paragraph (config.fontAsset, config.text, textRect, Centre, colours.text)
    ]

let getElementView runState =
    function
    | Button config ->
        getButtonView runState config
    | Label config ->
        getLabelView runState config