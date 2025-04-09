# Write Yourself a Scheme in 48 hours using F#!

[참고](https://write-yourself-a-scheme.pangwa.com/#/)

## First Steps: Compiling and running

```F#
open System

[<EntryPoint>]
let main argv =
    let who = if argv.Length = 0 then "F#" else argv.[0]
    printfn "Hello world from %s" who
    0 // return an integer exit code
```

+ `open`: 첫 번째 줄은 `System` 모듈을열도록 지정

+  **attribute**: F#에서 `[<...>]` 형태의 annotation을 attribute라고 한다. [attribute doc](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/attributes).
  + 모든 F# 프로그램에는 attribute로 지정된 진입점(`[<EntryPoint>]`)이 하나 있어야 한다. 
  + attribute를 사용하면 프로그래밍 구조에 메타데이터를 적용할 수 있다.
+ `let main argv =`: type declaration이다. `main`이 인수 하나를 취하는 함수임을 나타낸다.
  + F#에서 함수는 일반적으로 변수로 정의된다. 둘다 `let` 키워드를 사용한다.
  + `argv`: 프로그팸을 실행할때 사용자가 전달한 인수 배열이다.  
+ **type inference**: F#에는 매우 강력한 type inference(타입 추론) 기능이 있으며. 일반적으로 컨텍스트에 따라 타입을 추론한다. 대부분의 경우 F# 프로그램을 작성하는 동안 형식을 명시적으로 지정할 필요가 없다. 정말 필요한 경우 명시적 타입 annotation을 적용할 수 있다.
  +  `argv` 타입이나 `main` 함수의 반환 값 타입을 지정하지 않은 것을 알 수 있다.
+ **등호**: F#에서 같음은 단일 `=`를 사용하여 검사하며 `==`는 사용하지 않는다.
+ `printfn`:
  + `System` 모듈에서 제공한 콘솔에 텍스트를 쓰기 위해 `printfn` 함수를 사용한다.
  + F#은 순수한 함수형 프로그래밍 언어가 아니며, Haskell과 달리 F#에서 IO 작업을 수행하기 위해 IO 모나드를 제공할 필요가 없다.

```
$ dotnet run
$ dotnet run -- jacky
```

### Exercises

**1.** 명령줄에서 두 개의 인수를 읽고 두 인수를 모두 사용하여 메시지를 인쇄하도록 프로그램을 변경하자.

```F#
open System

[<EntryPoint>]
let main argv =
    let where = if argv.Length = 0 then "world" else argv.[0]
    let who = if argv.Length < 2 then "F#" else argv.[1]
    printfn "Hello %s from %s" where who
    0 // return an integer exit code
```

**2.** 두 인수에 대해 간단한 산술 연산을 수행하고 결과를 출력하도록 프로그램을 변경하자. `Int32.tryParse`를 사용하여 문자열을 숫자로 변환하고 `sprintfn` 함수를 사용하여 문자열로 변환할 수 있다.

```F#
open System

[<EntryPoint>]
let main argv =
    let a = argv.[0] |> int
    let b = argv.[1] |> int
    let res = sprintf "%i" (a + b)
    printfn "result: %s" res
    0 // return an integer exit code
```

**3.** `Console.ReadLine()`은 콘솔에서 줄을 읽고 문자열로 반환한다. 프로그램을 변경하여 이름을 묻고 이름을 읽은 다음 명령줄 값 대신 해당 이름을 인쇄하자.

```F#
open System

[<EntryPoint>]
let main argv =
    let who = Console.ReadLine()
    printfn "Hello world from %s" who
    0 // return an integer exit code
```

## Testing

.NET에는 매우 간단한 단위 테스트 시스템이 있다. 간단한 예를 살펴보자.

1. `dotnet add package` 명령을 사용하여 필요한 패키지를 추가.

   ```
   dotnet add package FSUnit
   dotnet add package NUnit
   dotnet add package NUnit3TestAdapter
   dotnet add package Microsoft.Net.Test.Sdk
   ```

2. `Program.fs`에 다음 코드 추가.

   ```F#
   open NUnit.Framework
   open FsUnit
   
   [<Test>]
   let ``test hello`` () =
       5 + 1 |> should equal 6
   ```

   + **함수 이름**: 함수 이름을 `test hello`로 지었다. F#에서 이름에는 공백이 포함될 수 있으며 더블 `를 사용하여 묶어야 한다.
   + **인수를 취하지 않는 함수**: 이런 함수의 경우, 값과 구별하기 위해 이름 뒤에 명시적 `()`를 추가해야 한다.
   + **pipe 연산자**(`|>`): 왼쪽 표현식의 결과를 오른쪽 함수에 전달한다. 따라서`5 + 1 |> should equal 6`은`should equal 6 (5 + 1)`과 같다.  

3. 테스트를 실행.

   ```
   dotnet test
   ```

### Exercises

**1.** 테스트를 실패하도록 수정한 후 실행하고 출력을 확인하자.

```F#
[<Test>]
let ``test hello`` () =
    5 + 2 |> should equal 6
```

```
테스트 요약: 합계: 1, 실패: 1, 성공: 0, 건너뜀: 0, 기간: 0.9초
1 오류와 1 경고와 함께 실패 빌드(2.7초)
```

## Parsing

아주 간단한 파서를 작성해 보자. 이 장에서는 fparsec을 사용하여 Scheme 파서를 작성한다.

```
dotnet add package fparsec
dotnet add package FSharpPlus
```

### Writing a Simple Parser

1. 가져오기 섹션에 다음줄 추가:

   ```F#
   open FParsec
   ```

2. 이제 Scheme 식별자에서 허용되는 기호 중 하나를 인식하는 파서를 정의해 보자.

   ```F#
   type LispState = unit // doesn't have to be unit, of course
   type Parser<'t> = Parser<'t, LispState>
   
   let pSymbol: Parser<_> = anyOf "!#$%&|*+-/:<=>?@^_~"
   ```

   + 이는 모나드의 예이다. 이 경우 숨겨지는 "추가 정보"는 입력 스트림의 위치, 백트래킹 레코드, 첫 번째 및 두 번째 세트 등에 대한 모든 정보이다. `FParsec`이 이 모든 것을 처리해 준다. `FParsec` 라이브러리 함수인 `anyOf`만 사용하면 전달된 문자열의 모든 문자 중 하나만 인식한다.
   + pSymbol에 대한 명시적 타입 정보를 추가한 것도 참고하자. 이 정보가 없으면 컴파일 오류가 발생하기 때문이다. 이 오류는 F#의 "value restriction"에 대한 내용이며, 해당 내용은 [문서](https://www.quanttec.com/fparsec/tutorial.html#fs-value-restriction)에 설명되어 있다.

3. 파서를 호출하고 발생할 수 있는 오류를 처리하는 함수를 정의하자.

   ```F#
   let readExpr input =
       match run pSymbol input with
       | Failure (_, err, _) -> sprintf "No match: %s"  (err.ToString())
       | Success _ -> "Found value"
   ```

   + 타입 시그니처에서 볼 수 있듯이 `readExpr`은 문자열에서 문자열로 변환하는 함수(->)이다.
   + run은 파싱된 값이나 오류를 반환할 수 있으므로 오류 사례를 처리해야 한다. `FParsec` 규칙에 따라 `FParsec`은 `arseResult` 데이터 유형을 반환하며, `Failure` constructor를 사용하여 오류를 나타내고 `Success` constructor를 사용하여 정상 값을 나타낸다.
   + **match**: `match ... with` construction은 pattern matching의 한 예이다. 나중에 살펴보겠다.

4. 마지막으로, readExpr을 호출하고 결과를 출력하도록 메인 함수를 변경한다.

   ```F#
   let main argv =
       let input = if argv.Length = 0 then "" else argv.[0]
       let result = readExpr input
       printfn "%s\n" result
       0 // return an integer exit code
   ```

```
$ dotnet run -- '|'
Found value

$ dotnet run -- 1
No match: Error in Ln: 1 Col: 1
Expecting: any char in ‘!#$%&|*+-/:<=>?@^_~’
```

### Whitespace

파서가 점점 더 복잡한 표현식을 인식할 수 있도록 일련의 개선 사항을 추가할 것이다. 현재 파서는 기호 앞에 공백이 있으면 작동하지 않는다.

공백을 무시하도록 수정하자. `FParsec`은 아무리 많은 공백 문자라도 일치시킬 수 있는 `space` 파서를 제공한다.

```F#
let readExpr input =
    match run (spaces >>. pSymbol) input with
    | Failure (_, err, _) -> sprintf "No match: %s"  (err.ToString())
    | Success _ -> "Found value"
```

+ `>>.`: `FParsc`가 제공하는 결합자. 파서 `p1 >>. p2`는 `p1`과 `p2`를 순서대로 파싱하고 `p2`의 결과를 반환한다.
  + `.>>`: `p1`과 `p2`를 순서대로 파싱하지만 `p2` 대신 `p1`의 결과를 반환한다.
  + 두 연산자를 `p1 >>. p2 .>> p3`에 결합하면 `p1`, `p2`,`p3`을 순서대로 파싱하고 `p2`의 결과를 반환한다.

```
$ dotnet run -- '|'
Found value

$ dotnet run -- '    %'
Found value

$ dotnet run -- '    abc'
No match: Error in Ln: 1 Col: 4
Expecting: any char in ‘!#$%&|*+-/:<=>?@^_~’
```

### Return Values

현재 파서는 별다른 작업을 하지 않는다. 주어진 문자열을 인식할 수 있는지 여부만 알려준다. 일반적으로 우리는 파서에 더 많은 기능을 기대한다. 입력값을 쉽게 탐색할 수 있는 데이터 구조로 변환해 주는 것이다. 이 섹션에서는 데이터 유형을 정의하는 방법과 파서가 해당 데이터 유형을 반환하도록 수정하는 방법을 알아본다.

1. 모든 Lisp 값을 저장할 수 있는 데이터 유형을 정의한다.

   ```F#
   type LispVal =
       | LispAtom of string
       | LispList of List<LispVal>
       | LispDottedList of List<LispVal> * LispVal
       | LispNumber of int64
       | LispString of string
       | LispBool of bool
   ```

   + **discriminated union**: `LispVal` 유형의 변수가 보유할수 있는 가능한 값 집합을 정의한다.
     + 각 대안(constructor라고 하며 `|`로 구분)에는 constructor의 이름과 constructor가 보유할 수 있는 데이터 유형이 포함된다.
     

   + `LispVal`: 기존 F#의 유형이나 키워드와의 충돌을 피하기 위해 유형에는 `Lisp` 접두사가 붙는다.

     + `Atom`: 원자의 이름을 지정하는 문자열을 저장.

     + `List`: 다른 `LispVal`의 list를 저장하는 list(proper list). 

       > 여기서는 보다 일반적인 `Seq` 유형보다 `List`를 선호하는데, `List`는 패턴 매칭이 가능하기 때문에 나중에 유용하게 사용할 수 있기 때문이다.

     + `DottedList`: improper list를 나타낸다. 이것은 마지막 요소를 제외한 모든 요소의 목록을저장한 다음 마지막 요소를다른 필드로 저장한다.

     + `Number`: F# Integer를 포함.

     + `String`: F# String을 포함.

     + `Bool`: F# 불 값을 포함.

2. 이러한 유형의 값을 생성하기 위해 몇 가지 구문 분석 함수를 더 추가하자. 더 복잡한 파서를 만들기 위해 한 번에 하나씩 작은 파서를 구축할 수 있는 강력한 도구인 [Parser Combinators](https://fsharpforfunandprofit.com/posts/understanding-parser-combinators/)를 사용할 것이다.

3. **string**: 정의를 살펴보면 문자열은 "A string is a double quote mark, followed by any number of non-quote characters, followed by a closing quote mark."이다.

   1. 샌드위치의 핵심부터 시작해 보자. "any number of non-quote characters"는 두 부분으로 나눌 수 있다.

      1. "non-quote characters"

         ```F#
         let notQuoteChar = noneOf (Seq.toList "\"")
         ```

      2. "any number of" (which is really just #1 repeating)

         ```F#
         let unquotedString = manyChars notQuoteChar
         ```

   2. 이제 두 개의 따옴표 사이에 따옴표 없는 문자열이 필요하다. `FParsec`은 `between` 함수를 통해 바로 이 기능을 제공한다.

      ```F#
      let betweenQuotes = between (pstring "\"") (pstring "\"")
      ```

   3. 완성.

      ```F#
      let parseString: Parser<LispVal> =
          betweenQuotes unquotedString |>> LispString
      ```

      + `|>>`: 이 연산자를 사용하여 파서 결과를 `LispString` 함수로 파이프한다.
      + `LispString`: (`LispVal` 데이터 유형의) constructor로, 이를 `LispVal`로 변환한다. Record 유형의 모든 constructor는 인수를 해당 유형의 값으로 변환하는 함수처럼 작동한다. 또한 pattern matching expression의 좌변에서 사용할 수 있는 패턴 역할도 한다.

4. **atom**: 문자 또는 기호이며, 그 뒤에는 여러 개의 문자, 숫자 또는 기호가 붙는다.

   ```F#
   let parseAtom =
       pipe2 (letter <|> pSymbol)
             (manyChars (letter <|> digit <|> pSymbol))
             (fun s rest ->
                   let atom = sprintf "%c%s" s rest
                   match atom with
                   | "#t" -> LispBool true
                   | "#f" -> LispBool false
                   | _ -> LispAtom atom)
   
   ```

   + `<|>`: 이 연산자는 첫 번째 파서를 시도한 후, 실패하면 두 번째 파서를 시도한다. 두 파서 중 하나라도 성공하면 해당 파서가 반환한 값을 반환한다. 첫 번째 파서는 입력을 사용하기 전에 반드시 실패해야 한다.

5. **number**:

   ```F#
   let parseNumber: Parser<_> = pint64 |>> LispNumber
   ```

   + `FParsec` 파서 `pint64`는 `int64` 값을 파싱한다. `LispNumber` constructor를 사용하여 결과 숫자에서 숫자 `LispVal`을 생성하려고 한다.

6. 문자열, 숫자 또는 원자를 허용하는 파서를 만들어 보자.

   ```F#
   let parseExpr = parseAtom <|>
                   parseString <|>
                   parseNumber
   ```

7. 이제 `parseExpr`을 사용하도록 `readExpr` 함수를 업데이트하자.

   ```F#
   let readExpr input =
       match run (spaces >>. parseExpr) input with
           | Failure (_, err, _) -> sprintf "No match: %s" (err.ToString())
           | Success _ -> "Found Value"
   ```

```
$ dotnet run -- '"this is a string"'
Found value
$ dotnet run -- '25'
Found value
$ dotnet run -- symbol
Found value
$ dotnet run -- '(symbol)'
No match: Error in Ln: 1 Col: 1
Expecting: any char in ‘!#$%&|*+-/:<=>?@^_~’, integer number (64-bit, signed),
letter or '"'
```

