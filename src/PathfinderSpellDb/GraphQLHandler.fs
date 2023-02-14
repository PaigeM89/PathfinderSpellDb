namespace PathfinderSpellDb.Endpoints

open System
open System.IO
open System.Text
open Falco
open PathfinderSpellDb
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open FSharp.Data.GraphQL.Execution
open FSharp.Data.GraphQL
open FSharp.Data.GraphQL.Types
open PathfinderSpellDb
open PathfinderSpellDb.Parsing.Types
open PathfinderSpellDb.GraphQL
open PathfinderSpellDb.JsonConverters

open FsLibLog
open FsLibLog.Types
open FsLibLog.Operators

module GraphQLHandler =

  let logger = LogProvider.getLoggerByName "GraphQLHandler"

  type Root = {
    RequestId : System.Guid
  } with
    static member Create() = { RequestId = System.Guid.NewGuid() }

  let RootType =
    Define.Object<Root>(
      name = "Root",
      description = "The Root type to be passed to all our resolvers.",
      isTypeOf = (fun o -> o :? Root),
      fieldsFn = fun () ->
      [
        Define.Field("requestId", Guid, "The ID of the client.", fun _ (r : Root) -> r.RequestId)
      ])

  let spells : Spell list = SpellParsing.spells |> Seq.toList

  let Query =
    let textInputs = [ Define.Input ("name", String ) ]
    let idInputs = [
      Define.Input ("id", Int)
    ]
    Define.Object<Root>(
      name = "Query",
      fields = [
        Define.Field("spells", ListOf SpellType, "Gets spells", textInputs, fun ctx _ -> ctx.Arg "name" |> spellNameSearch)
        Define.Field("spell", Nullable SpellType, "Gets a specific spell", idInputs, fun ctx _ -> findSpellByIndex (ctx.Arg "id"))
      ]
    )

  let schema : ISchema<Root> = upcast Schema(Query)


  let private converters : JsonConverter[] = [| OptionConverter () |]
  let private jsonSettings = jsonSerializerSettings converters

  let okWithStr str =
    Response.withStatusCode 200 
    >> Response.withHeaders [("Content-Type", "application/json")]
    >> Response.ofPlainText str

  let executor = Executor(schema)

  // TODO: Rewrite completely to async code
  let handle (ctx : HttpContext) =
    let serialize d = JsonConvert.SerializeObject (d, jsonSettings)

    let deserialize (data : string) =
        let getMap (token : JToken) =
            let rec mapper (name : string) (token : JToken) =
                match name, token.Type with
                | "variables", JTokenType.Object -> token.Children<JProperty> () |> Seq.map (fun x -> x.Name, mapper x.Name x.Value) |> Map.ofSeq   |> box
                | name       , JTokenType.Array  -> token                        |> Seq.map (fun x -> mapper name x)                 |> Array.ofSeq |> box
                | _ -> (token :?> JValue).Value

            token.Children<JProperty> ()
            |> Seq.map (fun x -> x.Name, mapper x.Name x.Value)
            |> Map.ofSeq

        if System.String.IsNullOrWhiteSpace (data)
        then None
        else data |> JToken.Parse |> getMap |> Some

    let json =
        function
        | Direct   (data, _) ->
            JsonConvert.SerializeObject (data, jsonSettings)
        | Deferred (data, _, deferred) ->
            deferred |> Observable.add (fun d -> printfn "Deferred: %s" (serialize d))
            JsonConvert.SerializeObject (data, jsonSettings)
        | Stream data ->
            data |> Observable.add (fun d -> printfn "Subscription data: %s" (serialize d))
            "{}"

    let removeWhitespacesAndLineBreaks (str : string) = str.Trim().Replace ("\r\n", " ")

    let readStream (s : Stream) =
        use ms = new MemoryStream (4096)
        s.CopyTo (ms)
        ms.ToArray ()

    let data = Encoding.UTF8.GetString (readStream ctx.Request.Body) |> deserialize

    let query =
        data
        |> Option.bind (fun data ->
            if data.ContainsKey ("query")
            then
                match data.["query"] with
                | :? string as x -> Some x
                | _ -> failwith "Failure deserializing repsonse. Could not read query - it is not stringified in request."
            else
                None)

    let variables =
        data
        |> Option.bind (fun data ->
            if data.ContainsKey ("variables")
            then
                match data.["variables"] with
                | null -> None
                | :? string as x -> deserialize x
                | :? Map<string, obj> as x -> Some x
                | _ -> failwith "Failure deserializing response. Could not read variables - it is not a object in the request."
            else
                None)

    match query, variables with
    | Some query, Some variables ->
        Log.setMessage "Received query and variables for graphql query"
        >> Log.addContextDestructured "query" query
        >> Log.addContextDestructured "variables" variables
        |> logger.info
        
        let query = removeWhitespacesAndLineBreaks query
        let root = Root.Create()
        let result = executor.AsyncExecute (query, root, variables) |> Async.RunSynchronously

        // Log.setMessage "Data response for query"
        // >> Log.addContextDestructured "data" result.Content
        // |> logger.info

        Log.setMessage "Metadata response for query"
        >> Log.addContextDestructured "metadata" result.Metadata
        |> logger.info

        okWithStr (json result) ctx

    | Some query, None ->
        Log.setMessage "Received query and variables for graphql query"
        >> Log.addContextDestructured "query" query
        |> logger.info

        // printfn "Received query: %s" query
        let query = removeWhitespacesAndLineBreaks query
        let result = executor.AsyncExecute (query) |> Async.RunSynchronously

        // Log.setMessage "Data response for query"
        // >> Log.addContextDestructured "data" result.Content
        // |> logger.info

        Log.setMessage "Metadata response for query"
        >> Log.addContextDestructured "metadata" result.Metadata
        |> logger.info

        // printfn "Result content: %A" result.Content
        // printfn "Result metadata: %A" result.Metadata
        okWithStr (json result) ctx
    | None, _ ->
        let result = executor.AsyncExecute (Introspection.IntrospectionQuery) |> Async.RunSynchronously
        // printfn "Result metadata: %A" result.Metadata
        okWithStr (json result) ctx