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
    | Checkbox (label, startValue, update) ->
        let mutable state = startValue model
        ImGui.Checkbox (label, &state) |> ignore
        update model state
    | TextInput (startValue, length, update) ->
        let mutable buffer = startValue model
        ImGui.InputText("", &buffer, uint32 length) |> ignore
        update model buffer
    | TextAreaInput (startValue, length, (w, h), update) ->
        let mutable buffer = startValue model
        ImGui.InputTextMultiline ("", &buffer, uint32 length, new Vector2(float32 w, float32 h)) |> ignore
        update model buffer
    | Row children ->
        let lasti = List.length children - 1
        ((0, model), children) 
        ||> List.fold (fun (i, last) child -> 
            let next = renderElement last child
            if i <> lasti then ImGui.SameLine ()
            i + 1, next) 
        |> snd
    | Direct o ->
        o model
        model
    | DirectUpdate (update) ->
        update model
    | Window (config, children) ->
        renderWindow config children

let internal applyStyle styleConfig = 
    let mutable style = ImGui.GetStyle ()
    style.WindowRounding <- float32 styleConfig.windowRounding

let render styleConfig startModel ui = 
    applyStyle styleConfig
    (startModel, ui) ||> List.fold renderElement