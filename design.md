# Design

A set of controls, all of a type, that can be processed by a 'get UI elements' func, and possibly all by a 'get events' func (though this might just be for buttons)

## Core Aspects

### Origin

Nine different draw positions. Are left, right, top, bottom relevant? Or should it just be centre, topleft, bottomright etc?
Could be that origin is combined with x,y, so two different properties are not required.

### Text

Text is always either a single line or multiple, combined with a font asset and a font size

### One off or set?

Right now there is a DU of elements, and 'core' methods for rendering and events
Perhaps elements could be divorced of this, and just be standalone?

## Element Ideas

### Button

Should have a fixed size, three states, give events, and render text in its centre

position, size, text, three colour sets
does it need an origin? origin can be derived by user, given the size is known.

### Label

Possibly size given by contents. Has a set of colours, no events, renders text top left
Probably needs an origin, given size is dynamic
Is this more of a pop up label? Given its border and dynamic nature?

### Stack Panel

Places elements next to each other, in a line or in a column. Each element is resized to fit the size of the stack, with a gap between
Is this useful? How would this work in a game?
It seems to me, a game would serve better just having everything fixed place

### Border / Panel

A simple block with colours and an optional border. Elements can be placed on top, but there need not be a 'child' relationship

### Table

Takes a map of string/string, and renders it out with spacing between rows and columns
Alternatively could take a tuple list or even a list of lists, and do the same. Would there be use of this? Maybe in a paradox-like game