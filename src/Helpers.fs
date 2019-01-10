module GameCore.UIElements.Helpers

let internal pointsFor (x, y, w, h) = 
    x, y, x + w, y + h

let internal contains (x, y) (rx, ry, rmx, rmy) = 
    x >= rx && y >= ry && x <= rmx && y <= rmy

let internal contract n (rx, ry, rw, rh) =
    rx + n, ry + n, rw - 2*n, rh - 2*n

let internal centre (rx, ry, rw, rh) =
    rx + rw/2, ry + rh/2