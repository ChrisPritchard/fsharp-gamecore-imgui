module GameCore.UIElements.Functions

open GameCore.GameModel
open GameCore.UIElements.Model
open GameCore.UIElements.Helpers

let getElementEvents runState =
    function
    | Button config -> 
        if not <| contains runState.mouse.position (pointsFor config.destRect)
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
    [
        match colours.border with
        | None ->
            yield Colour (config.destRect, colours.background)
        | Some (width, borderColour) ->
            yield Colour (config.destRect, borderColour)
            let inner = contract width config.destRect
            yield Colour (inner, colours.background)

        let x, y, width, height = config.destRect
        let centre = x + (width / 2), y + (height / 2)
        yield Text (config.text.font, config.text.text.[0], centre, config.text.size, Centre, colours.text)
    ]

let private getLabelView _ (config: LabelConfig) =
    let colours = config.colours
    [
        match colours.border with
        | None ->
            yield Colour (config.destRect, colours.background)
        | Some (width, borderColour) ->
            yield Colour (config.destRect, borderColour)
            let inner = contract width config.destRect
            yield Colour (inner, colours.background)

        let x, y, _, height = config.destRect
        let padding = height / (2 + config.text.text.Length + 1)

        yield Paragraph (config.text.font, config.text.text, (x + padding, y + padding), config.text.size, TopLeft, colours.text)
    ]

let private getPanelView _ (config: PanelConfig) elements =
    let panel = [
        match config.border with
        | None -> ()
        | Some (_, borderColour) ->
            yield Colour (config.destRect, borderColour)
        match config.background, config.border with
        | None, _ -> ()
        | Some colour, Some (width, _) -> 
            let inner = contract width config.destRect
            yield Colour (inner, colour)
        | Some colour, None ->
            yield Colour (config.destRect, colour)
    ]

    let updateElementRects rectFor =
        List.mapi (fun i -> 
            function
            | Button config -> 
                Button { config with destRect = rectFor i }
            | Label config -> 
                Label { config with destRect = rectFor i }
            | Panel (config, elements) ->
                Panel ({ config with destRect = rectFor i }, elements))

    let children = List.length elements
    let x, y, w, h = 
        match config.padding with Some n -> contract n config.destRect | _ -> config.destRect

    let positioned = 
        match config.alignment with
        | None -> elements
        | Some (AlignHorizontally spacing) ->
            let elemWidth = (w - ((children-1) * spacing)) / children
            let rectFor index = x + index*(elemWidth+spacing), y, elemWidth, h
            updateElementRects rectFor elements
        | Some (AlignVertically spacing) -> 
            let elemHeight = (h - ((children-1) * spacing)) / children
            let rectFor index = x, y + index*(elemHeight+spacing), w, elemHeight
            updateElementRects rectFor elements
        
    panel, positioned

let rec getElementView runState =
    function
    | Button config ->
        getButtonView runState config
    | Label config ->
        getLabelView runState config
    | Panel (config, elements) -> 
        let panel, positioned = getPanelView runState config elements
        List.append panel <| List.collect (getElementView runState) positioned
