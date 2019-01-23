module GameCore.ImGui.Helpers

open ImGuiNET
open System.Numerics

type Model = {
    Button1: bool
    Button2: bool
    Text: string
}

type UIElement<'UIModel> = 
    | Text of string
    | Button of string * update:('UIModel -> bool -> 'UIModel)
    | TextInput of startValue:('UIModel -> string) * update:('UIModel -> string -> 'UIModel)
    | Direct of ('UIModel -> unit)
    | DirectUpdate of ('UIModel -> 'UIModel)
    | Window of windowConfig: WindowConfig * children: UIElement<'UIModel> list
and WindowConfig = {
    title: string option
    pos: (int * int) option
    size: (int * int) option
    flags: WindowFlags
} and WindowFlags = {
    noCollapse: bool
    noResize: bool
    noMove: bool
    noTitleBar: bool
    autoResize: bool
} with 
    member internal __.AsImGuiWindowFlags = 
        [
            if __.noCollapse then yield ImGuiWindowFlags.NoCollapse
            if __.noResize then yield ImGuiWindowFlags.NoResize
            if __.noMove then yield ImGuiWindowFlags.NoMove
            if __.noTitleBar then yield ImGuiWindowFlags.NoTitleBar
            if __.autoResize then yield ImGuiWindowFlags.AlwaysAutoResize
        ] |> List.reduce (|||)

let standardFlags = { noCollapse = true; noResize = true; noMove = false; noTitleBar = false; autoResize = true }

let window config children = Window (config, children)
let text value = Text value
let button value update = Button (value, update)
let textinput startValue update = TextInput (startValue, update)

let ui = [

    let win1Config = { title = Some "window 1"; pos = Some (10, 10); size = Some (200, 200); flags = standardFlags }
    yield window win1Config [
        text "hello world"
        button "this is a test" (fun m b -> { m with Button1 = b })
    ]

    let win2Config = { title = Some "window 2"; pos = Some (300, 10); size = None; flags = standardFlags }
    yield window win2Config [
        text "test two"
        text "test three"
        textinput (fun m -> m.Text) (fun m us -> { m with Text = us })
        button "test two" (fun m b -> { m with Button2 = b })
    ]

    let win3Config = { title = None; pos = Some (300, 300); size = Some (200, 200); flags = { standardFlags with noTitleBar = true } }
    yield window win3Config [
        yield text "line 1"
        yield text "line 2"
        yield DirectUpdate (fun model -> 
            let mutable buffer = model.Text
            ImGui.InputText("Text input", &buffer, 100ul) |> ignore
            { model with Text = buffer })
        yield Direct (fun model ->
            ImGui.Text model.Text)
        
        let win4Config = { title = Some "sub window"; pos = None; size = None; flags = standardFlags }
        yield window win4Config [
            text "sub"
        ]
    ]

]


let rec renderElement model =
    let renderWindow config children =
        match config.pos with 
            | None -> () 
            | Some (x, y) -> ImGui.SetNextWindowPos (new Vector2 (float32 x, float32 y))
        match config.size with
            | None -> ()
            | Some (w, h) -> ImGui.SetNextWindowSize (new Vector2 (float32 w, float32 h))
        let label = config.title |> Option.defaultValue ""
        ImGui.Begin (label, config.flags.AsImGuiWindowFlags) |> ignore
        let next = (model, children) ||> List.fold renderElement
        ImGui.End ()
        next

    function
    | Text s ->
        ImGui.Text s
        model
    | Button (s, update) ->
        ImGui.Button s |> update model
    | TextInput (startValue, update) ->
        let mutable buffer = startValue model
        ImGui.InputText("", &buffer, 100ul) |> ignore
        update model buffer
    | Direct o ->
        o model
        model
    | DirectUpdate (update) ->
        update model
    | Window (config, children) ->
        renderWindow config children

let render startModel ui = 
    (startModel, ui) ||> List.fold renderElement