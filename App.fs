﻿module SuaveTest.App

open Suave // always open suave
open Suave.Successful // for OK-result
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors

let html container = 
    OK (View.index container)
    >=> Writers.setMimeType "text/html; charset=utf-8"

let browse =
    request (fun r ->
        match r.queryParam Path.Store.browseKey with
        | Choice1Of2 genre -> 
            Db.getContext()
            |> Db.getAlbumsForGenre genre
            |> View.browse genre
            |> html
        | Choice2Of2 msg -> BAD_REQUEST msg)

let overview = warbler (fun _ ->
    Db.getContext()
    |> Db.getGenres
    |> List.map (fun g -> g.Name)
    |> View.store
    |> html)

let details id = 
    match Db.getAlbumDetails id (Db.getContext()) with
    | Some album ->
        html (View.details album)
    | None ->
        never

let webPart =
    choose [
        path Path.home >=> html View.home
        path Path.Store.overview >=> overview
        path Path.Store.browse >=> browse
        pathScan Path.Store.details details   
        pathRegex "(.*)\.(css|png)" >=> Files.browseHome
        ]

startWebServer defaultConfig webPart
