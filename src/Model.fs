module GameCore.ImGui.Model

open ImGuiNET

type StyleConfig = {
    windowRounding: float
}

type Element<'UIModel> = 
    | Text of string
    | Button of string * update:('UIModel -> bool -> 'UIModel)
    | TextInput of startValue:('UIModel -> string) * length: int * update:('UIModel -> string -> 'UIModel)
    | Row of children: Element<'UIModel> list
    | Direct of ('UIModel -> unit)
    | DirectUpdate of ('UIModel -> 'UIModel)
    | Window of windowConfig: WindowConfig * children: Element<'UIModel> list
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
let textinput startValue length update = TextInput (startValue, length, update)
let row children = Row children