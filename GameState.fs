namespace FunkyTicTacToe

open Types

module GameState =

    let init =
        [ 0..8 ]
        |> List.map CellPosition
        |> List.map (fun pos -> { Position = pos; Status = Empty })
        |> fun cells ->
            { Cells = cells
              Player = X
              GameStatus = InProgress }


    let getValue (cell: Cell) : string =
        match cell.Status with
        | Empty -> ""
        | Full token ->
            match token with
            | X -> "X"
            | O -> "O"

    let canClickCell ({ Status = status }) =
        match status with
        | Empty -> true
        | Full _ -> false

    let setCellValue newCell oldCell : CellUpdateResult =
        match oldCell.Position = newCell.Position with
        | true ->
            match canClickCell oldCell with
            | true -> { Cell = newCell; Updated = true }
            | false -> { Cell = oldCell; Updated = false }
        | false -> { Cell = oldCell; Updated = false }

    let shouldUpdatePlayer cellUpdateResults =
        cellUpdateResults
        |> List.map (fun ({ Updated = updated }: CellUpdateResult) -> updated)
        |> List.contains true

    let extractCellsFromUpdate (cellUpdateResults: CellUpdateResult list) =
        cellUpdateResults |> List.map (fun ({ Cell = cell }) -> cell)

    let swapPlayerToken player =
        match player with
        | X -> O
        | O -> X

    let playerClaimedCell player cell =
        match cell.Status with
        | Full playerToken -> player = playerToken
        | Empty -> false

    let leftColumn () = [ 0; 3; 6 ] |> List.map CellPosition

    let middleColumn () = [ 1; 4; 7 ] |> List.map CellPosition

    let rightColumn () = [ 2; 5; 8 ] |> List.map CellPosition

    let topRow () = [ 0; 1; 2 ] |> List.map CellPosition

    let middleRow () = [ 3; 4; 5 ] |> List.map CellPosition

    let bottomRow () = [ 6; 7; 8 ] |> List.map CellPosition

    let diagonalDownToRight () = [ 0; 4; 8 ] |> List.map CellPosition

    let diagonalDownToLeft () = [ 2; 4; 6 ] |> List.map CellPosition

    let checkWin ({ Cells = cells; Player = player }) winPositions =
        cells
        |> List.where (fun cell -> winPositions |> List.contains cell.Position)
        |> List.forall (fun cell -> playerClaimedCell player cell)

    let checkForWinCondition ({ Player = player } as state) =
        [ leftColumn
          middleColumn
          rightColumn
          topRow
          middleRow
          bottomRow
          diagonalDownToLeft
          diagonalDownToRight ]
        |> List.map (fun positionList -> (positionList () |> checkWin state))
        |> List.contains true
        |> function
            | true ->
                { state with
                    GameStatus = Winner player }
            | false -> state

    let checkForNoMoreMoves ({ Cells = cells } as gameState) =
        cells
        |> List.map (fun cell -> cell.Status = Empty)
        |> List.contains true
        |> function
            | true -> gameState
            | false ->
                match gameState.GameStatus with
                | InProgress -> { gameState with GameStatus = Tie }
                | _ -> gameState

    let swapPlayer state =
        { state with
            Player = swapPlayerToken state.Player }

    let updateCells state cells = { state with Cells = cells }

    let handlePlayerSwap state =
        match state.GameStatus with
        | InProgress -> swapPlayer state
        | _ -> state

    let checkForWinAndSwapPlayers =
        checkForWinCondition >> checkForNoMoreMoves >> handlePlayerSwap

    let getCellUpdates updateResults =
        (extractCellsFromUpdate updateResults), (shouldUpdatePlayer updateResults)

    let modifyStateAfterUpdates gameState (updatedCells, shouldUpdatePlayer) =
        match shouldUpdatePlayer with
        | true -> updatedCells |> updateCells gameState |> checkForWinAndSwapPlayers
        | false -> gameState

    let setValue ({ Cells = cells } as gameState) cell =
        cells
        |> List.map (setCellValue cell)
        |> getCellUpdates
        |> modifyStateAfterUpdates gameState
