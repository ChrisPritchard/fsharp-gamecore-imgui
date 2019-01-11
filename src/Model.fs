module GameCore.UIElements.Model

open Microsoft.Xna.Framework

type ColourSet = {
    text: Color
    background: Color
    border: (int * Color) option
}

type TextSet = {
    text: string list
    font: string
    size: int
}

type ButtonConfig = {
    destRect: int * int * int * int
    text: TextSet
    idleColours: ColourSet
    hoverColours: ColourSet option
    pressedColours: ColourSet
}

type LabelConfig = {
    destRect: int * int * int * int
    text: TextSet
    colours: ColourSet
}

type PanelAlignment =
    | AlignVertically of spacing:int
    | AlignHorizontally of spacing:int

type PanelConfig = {
    destRect: int * int * int * int
    background: Color option
    border: (int * Color) option
    padding: int option
    alignment: PanelAlignment option
}

type UIElement =
    | Button of ButtonConfig
    | Label of LabelConfig
    | Panel of PanelConfig * UIElement list

type UIEvent = | Pressed | Hover