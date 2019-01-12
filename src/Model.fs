module GameCore.UIElements.Model

open Microsoft.Xna.Framework
open GameCore.GameModel

type ColourSet = {
    text: Color
    background: Color
    border: (int * Color) option
}

type TextSet = {
    lines: string list
    font: string
    size: int
}

type ButtonConfig = {
    position: int * int
    origin: Origin
    text: TextSet
    padding: int
    idleColours: ColourSet
    hoverColours: ColourSet option
    pressedColours: ColourSet
}

// type LabelConfig = {
//     position: int * int
//     origin: Origin
//     text: TextSet
//     padding: int
//     colours: ColourSet
// }

// type Alignment =
//     | Vertical of spacing:int
//     | Horizontal of spacing:int

// type StackPanelConfig = {
//     position: int * int
//     origin: Origin
//     alignment: Alignment option
// }

type UIElement =
    | Button of ButtonConfig
    // | Label of LabelConfig
    // | StackPanel of StackPanelConfig * UIElement list

type UIEvent = Pressed | Hover