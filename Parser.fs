module Parser

open NUnit.Framework
open FsUnit
open FParsec
open LispTypes

type LispState = unit // doesn't have to be unit, of course
type Parser<'t> = Parser<'t, LispState>

let pSymbol: Parser<_> = anyOf "!#$%&|*+-/:<=>?@^_~"

let parseString: Parser<LispVal> =
      between (pstring "\"") (pstring "\"") (manyChars (noneOf (Seq.toList "\"")))
            |>> LispString

let parseAtom =
    pipe2 (letter <|> pSymbol)
          (manyChars (letter <|> digit <|> pSymbol))
          (fun s rest ->
                let atom = sprintf "%c%s" s rest
                match atom with
                | "#t" -> LispBool true
                | "#f" -> LispBool false
                | _ -> LispAtom atom)

let parseNumber: Parser<_> = pint64 |>> LispNumber

let parseExpr = parseAtom <|>
                parseString <|>
                parseNumber

let readExpr input =
    match run parseExpr input with
    | Failure (_, err, _) -> sprintf "No match: %s"  (err.ToString())
    | Success _ -> "Found value"

