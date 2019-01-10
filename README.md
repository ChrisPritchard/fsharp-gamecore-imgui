# fsharp-gamecore-ui

Simple UI elements for use with [**fsharp-gamecore**](https://github.com/ChrisPritchard/fsharp-gamecore) in the development of games. So far (as of **0.0.1**) this includes a:

- **Button**, which has a idle, hover and pressed colour state and emits either hover or pressed events
- **Label** which has idle colours only, but supports multiple lines of bordered text
- **Panel** holds other elements, has optional back/border colours, and can stack its children vertically or horizontally

Through the use of nested panels you can create grids.

Available on Nuget at: <https://www.nuget.org/packages/fsharp-gamecore-ui/0.0.1>

## Samples

In this repository (or if you follow the repo url, if using Nuget), there is a samples folder containing a simple game demonstrating the use of the various ui elements in this project.

## License

Provided under **Unilicense** (except for the font, see below), so go nuts.

## Font and its License

The sample project includes a font that is compiled by the monogame pipeline. The font used is 'Connection', from here: https://fontlibrary.org/en/font/connection

This font is provided under the **SIL Open Font License**, a copy of which lies in the root of this repository.