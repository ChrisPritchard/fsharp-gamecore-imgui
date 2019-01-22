module GameCore.ImGui.Helpers

open ImGuiNET
open System.Numerics

type Model = {
    Button1: bool
    Button2: bool
}

type UIWindow<'UIModel> = 
    | FixedWindow of label:string * x:int * y:int * width:int * height:int * children: UIElement<'UIModel> list
    | Window of label:string * x:int * y:int * children: UIElement<'UIModel> list
and UIElement<'UIModel> = 
    | Text of string
    | Button of string * update:('UIModel -> bool -> 'UIModel)

let fixedwindow label (x, y, w, h) children = FixedWindow (label, x, y, w, h, children)
let window label (x, y) children = Window (label, x, y, children)
let text value = Text value
let button value update = Button (value, update)

let ui = [

    fixedwindow "window 1" (10, 10, 200, 200) [
        text "hello world"
        button "this is a test" (fun m b -> { m with Button1 = b })
    ]

    window "window 2" (300, 10) [
        text "test two"
        text "test three"
        button "test two" (fun m b -> { m with Button2 = b })
    ]

    fixedwindow "" (300, 300, 200, 200) [
        text "line 1"
        text "line 2"
        text "line 3"
    ]

]

let standardWindowFlags = ImGuiWindowFlags.NoCollapse ||| ImGuiWindowFlags.NoResize ||| ImGuiWindowFlags.NoMove

let renderElements children startModel =
    (startModel, children)
    ||> List.fold (fun model element ->
        match element with
        | Text s ->
            ImGui.Text s
            model
        | Button (s, update) ->
            ImGui.Button s |> update model)

let flags label = 
    if label = "" then standardWindowFlags ||| ImGuiWindowFlags.NoTitleBar else standardWindowFlags

let render ui startModel = 
    (startModel, ui)
    ||> List.fold (fun model window ->
        match window with
        | FixedWindow (label, x, y, w, h, children) ->
            ImGui.SetNextWindowPos (new Vector2 (float32 x, float32 y))
            ImGui.SetNextWindowSize (new Vector2 (float32 w, float32 h))
            ImGui.Begin (label, flags label) |> ignore
            let next = renderElements children model
            ImGui.End ()
            next
        | Window (label, x, y, children) ->
            ImGui.SetNextWindowPos (new Vector2 (float32 x, float32 y))
            ImGui.Begin (label, flags label) |> ignore
            let next = renderElements children model
            ImGui.End ()
            next)