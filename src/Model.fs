module GameCore.UIElements.Model

open Microsoft.Xna.Framework

type UIColourSet = {
    text: Color
    background: Color
    border: (int * Color) option
}

type ButtonConfig = {
    position: int * int
    size: int * int
    text: string
    fontAsset: string
    idleColours: UIColourSet
    hoverColours: UIColourSet option
    pressedColours: UIColourSet
}

type LabelConfig = {
    position: int * int
    size: int * int
    text: string list
    fontAsset: string
    colours: UIColourSet
}

type UIElement =
    | Button of ButtonConfig
    | Label of LabelConfig

type UIEvent = | Pressed | Hover