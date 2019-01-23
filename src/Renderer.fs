module GameCore.ImGui.Renderer

open ImGuiNET
open Model
open System.Numerics

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
    | TextInput (startValue, length, update) ->
        let mutable buffer = startValue model
        ImGui.InputText("", &buffer, uint32 length) |> ignore
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