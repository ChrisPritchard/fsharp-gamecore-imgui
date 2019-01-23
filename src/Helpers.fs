module GameCore.ImGui.Helpers

open ImGuiNET
open System.Numerics

type Model = {
    Button1: bool
    Button2: bool
    Text: string
}

type UIWindow<'UIModel> = 
    | FixedWindow of label:string * x:int * y:int * width:int * height:int * children: UIElement<'UIModel> list
    | DynamicWindow of label:string * x:int * y:int * children: UIElement<'UIModel> list
and UIElement<'UIModel> = 
    | Text of string
    | Button of string * update:('UIModel -> bool -> 'UIModel)
    | TextInput of startValue:('UIModel -> string) * update:('UIModel -> string -> 'UIModel)
    | Direct of ('UIModel -> unit)
    | DirectUpdate of ('UIModel -> 'UIModel)
    | FixedWindow of label:string * x:int * y:int * width:int * height:int * children: UIElement<'UIModel> list
    | DynamicWindow of label:string * x:int * y:int * children: UIElement<'UIModel> list

let fixedwindow label (x, y, w, h) children = FixedWindow (label, x, y, w, h, children)
let window label (x, y) children = DynamicWindow (label, x, y, children)
let text value = Text value
let button value update = Button (value, update)
let textinput startValue update = TextInput (startValue, update)

let ui = [

    fixedwindow "window 1" (10, 10, 200, 200) [
        text "hello world"
        button "this is a test" (fun m b -> { m with Button1 = b })
    ]

    window "window 2" (300, 10) [
        text "test two"
        text "test three"
        textinput (fun m -> m.Text) (fun m us -> { m with Text = us })
        button "test two" (fun m b -> { m with Button2 = b })
    ]

    fixedwindow "" (300, 300, 200, 200) [
        text "line 1"
        text "line 2"
        DirectUpdate (fun model -> 
            let mutable buffer = model.Text
            ImGui.InputText("Text input", &buffer, 100ul) |> ignore
            { model with Text = buffer })
        Direct (fun model ->
            ImGui.Text model.Text)
        window "sub window" (20, 20) [
            text "sub"
        ]
    ]

]

let standardWindowFlags = ImGuiWindowFlags.NoCollapse ||| ImGuiWindowFlags.NoResize ||| ImGuiWindowFlags.NoMove
let flags label = 
    if label = "" then standardWindowFlags ||| ImGuiWindowFlags.NoTitleBar else standardWindowFlags

let rec renderElement model =
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
    | FixedWindow (label, x, y, w, h, children) ->
        ImGui.SetNextWindowPos (new Vector2 (float32 x, float32 y))
        ImGui.SetNextWindowSize (new Vector2 (float32 w, float32 h))
        ImGui.Begin (label, flags label) |> ignore
        let next = (model, children) ||> List.fold renderElement
        ImGui.End ()
        next
    | DynamicWindow (label, x, y, children) ->
        ImGui.SetNextWindowPos (new Vector2 (float32 x, float32 y))
        ImGui.Begin (label, flags label ||| ImGuiWindowFlags.AlwaysAutoResize) |> ignore
        let next = (model, children) ||> List.fold renderElement
        ImGui.End ()
        next

let render startModel ui = 
    (startModel, ui) ||> List.fold renderElement