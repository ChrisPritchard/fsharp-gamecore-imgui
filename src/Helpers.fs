module GameCore.UIElements.Helpers

open GameCore.GameModel
open Model

let internal pointsFor (x, y, w, h) = 
    x, y, x + w, y + h

let internal contains (x, y) (rx, ry, rmx, rmy) = 
    x >= rx && y >= ry && x <= rmx && y <= rmy

let internal contract n (rx, ry, rw, rh) =
    rx + n, ry + n, rw - 2*n, rh - 2*n

let internal centre (rx, ry, rw, rh) =
    rx + rw/2, ry + rh/2

let internal size lines fontHeight = 
    let maxLength = lines |> Seq.map Seq.length |> Seq.max
    let approxWidth = float (maxLength * fontHeight) * 0.8
    match List.length lines with
    | 1 -> approxWidth, float fontHeight
    | n ->
        let lineSize = (float fontHeight / 3.) * 4.
        let approxHeight = (float (n-1) * lineSize) + float fontHeight
        approxWidth, approxHeight

let internal align (x, y) (width, height) = 
    let x, y = float x, float y
    function
    | TopLeft -> x, y
    | Left -> x, y - height/2.
    | BottomLeft -> x, y - height
    | Top -> x - width/2., y
    | Centre -> x - width/2., y - width/2.
    | Bottom -> x - width/2., y - height
    | TopRight -> x - width, y
    | Right -> x - width, y - width/2.
    | BottomRight -> x - width, y - height

let internal rect (width, height) padding position origin = 
    let width, height = width + padding*2., height + padding*2.
    let x, y = align position (width, height) origin
    int x, int y, int width, int height

let internal rectFor = 
    function
    | Button config ->
        let size = size config.text.lines config.text.size
        rect size (float config.padding) config.position config.origin
    // | Label config ->
    //     let size = size config.text.lines config.text.size
    //     rect size (float config.padding) config.position config.origin