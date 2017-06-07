module SuaveTest.Db


open FSharp.Data.Sql

//let [<Literal>] dbVendor = Common.DatabaseProviderTypes.MSSQLSERVER
//let [<Literal>] connString = "Server=(localdb)\MSSQLLocalDB;Database=SuaveMusicStoreApp;Trusted_Connection=true;MultipleActiveResultSets=true"
//let [<Literal>] useOptTypes  = true


//type Sql = 
//    SqlDataProvider<
//        "Data Source=(localdb)\MSSQLLocalDB;Database=SuaveMusicStoreApp;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True" >

type Sql = SqlDataProvider<Common.DatabaseProviderTypes.MSSQLSERVER,"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SuaveTest;Integrated Security=True">

type DbContext = Sql.dataContext
type Album = DbContext.``dbo.AlbumsEntity``
type Genre = DbContext.``dbo.GenresEntity``
type AlbumDetails = DbContext.``dbo.AlbumDetailsEntity``

let firstOrNone s = s |> Seq.tryFind (fun _ -> true)

let getGenres (ctx : DbContext) : Genre list = 
    ctx.Dbo.Genres |> Seq.toList

let getAlbumsForGenre genreName (ctx: DbContext) : Album list = 
    query {
        for album in ctx.Dbo.Albums do
            join genre in ctx.Dbo.Genres on (album.GenreId = genre.GenreId)
            where (genre.Name = genreName)
            select album
    }
    |> Seq.toList
    

let getAlbumDetails id (ctx: DbContext) : AlbumDetails option =
    query {
        for album in ctx.Dbo.AlbumDetails do
            where (album.AlbumId = id)
            select album
        
    } |> firstOrNone

let getContext() = Sql.GetDataContext()



