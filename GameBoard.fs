namespace FunkyTicTacToe

module GameBoard =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    open Avalonia.FuncUI.Types
    open Avalonia.Controls
    open Avalonia.FuncUI
    open Avalonia.Controls.Primitives

    open Types

    let gameStatusMessage (state: GameState) =
        match state.GameStatus with
        | InProgress -> $"Player {state.Player}'s Turn"
        | Tie -> "Tie Game, Try Again!"
        | Winner player -> $"Player {player} Won!"

    let update (msg: Msg) (state: GameState) : GameState =
        match msg with
        | CellClicked cell -> GameState.setValue state { cell with Status = Full state.Player }
        | Reset -> GameState.init

    let view (state: GameState) (dispatch) =
        StackPanel.create
            [ StackPanel.horizontalAlignment HorizontalAlignment.Center
              StackPanel.verticalAlignment VerticalAlignment.Center
              StackPanel.children
                  [ TextBlock.create
                        [ TextBlock.horizontalAlignment HorizontalAlignment.Center
                          TextBlock.verticalAlignment VerticalAlignment.Center
                          TextBlock.fontSize 30
                          TextBlock.text (gameStatusMessage state) ]
                    UniformGrid.create
                        [ UniformGrid.columns 3
                          UniformGrid.rows 3
                          UniformGrid.margin 50
                          UniformGrid.minHeight 300
                          UniformGrid.minWidth 300
                          UniformGrid.horizontalAlignment HorizontalAlignment.Center
                          UniformGrid.verticalAlignment VerticalAlignment.Center
                          UniformGrid.children (
                              state.Cells
                              |> List.map (fun cell ->
                                  Button.create
                                      [ Button.onClick ((fun _ -> dispatch (CellClicked cell)), OnChangeOf cell.Status)
                                        Button.isEnabled (state.GameStatus = InProgress)
                                        Button.background "#014a4a"
                                        Button.foreground "white"
                                        Button.content (GameState.getValue cell)
                                        Button.fontSize 30
                                        Button.verticalContentAlignment VerticalAlignment.Center
                                        Button.horizontalContentAlignment HorizontalAlignment.Center
                                        Button.margin 2.
                                        Button.width 96.
                                        Button.height 96. ]
                                  |> generalize)
                              |> List.map (fun views -> views)
                          ) ]
                    Button.create
                        [ Button.content "Reset"
                          Button.onClick (fun _ -> dispatch Reset)
                          Button.fontSize 30
                          Button.background "#014a4a"
                          Button.foreground "white"
                          Button.horizontalContentAlignment HorizontalAlignment.Center
                          Button.verticalContentAlignment VerticalAlignment.Center
                          Button.horizontalAlignment HorizontalAlignment.Center
                          Button.verticalAlignment VerticalAlignment.Center
                          Button.height 50
                          Button.width 200 ] ] ]
        |> generalize
