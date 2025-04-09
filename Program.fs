open System
open NUnit.Framework
open FsUnit
open FParsec

type LispState = unit // doesn't have to be unit, of course
type Parser<'t> = Parser<'t, LispState>

let pSymbol: Parser<_> = anyOf "!#$%&|*+-/:<=>?@^_~"

type LispVal =
    | LispAtom of string
    | LispList of List<LispVal>
    | LispDottedList of List<LispVal> * LispVal
    | LispNumber of int64
    | LispString of string
    | LispBool of bool

// string
let notQuoteChar = noneOf (Seq.toList "\"")
let unquotedString = manyChars notQuoteChar
let betweenQuotes = between (pstring "\"") (pstring "\"")
let parseString: Parser<LispVal> =
    betweenQuotes unquotedString |>> LispString

// atom
let parseAtom =
    pipe2 (letter <|> pSymbol)
          (manyChars (letter <|> digit <|> pSymbol))
          (fun s rest ->
                let atom = sprintf "%c%s" s rest
                match atom with
                | "#t" -> LispBool true
                | "#f" -> LispBool false
                | _ -> LispAtom atom)

// number
let parseNumber: Parser<_> = pint64 |>> LispNumber

let parseExpr = parseAtom <|>
                parseString <|>
                parseNumber

let readExpr input =
    match run (spaces >>. parseExpr) input with
    | Failure (_, err, _) -> sprintf "No match: %s"  (err.ToString())
    | Success _ -> "Found value"

[<EntryPoint>]
let main argv =
    let input = if argv.Length = 0 then "" else argv.[0]
    let result = readExpr input
    printfn "%s\n" result
    0 // return an integer exit code