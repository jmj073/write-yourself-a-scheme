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
  + attribute를 사용하면 프로그래밍 구성에 메타데이터를 적용할 수 있다.
+ `let main argv =`: type declaration이다. `main`이 인수 하나를 취하는 함수임을 나타낸다.
  + F#에서 함수는 일반적으로 변수로 정의된다. 둘다 `let` 키워드를 사용한다.
  + `argv`: 프로그팸을 실행할때 사용자가 전달한 인수 배열이다.  
+ **type inference**: F#에는 매우 강력한 type inference(타입 추론) 기능이 있으며. 일반적으로 컨텍스트에 따라 타입을 추론한다. 대부분의 경우 F# 프로그램을 작성하는 동안 형식을 명시적으로 지정할 필요가 없다. 정말 필요한 경우 명시적 타입 annotation을 적용할 수 있다.
  +  `argv` 타입이나 `main` 함수의 반환 값 타입을 지정하지 않은 것을 알 수 있다.
+ **등호**: F#에서 같음은 단일 `=`를 사용하여 검사하며 `==`는 사용하지 않는다.
+ `printfn`:
  + `System` 모듈에서 제공한 콘솔에 텍스트를 쓰기 위해 `printfn` 함수를 사용한다.
  + F#은 순수한 함수형 프로그래밍 언어가 아니며, Haskell과 달리 F#에서 IO 작업을 수행하기 위해 IO 모나드를 제공할 필요가 없다.

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

**3.** `ㄴConsole.ReadLine()`은 콘솔에서 줄을 읽고 문자열로 반환한다. 프로그램을 변경하여 이름을 묻고 이름을 읽은 다음 명령줄 값 대신 해당 이름을 인쇄하자.

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

### Writing a Simple Parser

```F#
let readExpr input =
    match run pSymbol input with
    | Failure (_, err, _) -> sprintf "No match: %s"  (err.ToString())
    | Success _ -> "Found value"
```

+ **match**: `match ... with` construction은 pattern matching의 한 예이다. 나중에 살펴보겠다.

### Whitespace

```F#
let readExpr input =
    match run (spaces >>. pSymbol) input with
    | Failure (_, err, _) -> sprintf "No match: %s"  (err.ToString())
    | Success _ -> "Found value"
```

+ `>>.`: `FParsc`가 제공하는 결합자. 파서 `p1 >>. p2`는 `p1`과 `p2`를 순서대로 파싱하고 `p2`의 결과를 반환한다.
  + `.>>`: `p1`과 `p2`를 순서대로 파싱하지만 `p2` 대신 `p1`의 결과를 반환한다.
  + 두 연산자를 `p1 >>. p2 .>> p3`에 결합하면 `p1`, `p2`,`p3`을 순서대로 파싱하고 `p2`의 결과를 반환한다.
