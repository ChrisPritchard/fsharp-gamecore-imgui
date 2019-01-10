module GameCore.UIElements.Model

open Microsoft.Xna.Framework

type UIColourSet = {
    text: Color
    background: Color
    border: (int * Color) option
}

type ButtonConfig = {
    destRect: int * int * int * int
    text: string
    fontAsset: string
    idleColours: UIColourSet
    hoverColours: UIColourSet option
    pressedColours: UIColourSet
}

type LabelConfig = {
    destRect: int * int * int * int
    text: string list
    fontAsset: string
    colours: UIColourSet
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