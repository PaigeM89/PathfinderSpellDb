namespace PathfinderSpellDb.JsonConverters


open Newtonsoft.Json
open Newtonsoft.Json.Linq
open Microsoft.FSharp.Reflection
open FSharp.Data.GraphQL
open FSharp.Data.GraphQL.Types
open FSharp.Data.GraphQL.Types.Patterns
open FsLibLog
open System

[<Sealed>]
type OptionConverter () =
    inherit JsonConverter ()
    override _.CanConvert (t) =
        t.IsGenericType && t.GetGenericTypeDefinition () = typedefof<option<_>>
    override _.WriteJson (writer, value, serializer) =

        let value =
            if isNull value then null
            else
                let _, fields = Microsoft.FSharp.Reflection.FSharpValue.GetUnionFields (value, value.GetType ())
                fields.[0]

        serializer.Serialize (writer, value)

    override _.ReadJson (reader, t, _, serializer) =

        let innerType = t.GetGenericArguments().[0]

        let innerType =
            if innerType.IsValueType
            then (typedefof<System.Nullable<_>>).MakeGenericType ([| innerType |])
            else innerType

        let value = serializer.Deserialize (reader, innerType)
        let cases = FSharpType.GetUnionCases (t)

        if isNull value
        then FSharpValue.MakeUnion (cases.[0], [||])
        else FSharpValue.MakeUnion (cases.[1], [| value |])

type DUConverter() =
  inherit JsonConverter()

  let logger = LogProvider.getLoggerByName "Newtonsoft.Json.FSharp.DUConverter"

  // todo: I copy & pasted this from here: https://github.com/haf/Newtonsoft.Json.FSharp/blob/master/src/JsonNet/Converters/OptionConverter.fs
  // and did not edit any of it to actually work
  override x.CanConvert t =
    Log.setMessage "Checking if can convert type t"
    >> Log.addContextDestructured "t" t
    |> logger.info
    t.IsGenericType
    && typedefof<option<_>>.Equals (t.GetGenericTypeDefinition())

  override x.WriteJson(writer, value, serializer) =
    Log.setMessage "Attempting to write JSON for custom DU"
    >> Log.addContextDestructured "value" value
    |> logger.info
    let value =
      if value = null then
        null
      else 
        let _,fields = FSharpValue.GetUnionFields(value, value.GetType())
        fields.[0]
    serializer.Serialize(writer, value)

  override x.ReadJson(reader, t, existingValue, serializer) =
    let innerType = t.GetGenericArguments().[0]

    let innerType = 
      if innerType.IsValueType then
        typedefof<Nullable<_>>.MakeGenericType([| innerType |])
      else
        innerType

    let value = serializer.Deserialize(reader, innerType)
    let cases = FSharpType.GetUnionCases t

    if value = null then
      FSharpValue.MakeUnion(cases.[0], [||])
    else
      FSharpValue.MakeUnion(cases.[1], [|value|])

type GraphQLQuery =
    { ExecutionPlan : ExecutionPlan
      Variables : Map<string, obj> }

[<Sealed>]
type GraphQLQueryConverter<'a> (executor : Executor<'a>, replacements : Map<string, obj>, ?meta : Metadata) =
    inherit JsonConverter ()

    override _.CanConvert (t) = t = typeof<GraphQLQuery>

    override _.WriteJson (_, _, _) = failwith "Not supported"

    override _.ReadJson (reader, _, _, serializer) =

        let jobj = JObject.Load reader
        let query = jobj.Property("query").Value.ToString ()

        let plan =
            match meta with
            | Some meta -> executor.CreateExecutionPlan (query, meta = meta)
            | None -> executor.CreateExecutionPlan (query)

        let varDefs = plan.Variables

        match varDefs with
        | [] -> upcast { ExecutionPlan = plan; Variables = Map.empty }
        | vs ->
            // For multipart requests, we need to replace some variables
            Map.iter (fun path rep -> jobj.SelectToken(path).Replace (JObject.FromObject (rep))) replacements
            let vars = JObject.Parse (jobj.Property("variables").Value.ToString ())

            let variables =
                vs
                |> List.fold
                    (fun (acc : Map<string, obj>) (vdef : VarDef) ->
                        match vars.TryGetValue (vdef.Name) with
                        | true, jval ->
                            let v =
                                match jval.Type with
                                | JTokenType.Null -> null
                                | JTokenType.String -> jval.ToString () :> obj
                                | _ -> jval.ToObject (vdef.TypeDef.Type, serializer)

                            Map.add (vdef.Name) v acc
                        | false, _ ->
                            match vdef.DefaultValue, vdef.TypeDef with
                            | Some _, _ -> acc
                            | _, Nullable _ -> acc
                            | None, _ -> failwithf "Variable %s has no default value and is missing!" vdef.Name)
                    Map.empty

            upcast { ExecutionPlan = plan; Variables = variables }